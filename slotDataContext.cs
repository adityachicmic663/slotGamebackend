using Microsoft.EntityFrameworkCore;
using SlotGameBackend.Models;
namespace SlotGameBackend
{
    public class slotDataContext:DbContext
    {

        public slotDataContext(DbContextOptions<slotDataContext> options) : base(options)
        {

        }
        public DbSet<UserModel> users { get; set; }
        public DbSet<GameSession> gameSessions { get; set; }
        public DbSet<Spin> spinResults { get; set; }
        public DbSet<Transaction> transactions { get; set; }
        public DbSet<Wallet> wallets { get; set; }  
        public DbSet<Symbol> symbols { get; set; }  
        public DbSet<PayLine> payLines { get; set; }

        public DbSet<AdminSettings> settings { get; set; }

        public DbSet<payLinePositions> payLinesPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserModel>()
                .Property(u => u.otpToken)
                .IsRequired(false)
                .HasDefaultValue(null);

            modelBuilder.Entity<UserModel>()
                .Property(u => u.OtpTokenExpiry)
                .IsRequired(false)
                .HasDefaultValue(null);

            modelBuilder.Entity<UserModel>()
                .Property(u => u.profilePicturePath)
                .IsRequired(false)
               .HasDefaultValue(null);

            modelBuilder.Entity<UserModel>()
                .Property(u=>u.lastName)
                .IsRequired(false)
                .HasDefaultValue(null);

            modelBuilder.Entity<UserModel>()
               .HasOne(u => u.wallet)
               .WithOne(w => w.user)
               .HasForeignKey<Wallet>(w => w.userId);

             modelBuilder.Entity<PayLine>()
             .Property(p => p.payLineId)
             .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transaction>()
                .Property(p => p.transactionStatus)
                .HasConversion<string>();
            modelBuilder.Entity<Transaction>()
                .Property(p=>p.type)
                .HasConversion<string>();
        }

        public void SeedData()
        {
            var adminExists = this.users.Any(u => u.userName == "mightyAlpha" && u.role == "admin");
            var password = "Aditya@123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            if (!adminExists)
            {
                var adminUser = new UserModel
                {
                    userId = Guid.NewGuid(),
                    userName = "mightyAlpha",
                    firstName = "aditya",
                    lastName = "bisht",
                    role = "admin",
                    email = "adityabisht8436@gmail.com",
                    hashPassword = hashedPassword
                };

                this.users.Add(adminUser);
                this.SaveChanges();
            }
        }
    }
}
