using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryQuizApp.Models.EF
{
    public class Question
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Description { get; set; }
        [ForeignKey("GradeId")]
        public int? GradeId { get; set; } // Lớp
        public virtual Grade Grade { get; set; }
        [ForeignKey("ChapterId")]
        public int? ChapterId { get; set; } // CHƯƠNG
        public virtual Chapter Chapter{ get; set; }
        [ForeignKey("CategoryId")]
        public int? CategoryId { get; set; } // Thể loại
        public virtual Categories Category{ get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public bool Status { get; set; }
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
