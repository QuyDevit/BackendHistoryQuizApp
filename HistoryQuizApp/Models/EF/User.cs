using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HistoryQuizApp.Models.EF
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? School { get; set; } // Trường
        [ForeignKey("GradeId")]
        public int? GradeId { get; set; } // Lớp
        public virtual Grade Grade { get; set; }
        public string Role { get; set; } = "User"; 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public bool Status { get; set; }
        public string? LinkToken { get; set; }
        public string? VerificationCode { get; set; }

    }
}
