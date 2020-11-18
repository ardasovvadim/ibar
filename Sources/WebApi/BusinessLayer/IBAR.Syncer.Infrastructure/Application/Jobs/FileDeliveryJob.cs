using IBAR.Syncer.Infrastructure.Data;
using IBAR.TradeModel.Business.Services.FileServices;
using IBAR.TradeModel.Data.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace IBAR.Syncer.Infrastructure.Application.Jobs
{
    public class FileDeliveryJob : BaseJob
    {
        private readonly IDeliveryJobRepository _deliveryJobRepo;
        private readonly IExtractFileService _extractFileService;
        private readonly IRestApiService _restApiService;

        public FileDeliveryJob(
            IDeliveryJobRepository deliveryJobRepository,
            IExtractFileService extractFileService,
            IRestApiService restApiService)
        {
            var jobInterval = ConfigurationManager.AppSettings["job:FileDeliveryJobInterval"];
            if (string.IsNullOrEmpty(jobInterval))
                throw new ConfigurationErrorsException("Please add 'job:FileDeliveryJobInterval' settigns to .config file.");

            JobInterval = int.Parse(jobInterval);

            _deliveryJobRepo = deliveryJobRepository;
            _extractFileService = extractFileService;
            _restApiService = restApiService;
        }

        protected override async Task RunInternal()
        {
            SyncerInfoService.Log(GetType().Name, JobStatus.Started.ToString());

            using (_deliveryJobRepo.BeginOperation())
            {
                try
                {
                    var filesToSend = _deliveryJobRepo.GetFilesToSendList();

                    if (!filesToSend.Any())
                    {
                        GlobalLogger.LogInfo("Job has not found new files for uploading. Waiting...", GetType().Name, true);
                    }
                    else
                    {
                        var totalCount = 0;
                        foreach (var fileUpload in filesToSend)
                        {
                            totalCount++;
                            var originalFileName = _deliveryJobRepo.GetOriginalFileName(fileUpload.Id);
                            var fileStream = _extractFileService.ExtractFile(originalFileName);

                            try
                            {
                                var response = await _restApiService.GetResponseOfUploadAsync(originalFileName, fileStream);

                                GlobalLogger.LogInfo($@"Response upload file: {originalFileName} to ZOHO api. | Status: {response.StatusCode} | ReasonPhrase: {response.ReasonPhrase} | RequestUri: {response.RequestMessage.RequestUri}", GetType().Name, true);

                                fileUpload.IsSent = response.IsSuccessStatusCode;

                                if (!response.IsSuccessStatusCode)
                                {
                                    GlobalLogger.LogInfo($"File: {originalFileName} unsuccessfully uploaded | Status Code from ZOHO api: {response.StatusCode}", GetType().Name, true);
                                    continue;
                                }

                                fileUpload.SentDate = DateTime.UtcNow;
                                GlobalLogger.LogInfo($"File: {originalFileName} successfully uploaded to ZOHO api.", GetType().Name, true);
                            }
                            catch (Exception ex)
                            {
                                GlobalLogger.LogError($"Error while upload file: {originalFileName} to ZOHO api.", ex, GetType().Name, true);
                            }
                            finally
                            {
                                _deliveryJobRepo.UpdateFileUpload(fileUpload);
                                await _deliveryJobRepo.SaveChangesAsync();
                            }

                            GlobalLogger.LogInfo($"Amount uploaded: [{totalCount}] of [{filesToSend.Count()}].", GetType().Name, true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    GlobalLogger.LogError($"Error while upload files to ZOHO api.", ex, this.GetType().Name, true);
                }
            }

            SyncerInfoService.Log(GetType().Name, JobStatus.Stopped.ToString());
        }
    }
}