using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HistoryQuizApp.Models.EF
{
    [Table("Chapter")]
    public class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("GradeId ")]
        public int? GradeId { get; set; }
        public virtual Grade Grade { get; set; }
        public string ChapterName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;
    }
}
