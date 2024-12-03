namespace HistoryQuizApp.Models
{
    public class ResultViewModel
    {
        public int TestId { get; set; }
        public string TitleTest { get; set; }
        public string UserName { get; set; }
        public List<QuestionResult> QuestionsTest{ get; set; }
        public DateTime CreatedAt { get; set; }
        public int CorrectAnswers { get; set; }
        public double PercentageCorrect { get; set; }
    }
    public class QuestionResult
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? AnswerSelected {  get; set; }
        public bool IsCorrect { get; set; }
        public string NoAnswer { get; set; }
        public List<Choice> Choices { get; set; }
    }
}
