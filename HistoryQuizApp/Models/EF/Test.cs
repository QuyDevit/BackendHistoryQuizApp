using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryQuizApp.Models.EF
{
    [Table("Test")]
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int? UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("GradeId")]
        public int? GradeId { get; set; }
        public virtual Grade Grade { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Status { get; set; }  
        public string QuestionIds { get; set; }
    }
}
