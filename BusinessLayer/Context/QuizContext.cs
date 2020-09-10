using Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Models;

namespace BusinessLayer.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class QuizContext : DbContext
    {
        private readonly IOptions<AppSettings> settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public QuizContext(DbContextOptions<QuizContext> options, IOptions<AppSettings> settings) : base(options)
        {
            this.settings = settings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<UserToken>()
                .HasKey(c => new { c.UserId, c.DeviceId });
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}
