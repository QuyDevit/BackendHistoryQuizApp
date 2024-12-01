namespace HistoryQuizApp.Models.EF
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string Role { get; set; } = "User"; // Default is "User"
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        // Navigation property
    }
}
