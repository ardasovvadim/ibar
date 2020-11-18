using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Runtime.Remoting.Messaging;

namespace IBAR.TradeModel.Business.Services
{
    public interface IExceptionService
    {
        string GetException(string id);
        void SendBugReport(string keyGuid);
    }

    public class ExceptionService : IExceptionService
    {
        private string SupportTeamEmail { get; set; }
        private readonly IMessengerService _messageService;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public ExceptionService(IMessengerService messageService, IUserService userService, IIdentityService identityService)
        {
            _messageService = messageService;
            _userService = userService;
            _identityService = identityService;
            SupportTeamEmail = ConfigurationManager.AppSettings["accountEmail"];
        }

        private const string getById = @"SELECT message FROM ExceptionLog Where Id = @id";

        public string GetException(string id)
        {
            var cstr = ConfigurationManager.ConnectionStrings["log"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(cstr))
            {
                using (SqlCommand com = new SqlCommand(getById, conn))
                {
                    com.Connection.Open();
                    com.Parameters.AddWithValue("@id", Int32.Parse(id));
                    var messageExcp = com.ExecuteScalar();
                    com.Connection.Close();
                    return messageExcp.ToString();
                }
            }
        }

        public void SendBugReport(string keyGuid)
        {
            var to = SupportTeamEmail;
            var title = "Bug report";
            var from = _userService.GetById(_identityService.GetIdentityId()).Email;
            var message = $"<p>Reporter: {from}</p>" + $"<p>Guid: {keyGuid}</p>";
            
            _messageService.SendEmail(to, message, title, true);
        }
    }
}