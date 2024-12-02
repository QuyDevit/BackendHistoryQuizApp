using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HistoryQuizApp.Models.EF
{
    [Table("SelectedAnswer")]
    public class SelectedAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TestId")]
        public int TestId { get; set; }
        public virtual Test Test { get; set; }
        [ForeignKey("QuestionId")]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
        [ForeignKey("AnswerId")]
        public int AnswerId { get; set; }
        public virtual Answer Answer { get; set; }
        public bool IsCorrect { get; set; }
        public DateTime SelectedAt { get; set; }
    }
}
