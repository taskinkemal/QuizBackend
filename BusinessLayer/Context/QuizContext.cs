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

            modelBuilder.Entity<QuizAssignment>()
                .HasKey(c => new { c.QuizIdentityId, c.Email });

            modelBuilder.Entity<Question>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Option>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<QuestionOption>()
                .HasKey(c => new { c.QuestionId, c.OptionId });

            modelBuilder.Entity<QuizQuestion>()
                .HasKey(c => new { c.QuizId, c.QuestionId });
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

        /// <summary>
        /// 
        /// </summary>
        public DbSet<QuizAssignment> QuizAssignments { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Question> Questions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<Option> Options { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<QuestionOption> QuestionOptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
    }
}
