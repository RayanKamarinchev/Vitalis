using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Vitalis.Data.Entities;

namespace Vitalis.Data
{
    public class VitalisDbContext : IdentityDbContext
    {
        public VitalisDbContext(DbContextOptions<VitalisDbContext> options)
            : base(options)
        {
            if (Database.IsRelational())
            {
                Database.Migrate();
            }
            else
            {
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TestLike>()
                   .HasKey(p => new { p.TestId, p.UserId });
            builder.Entity<TestResult>()
                   .HasKey(p => new { p.TestId, p.TestTakerId });

            base.OnModelCreating(builder);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<ClosedQuestionAnswer> ClosedQuestionAnswers { get; set; }
        public DbSet<OpenQuestionAnswer> OpenQuestionAnswers { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<OpenQuestion> OpenQuestions { get; set; }
        public DbSet<ClosedQuestion> ClosedQuestions { get; set; }
        public DbSet<TestResult> TestResults { get; set; }
    }
}
