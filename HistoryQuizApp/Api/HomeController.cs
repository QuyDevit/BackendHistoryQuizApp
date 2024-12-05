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
        [HttpPost("GetLesson")]
        public async Task<IActionResult> GetLesson(RequestLesson request)
        {
            var data = await _context.Lessons.Where(n => n.ChapterId == request.ChapterId).SingleOrDefaultAsync();
            return Json(data);
        }
        [HttpGet("GetListHistoricalFigure")]
        public async Task<IActionResult> GetListHistoricalFigure()
        {
            var data = await _context.HistoricalFigures.Select(n => new
            {
                n.Id,
                n.Name,
                n.ImageUrl,
            }).ToListAsync();
            return Json(data);
        }
        [HttpPost("GetHistoricalFigureById")]
        public async Task<IActionResult> GetHistoricalFigureById(RequestHistoricalFigure request)
        {
            var data = await _context.HistoricalFigures.SingleOrDefaultAsync(n=>n.Id==request.Id);
            return Json(data);
        }
        [HttpPost("GetTest")]
        public async Task<IActionResult> GetTest(RequestTest request)
        {
            if (request.GradeId == 0 || request.GradeId == null)
            {
                return Json(new { status = false, msg = "Chọn lớp không phù hợp" });
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

            List<QuestionRequest> data;

            if (request.GradeId == 4)
            {
                var questionsGrade1 = await _context.Questions
                    .Where(q => q.GradeId == 1)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(7) 
                    .ToListAsync();

                var questionsGrade2 = await _context.Questions
                    .Where(q => q.GradeId == 2)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(7)
                    .ToListAsync();

                var questionsGrade3 = await _context.Questions
                    .Where(q => q.GradeId == 3)
                    .OrderBy(q => Guid.NewGuid())
                    .Take(6) 
                    .ToListAsync();

                var combinedQuestions = questionsGrade1
                    .Concat(questionsGrade2)
                    .Concat(questionsGrade3)
                    .OrderBy(q => Guid.NewGuid())
                    .ToList();

                data = combinedQuestions.Select(q => new QuestionRequest
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
                }).ToList();
            }
            else
            {
                data = await _context.Questions
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
            }

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
        [HttpPost("GetTestById")]
        public async Task<IActionResult> GetTestById(RequestTestById request)
        {
            if (request.TestId == 0 || request.TestId == null)
            {
                return Json(new { status = false, msg = "Chọn lớp không phù hợp" });
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
            var getTest = await _context.Tests.FindAsync(request.TestId);
            if(getTest == null)
            {
                return Json(new { status = false, msg = "Không tìm thấy bài thi!" });
            }
            var questionId = JsonConvert.DeserializeObject<List<int>>(getTest.QuestionIds);
            var listQuestionTest = await _context.Questions
                    .Where(q => questionId.Contains(q.Id))
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
            return Json(listQuestionTest);
        }
        [HttpPost("SendAnswer")]
        public async Task<IActionResult> SendAnswer(RequestAnswerTest request)
        {
            var existingAnswer = await _context.SelectedAnswers
        .SingleOrDefaultAsync(sa => sa.TestId == request.TestId && sa.QuestionId == request.QuestionId);

            if (existingAnswer == null)
            {
                var isCorrect = await _context.Answers
                    .AnyAsync(a => a.QuestionId == request.QuestionId && a.Id == request.AnswerId && a.IsCorrect);

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
        [HttpPost("GetListResult")]
        public async Task<IActionResult> GetListResult()
        {
            var authCookie = Request.Cookies["accesstoken"];
            if (authCookie == null)
            {
                return Json(new { status = false });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(authCookie);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            int id = int.Parse(userId);
            var data = await _context.Tests
                .Include(n => n.Grade) 
                .Where(n => n.UserId == id) 
                .ToListAsync();
            var result = data.Select((n, index) => new
            {
                TestId = n.Id,
                UserId = n.UserId,
                TestTile = $"Đề thi ngẫu nhiên {n.Grade.GradeName} Lần {index + 1}", 
                n.CreatedAt
            }).ToList();
            return Json(result);
        }
        [HttpPost("DetailResult")]
        public async Task<IActionResult> DetailResult(RequestDetailResult request)
        {
            var authCookie = Request.Cookies["accesstoken"];
            if (authCookie == null)
            {
                return Json(new { status = false });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(authCookie);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            int id = int.Parse(userId);
            var getResult = await _context.Tests
                .Where(n => n.Id == request.TestId)
                .Include(n => n.Grade)
                .Include(n => n.User)
                .Select(n => new
                {   n.UserId,
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
            if (getResult.UserId != id)
            {
                return Json(new { status = false, message = "Bạn không có quyền xem bài thi này" });
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
            var questionsTest = questionsWithAnswers.Select(q => new QuestionResult
            {
                Id = q.Question.Id,
                Title = q.Question.Content,
                AnswerSelected = q.SelectedAnswer?.AnswerId ?? 0,
                Description = q.Question.Description,
                IsCorrect = q.SelectedAnswer?.IsCorrect ?? false,
                NoAnswer = q.SelectedAnswer == null ? "Không có đáp án" : "",
                Choices = q.Question.Answers.Select(n => new Choice
                {
                    Id = n.Id,
                    Content = n.Content,
                    IsCorrect = n.IsCorrect,
                }).ToList(),
            }).ToList();
            var correctAnswers = questionsTest.Count(n => n.IsCorrect);

            var testIteration = await _context.Tests
                .CountAsync(t => t.UserId == id && t.CreatedAt <= getResult.CreatedAt);
            var data = new ResultViewModel
            {
                TestId = getResult.Id,
                TitleTest = $"Đề thi ngẫu nhiên {getResult.GradeName} Lần {testIteration}",
                UserName = getResult.FullName,
                CreatedAt = getResult.CreatedAt,
                QuestionsTest = questionsTest,
                CorrectAnswers = correctAnswers,
                PercentageCorrect = (double)correctAnswers / 20 * 100
            };

            return Json(data);
        }
    }
}
