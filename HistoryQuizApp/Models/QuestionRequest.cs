using HistoryQuizApp.Models.EF;

namespace HistoryQuizApp.Models
{
    public class QuestionRequest
    {
        public int? Id { get; set; } 
        public string Content { get; set; }
        public string Description { get; set; }
        public int? GradeId { get; set; } // Lớp
        public int? ChapterId { get; set; } // CHƯƠNG
        public int? CategoryId { get; set; } // Thể loại
        public List<Choice> Choices { get; set; }
    }
    public class Choice
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public bool IsCorrect { get; set; }
    }
    public class RequestQuestionByChapter
    {
        public int? GradeId { get; set; } // Lớp
        public int? ChapterId { get; set; }
    }
    public class RequestChapterByGrade
    {
        public int? GradeId { get; set; }
    }
    public class RequestLesson
    {
        public int? ChapterId { get; set; }
    }
    public class RequestTest
    {
        public int? GradeId { get; set; }
    }
    public class RequestAnswerTest
    {
        public int TestId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
    }
    public class RequestDetailResult
    {
        public int? TestId { get; set; }
    }
    public class RequestTestById
    {
        public int? TestId { get; set; }
    }
    public class RequestHistoricalFigure
    {
        public int Id { get; set; }
    }
    public class RequestViewHistoricalFigure
    {
        public int Id { get; set; }
    }
}
