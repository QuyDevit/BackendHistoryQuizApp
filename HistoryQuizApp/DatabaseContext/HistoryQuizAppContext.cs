using HistoryQuizApp.Models.EF;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HistoryQuizApp.DatabaseContext
{
    public class HistoryQuizAppContext :DbContext
    {
        public HistoryQuizAppContext(DbContextOptions<HistoryQuizAppContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<SelectedAnswer> SelectedAnswers { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
