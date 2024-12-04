using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Linq;

namespace HistoryQuizApp.Controllers
{
    [CustomAuthorize("Admin")]
    public class ResultController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public ResultController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/result")]
        public async Task< IActionResult> Index(int gradeid = 1)
        {
            ViewBag.SelectedGrade = gradeid;
            var listTest = await _context.Tests
                .Where(n=>n.GradeId == gradeid)
                .Include(n=>n.Grade)
                .Include(n=>n.User)
                .ToListAsync();
            return View(listTest);
        }
        [Route("manage/result/detail")]
        public async Task<IActionResult> DetailResult(int idlesson)
        {
            var getResult = await _context.Tests
                .Where(n => n.Id == idlesson)
                .Include(n => n.Grade)
                .Include(n => n.User)
                .Select(n => new 
                {
                    n.Id,
                    n.Grade.GradeName,
                    n.User.FullName,
                    n.QuestionIds,
                    n.CreatedAt
                })
                .SingleOrDefaultAsync();
            if (getResult == null)
            {
                return Json(new { status = false, message = "Không tìm thấy bài thi" });
            }
            var questionIds = JsonConvert.DeserializeObject<List<int>>(getResult.QuestionIds);
            var questionsWithAnswers = await _context.Questions
                  .Where(q => questionIds.Contains(q.Id))
                  .Include(q => q.Answers)
                  .Select(q => new
                  {
                      Question = q,
                      SelectedAnswer = _context.SelectedAnswers
                          .FirstOrDefault(sa => sa.TestId == getResult.Id && sa.QuestionId == q.Id)
                  })
                  .ToListAsync();
            var questionsTest = questionsWithAnswers.Select(q=> new QuestionResult
            {
                Id = q.Question.Id,
                Title= q.Question.Content,
                AnswerSelected = q.SelectedAnswer?.AnswerId ?? 0,
                Description= q.Question.Description,
                IsCorrect = q.SelectedAnswer?.IsCorrect ?? false,
                NoAnswer = q.SelectedAnswer == null ? "Không có đáp án" : "",
                Choices = q.Question.Answers.Select(n=> new Choice
                {
                    Id= n.Id,
                    Content= n.Content,
                    IsCorrect =n.IsCorrect,
                }).ToList(),
            }).ToList();
            var correctAnswers = questionsTest.Count(n => n.IsCorrect);
            var data = new ResultViewModel
            {
                TestId = getResult.Id,
                TitleTest = $"Đề thi ngẫu nhiên {getResult.GradeName}",
                UserName = getResult.FullName,
                CreatedAt = getResult.CreatedAt,
                QuestionsTest = questionsTest,
                CorrectAnswers = correctAnswers,
                PercentageCorrect = (double)correctAnswers / 20 * 100
            };

            return View(data);
        }
    }
}
