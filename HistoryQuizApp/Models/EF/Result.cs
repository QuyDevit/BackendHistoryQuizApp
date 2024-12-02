using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HistoryQuizApp.Models.EF
{
    [Table("Result")]
    public class Result
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("TestId")]
        public int? TestId { get; set; }
        public virtual Test Test { get; set; }
        public int Score { get; set; } // Điểm số
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; } // Số câu trả lời đúng
        public DateTime StartTime { get; set; } // Thời gian bắt đầu làm bài
        public DateTime EndTime { get; set; } // Thời gian kết thúc làm bài
        public string Status { get; set; }
    }
}
