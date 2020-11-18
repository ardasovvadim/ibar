using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using IBAR.TradeModel.Data.Entities;

namespace IBAR.TradeModel.Data
{
    // TODO: Get rid of Seeder in the near future! Use sql script by default instead of Seeder.
    public class TradeModelDbInitializer : CreateDatabaseIfNotExists<TradeModelContext>
    {
        protected override void Seed(TradeModelContext context)
        {
            SeedMasterAccounts(context);
            SeedUsers(context);
            SeedFtpCredentials(context);
            SeedTransitFiles(context);
            SeedFileNameRegexes(context);

            base.Seed(context);
        }

        private void SeedFtpCredentials(TradeModelContext context)
        {
            var masterAccounts = context.MasterAccounts.ToList();

            // TODO: uncoment
            var initialFtpCredentials = new List<FtpCredential>
            {
                // TODO: your credentials
            };

            context.FtpCredentials.AddRange(initialFtpCredentials);

            context.SaveChanges();
        }

        private void SeedMasterAccounts(TradeModelContext context)
        {
            var initialMasterAccounts = new List<MasterAccount>
            {
                new MasterAccount {AccountName = "I2078970", AccountAlias = "Mexem"},
                new MasterAccount {AccountName = "I2174453", AccountAlias = "afrika"},
                new MasterAccount {AccountName = "I533594", AccountAlias = "RTL"},
                new MasterAccount {AccountName = "I3276148", AccountAlias = "Zero"},
                new MasterAccount {AccountName = "F3490885", AccountAlias = "portfolio"},
                new MasterAccount {AccountName = "I1191921"},
                new MasterAccount {AccountName = "I1259743"},
                new MasterAccount {AccountName = "I2580098"},
                new MasterAccount {AccountName = "I8036922", AccountAlias="Hong Kong"}
            };

            context.MasterAccounts.AddRange(initialMasterAccounts);

            context.SaveChanges();
        }

        private void SeedUsers(TradeModelContext context)
        {
            var initialRoles = new List<Role> { new Role { Name = "user" }, new Role { Name = "admin" } };

            context.Roles.AddRange(initialRoles);

            var initialAdmin = new User
            {
                FirstName = "Admin",
                Email = "admin@sytoss.com",
                Password = "your_password",
                Phone = "+380951234567",
                Roles = initialRoles
            };

            context.Users.Add(initialAdmin);

            context.SaveChanges();
        }

        private void SeedTransitFiles(TradeModelContext context)
        {
            var transitFiles = new TransitFiles[]
            {
                new TransitFiles { AccountName ="I2078970", OriginalFileName="acct_status_report_non_eca" },
                new TransitFiles { AccountName ="I2078970", OriginalFileName="acct_status_report"  },
                new TransitFiles { AccountName ="I2078970", OriginalFileName="Commissions_CRM" },
                new TransitFiles { AccountName ="I2078970", OriginalFileName="All_kinds_of_data_CRM" },
                new TransitFiles { AccountName ="I2078970", OriginalFileName="Ending_value_CRM" },
                new TransitFiles { AccountName ="I3276148", OriginalFileName="acct_status_report_non_eca" },
                new TransitFiles { AccountName ="I2580098", OriginalFileName="acct_status_report_non_eca" },
                new TransitFiles { AccountName ="I3276148", OriginalFileName="acct_status_report" },
                new TransitFiles { AccountName ="I2580098", OriginalFileName="acct_status_report" },
                new TransitFiles { AccountName ="I3276148", OriginalFileName="Commissions_CRM" },
                new TransitFiles { AccountName ="I2580098", OriginalFileName="Commissions_CRM" },
                new TransitFiles { AccountName ="I3276148", OriginalFileName="All_kinds_of_data_CRM" },
                new TransitFiles { AccountName ="I2580098", OriginalFileName="All_kinds_of_data_CRM" },
                new TransitFiles { AccountName ="I3276148", OriginalFileName="Ending_value_CRM" },
                new TransitFiles { AccountName ="I2580098", OriginalFileName="Ending_value_CRM" }
            };

            context.TransitFiles.AddRange(transitFiles);
            context.SaveChanges();
        }

        private void SeedFileNameRegexes(TradeModelContext context)
        {
            var transitFiles = new FileNameRegex[]
            {
                new FileNameRegex { FileName ="ClientFeesRegex", FileRegex=@"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-clientfees-det\.\d{8}\.\d{8}" },
                new FileNameRegex { FileName ="AcctStatusReportNonEcaRegex", FileRegex=@"acct_status_report_non_eca\.[A-Za-z]{1}\d{5,7}\.\d{8}" },
                new FileNameRegex { FileName ="AcctStatusReportRegex", FileRegex=@"acct_status_report\.[A-Za-z]{1}\d{5,7}\.\d{8}" },
                new FileNameRegex { FileName ="SytossCashReportRegex", FileRegex=@"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Cash\.\d{8}\.\d{8}"},
                new FileNameRegex { FileName ="SytossNavRegex", FileRegex=@"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Nav\.\d{8}\.\d{8}"},
                new FileNameRegex { FileName ="SytossTradesAsRegex", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Trades-ass\.\d{8}\.\d{8}"},
                new FileNameRegex { FileName ="CommissionsCrmRegex", FileRegex= @"[A-Za-z]{1}\d{5,7}\.Commissions_CRM" },
                new FileNameRegex { FileName ="AllKindsOfDataCrmRegex", FileRegex= @"[A-Za-z]{1}\d{5,7}\.All_kinds_of_data_CRM" },
                new FileNameRegex { FileName ="EndingValueCrmRegex", FileRegex= @"[A-Za-z]{1}\d{5,7}\.Ending_value_CRM" },
                new FileNameRegex { FileName ="SytossClientInfoRegex", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Client_info\.\d{8}\.\d{8}" },
                new FileNameRegex { FileName ="SytossOpenPositions", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Open_Positions\.\d{8}\.\d{8}" },
                new FileNameRegex { FileName ="SytossTradesEXE", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Trades-EXE\.\d{8}\.\d{8}" },
                new FileNameRegex { FileName ="SytossInterestAccrua", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-InterestAccrua\.\d{8}\.\d{8}" },
                new FileNameRegex { FileName ="SytossCommissionsDet", FileRegex= @"[A-Za-z]{1}\d{5,7}\.[A-Za-z]{1}\d{5,7}-Sytoss-Commission_Det\.\d{8}\.\d{8}" }
            };

            context.FileNameRegexes.AddRange(transitFiles);
            context.SaveChanges();
        }
    }
}