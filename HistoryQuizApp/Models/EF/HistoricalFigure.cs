using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryQuizApp.Models.EF
{
    [Table("HistoricalFigure")]
    public class HistoricalFigure
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string VideoUrl { get; set; }
        public string Content { get; set; }

    }
}
