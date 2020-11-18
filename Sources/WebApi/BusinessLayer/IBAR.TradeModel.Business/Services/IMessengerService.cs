using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using PhoneNumbers;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using PhoneNumber = Twilio.Types.PhoneNumber;

namespace IBAR.TradeModel.Business.Services
{
    public interface IMessengerService
    {
        void SendSms(string number, string message);
        void SendEmail(string email, string message, string title, bool isHtml=false);
    }

    public class MessengerService : IMessengerService
    {
        private string AccEmailFrom { get; set; }
        private string AccPasswordFrom { get; set; }

        public MessengerService()
        {
            AccEmailFrom = ConfigurationManager.AppSettings["accountEmail"];
            AccPasswordFrom = ConfigurationManager.AppSettings["accountPassword"];
            if (AccEmailFrom == null || AccPasswordFrom == null)
            {
                throw new ConfigurationErrorsException(
                    "Please configure account email/password settings in web.config file.");
            }

            var twilioSettings = ConfigurationManager.GetSection("twilioSettings") as NameValueCollection;
            if (twilioSettings == null)
                throw new ConfigurationErrorsException("Please configure twilioSettings in web.config file.");

            var twilioAccountSid = twilioSettings["accountSid"];
            var twilioAuthToken = twilioSettings["authToken"];

            TwilioClient.Init(twilioAccountSid, twilioAuthToken);
        }

        public void SendSms(string number, string message)
        {
            var phoneUtil = PhoneNumberUtil.GetInstance();
            var phoneNumber = $"+{number}";
            if (!phoneUtil.IsValidNumber(phoneUtil.Parse(phoneNumber, null)))
            {
                throw new ApplicationException($"Phone number: {phoneNumber} is invalid.");
            }

            MessageResource.Create(
                body: message,
                @from: new PhoneNumber("+32460236101"),
                to: new PhoneNumber(phoneNumber));
        }

        public void SendEmail(string email, string message, string title, bool isHtml = false)
        {
            var from = new MailAddress(AccEmailFrom, "IBA Reporting Support Team");

            using (var client = new SmtpClient()
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(AccEmailFrom, AccPasswordFrom),
                Port = 587,
                Host = "smtp.gmail.com",
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network
            })
            {
                using (var msg = new MailMessage())
                {
                    msg.From = from;
                    msg.To.Add(email);
                    msg.Subject = title;
                    msg.Body = message;
                    msg.IsBodyHtml = isHtml;

                    client.Send(msg);
                }
            }
        }

    }
}