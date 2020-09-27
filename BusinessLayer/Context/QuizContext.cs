using Microsoft.EntityFrameworkCore;
using Models.DbModels;

namespace BusinessLayer.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public QuizContext(DbContextOptions<QuizContext> options) : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<UserToken>()
                .HasKey(c => new { c.UserId, c.DeviceId });

            modelBuilder.Entity<OneTimeToken>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<QuizIdentity>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Quiz>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Quiz>()
                .HasOne<QuizIdentity>()
                .WithMany()
                .HasForeignKey(p => p.QuizId);

            modelBuilder.Entity<QuizAttempt>()
                .HasKey(c => c.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<UserToken> UserTokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<OneTimeToken> OneTimeTokens { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Quiz> Quizes { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<QuizIdentity> QuizIdentities { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
    }
}
