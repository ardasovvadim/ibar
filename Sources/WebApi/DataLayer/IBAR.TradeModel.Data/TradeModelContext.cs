using IBAR.TradeModel.Data.Entities;
using IBAR.TradeModel.Data.Entities.Trade;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBAR.TradeModel.Data
{
    public class TradeModelContext : DbContext
    {
        static TradeModelContext()
        {
#if (DEBUG || STAGE)
            // if on RELEASE env was dropped DB -> use initializer
            Database.SetInitializer<TradeModelContext>(new TradeModelDbInitializer());
#elif (RELEASE || PRODUCTION)
            Database.SetInitializer<TradeModelContext>(null);
#endif
        }

        public TradeModelContext() : base("TradeDbContext")
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<decimal>()
                        .Configure(c => c.HasPrecision(18, 9));

            modelBuilder.Entity<TradeAccount>()
                        .HasMany(a => a.TradeFees)
                        .WithRequired(f => f.TradeAccount)
                        .HasForeignKey(a => a.TradeAccountId)
                        .WillCascadeOnDelete(false);

            modelBuilder.Entity<ImportedFile>()
                        .HasOptional(f => f.FileUpload)
                        .WithRequired(u => u.ImportedFile);

            modelBuilder.Entity<FileUpload>()
                        .Property(u => u.Id)
                        .HasColumnName("ImportedFileId")
                        .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            modelBuilder.Entity<MasterAccount>()
                .HasMany(a => a.TradeAccounts)
                .WithRequired(a => a.MasterAccount)
                .HasForeignKey(a => a.MasterAccountId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ImportedFile>()
                .HasMany(f => f.TradeAccounts)
                .WithRequired(a => a.ImportedFile)
                .HasForeignKey(a => a.ImportedFileId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<FtpCredential>()
                .HasMany(f => f.MasterAccounts)
                .WithMany(a => a.FtpCredentials);

            modelBuilder.Entity<FtpCredential>()
                .HasMany(f => f.ImportedFiles)
                .WithRequired(f => f.FtpCredential)
                .HasForeignKey(f => f.FtpCredentialId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.CreatedMasterAccounts)
                .WithOptional(m => m.CreatedBy)
                .HasForeignKey(m => m.CreatedById)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasMany(u => u.UpdatedMasterAccounts)
                .WithOptional(m => m.UpdatedBy)
                .HasForeignKey(m => m.UpdatedById)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            UpdateIAuditableRecords();

            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var validationErrors = new StringBuilder();
                validationErrors.AppendLine(ex.Message);
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    validationErrors.AppendLine(validationError.Entry.Entity.GetType().Name);
                    foreach (var dbValidationError in validationError.ValidationErrors)
                        validationErrors.AppendLine(dbValidationError.ErrorMessage);
                }
                throw new DbEntityValidationException($"There were issues validating entities when saving changes: {validationErrors}. See the inner exception for more details.", ex);
            }
        }

        public override async Task<int> SaveChangesAsync()
        {
            UpdateIAuditableRecords();

            try
            {
                return await base.SaveChangesAsync();
            }
            catch (DbEntityValidationException ex)
            {
                var validationErrors = new StringBuilder();
                validationErrors.AppendLine(ex.Message);
                foreach (var validationError in ex.EntityValidationErrors)
                {
                    validationErrors.AppendLine(validationError.Entry.Entity.GetType().Name);
                    foreach (var dbValidationError in validationError.ValidationErrors)
                        validationErrors.AppendLine(dbValidationError.ErrorMessage);
                }
                throw new DbEntityValidationException($"There were issues validating entities when saving changes: {validationErrors}. See the inner exception for more details.", ex);
            }
        }

        // Update all objects that can be cast to <see cref="IAuditable"/> in the change tracking context based on their
        // To update how auditing fields are updated override this method.
        protected virtual void UpdateIAuditableRecords()
        {
            var auditableEntities = ChangeTracker.Entries()
                                                 .Where(entry => (entry.State != EntityState.Deleted)
                                                                 && (entry.State != EntityState.Unchanged)
                                                                 && (entry.Entity as IAuditable != null))
                                                 .Select(record => record.Entity)
                                                 .OfType<IAuditable>()
                                                 .ToList();

            foreach (var item in auditableEntities)
            {
                switch (Entry(item).State)
                {
                    case EntityState.Added:
                        item.CreatedDate = DateTime.UtcNow;
                        break;
                    case EntityState.Detached:
                    case EntityState.Modified:
                        item.ModifiedDate = DateTime.UtcNow;
                        Entry(item).Property(t => t.CreatedDate).IsModified = false;
                        break;
                }
            }
        }

        #region Trade Entities
        public virtual DbSet<MasterAccount> MasterAccounts { get; set; }
        public virtual DbSet<TradeAccount> TradeAccounts { get; set; }
        public virtual DbSet<TradeFee> TradeFees { get; set; }
        public virtual DbSet<TradeFeeType> TradeFeeTypes { get; set; }
        public virtual DbSet<TradeInstrument> TradeInstruments { get; set; }
        public virtual DbSet<ImportedFile> ImportedFiles { get; set; }
        public virtual DbSet<FileUpload> FileUploads { get; set; }
        public virtual DbSet<TradeNav> TradeNavs { get; set; }
        public virtual DbSet<TradeCash> TradeCashes { get; set; }
        public virtual DbSet<TradeTradesAs> TradeTradesAs { get; set; }
        public virtual DbSet<FtpCredential> FtpCredentials { get; set; }
        public virtual DbSet<TransitFiles> TransitFiles { get; set; }
        public virtual DbSet<FileNameRegex> FileNameRegexes { get; set; }
        public virtual DbSet<TradingPermission> TradingPermissions { get; set; }
        public virtual DbSet<TradeAccountNote> TradeAccountNotes { get; set; }
        public virtual DbSet<TradeSytossOpenPosition> TradeSytossOpenPositions { get; set; }
        public virtual DbSet<TradesExe> TradesExes { get; set; }
        public virtual DbSet<TradeInterestAccrua> TradeInterestAccruas { get; set; }
        public virtual DbSet<TradeCommissions> TradeCommissions { get; set; }
        public virtual DbSet<SyncerInfo> SyncerInfos { get; set; }
        #endregion

        #region User Entities
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Invite> Invites { get; set; }
        public virtual DbSet<LogInfo> LogInfos { get; set; }
        #endregion
    }
}