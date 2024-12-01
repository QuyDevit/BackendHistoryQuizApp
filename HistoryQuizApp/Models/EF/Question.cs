using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryQuizApp.Models.EF
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        [ForeignKey("GradeId")]
        public int? GradeId { get; set; } // Lớp
        public virtual Grade Grade { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool Status { get; set; }
    }
}
