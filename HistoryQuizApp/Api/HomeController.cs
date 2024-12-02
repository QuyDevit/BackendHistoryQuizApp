using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models.EF;
using HistoryQuizApp.Models;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace HistoryQuizApp.Api
{
    [CustomAuthorize("User")]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public HomeController(HistoryQuizAppContext context) {
            _context = context;
        }
        [HttpPost("GetListGrade")]
        public async Task<IActionResult> GetListGrade()
        {
            var data = await _context.Grades.ToListAsync();
            return Json(data);
        }
        [HttpPost("GetListQuestionByGradeAndChapter")]
        public async Task<IActionResult> GetListQuestionByGradeAndChapter(RequestQuestionByChapter request)
        {
            var data = await _context.Questions
                .Where(q => q.GradeId == request.GradeId && q.ChapterId == request.ChapterId)
                .Select(q => new QuestionRequest
                {
                    Id = q.Id,
                    Content = q.Content,
                    Description = q.Description,
                    GradeId = q.GradeId,
                    ChapterId = q.ChapterId,
                    CategoryId = q.CategoryId,
                    Choices = _context.Answers
                        .Where(a => a.QuestionId == q.Id)
                        .Select(a => new Choice
                        {
                            Content = a.Content,
                            IsCorrect = a.IsCorrect,
                        }).ToList()
                }).ToListAsync();

            return Json(data);
        }

        [HttpPost("GetListChapter")]
        public async Task<IActionResult> GetListChapter(RequestChapterByGrade request)
        {
            var data = await _context.Chapters.Where(n=>n.GradeId == request.GradeId).ToListAsync();
            return Json(data);
        }
        [AllowAnonymous]
        [HttpPost("GetLesson")]
        public async Task<IActionResult> GetLesson(RequestLesson request)
        {
            var data = await _context.Lessons.Where(n => n.ChapterId == request.ChapterId).SingleOrDefaultAsync();
            return Json(data);
        }
        [AllowAnonymous]
        [HttpPost("GetTest")]
        public async Task<IActionResult> GetTest(RequestTest request)
        {
            if (request.GradeId == 0 || request.GradeId == null)
            {
                return Json(new {status = false,msg = "Chọn lớp ko phù hợp"});
            }
            var authCookie = Request.Cookies["accesstoken"];
            if (authCookie == null)
            {
                return Json(new { status = false, msg = "Bạn không có quyền truy cập!" });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(authCookie);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            int userid = int.Parse(userId);
            var data = await _context.Questions
               .Where(q => q.GradeId == request.GradeId)
                .OrderBy(q => Guid.NewGuid())
                .Take(20)
               .Select(q => new QuestionRequest
               {
                   Id = q.Id,
                   Content = q.Content,
                   Description = q.Description,
                   GradeId = q.GradeId,
                   ChapterId = q.ChapterId,
                   CategoryId = q.CategoryId,
                   Choices = _context.Answers
                       .Where(a => a.QuestionId == q.Id)
                       .Select(a => new Choice
                       {
                           Id = a.Id,
                           Content = a.Content,
                           IsCorrect = a.IsCorrect,
                       }).ToList()
               }).ToListAsync();
            var questionIds = data.Select(q => q.Id).ToList();
            var questionIdsJson = JsonConvert.SerializeObject(questionIds);
            var test = new Test
            {
                GradeId = request.GradeId,
                UserId = userid, 
                QuestionIds = questionIdsJson, 
                Status = true 
            };
            _context.Tests.Add(test);
            await _context.SaveChangesAsync();
            var response = new 
            {
                TestId = test.Id,
                ListQuestion = data
            };
            return Json(response);
        }
        [AllowAnonymous]
        [HttpPost("SendAnswer")]
        public async Task<IActionResult> SendAnswer(RequestAnswerTest request)
        {
            var existingAnswer = await _context.SelectedAnswers
        .SingleOrDefaultAsync(sa => sa.TestId == request.TestId && sa.QuestionId == request.QuestionId);

            if (existingAnswer == null)
            {
                var isCorrect = await _context.Answers
                    .AnyAsync(a => a.QuestionId == request.QuestionId && a.Id == request.AnswerId && a.IsCorrect);

                // Create and add the new selected answer
                var newSelectedAnswer = new SelectedAnswer
                {
                    TestId = request.TestId,
                    QuestionId = request.QuestionId,
                    AnswerId = request.AnswerId,
                    IsCorrect = isCorrect,
                    SelectedAt = DateTime.UtcNow,
                };

                await _context.SelectedAnswers.AddAsync(newSelectedAnswer);
            }
            else
            {
                existingAnswer.AnswerId = request.AnswerId;
            }

            await _context.SaveChangesAsync();
            return Json(new {status = true});
        }
    }
}
