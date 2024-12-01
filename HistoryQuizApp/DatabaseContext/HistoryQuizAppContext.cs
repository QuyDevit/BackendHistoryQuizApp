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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
