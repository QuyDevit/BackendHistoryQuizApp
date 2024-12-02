using Azure.Core;
using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models;
using HistoryQuizApp.Models.EF;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    [CustomAuthorize("Admin")]
    public class QuestionController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public QuestionController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/question")]
        public async Task<IActionResult> Index(int gradeid = 1, int chapterid = 1)
        {
            ViewBag.SelectedGrade = gradeid;
            ViewBag.SelectedChapter = chapterid;
            var listquestion = await _context.Questions
                .Include(n => n.Grade)
                .Include(n => n.Category)
                .Include(n => n.Chapter)
                .Where(n=>n.ChapterId == chapterid && n.GradeId == gradeid)
                .ToListAsync();
            return View(listquestion);
        }
        [Route("manage/question/add")]
        public  async Task<IActionResult>AddQuestion()
        {
            ViewBag.ListCategory = await _context.Categories.ToListAsync();
            return View();
        }
        [Route("manage/question/edit")]
        public async Task<IActionResult> EditQuestion(int idquestion)
        {
            ViewBag.ListCategory = await _context.Categories.ToListAsync();
            var getQuestion = await _context.Questions.Where(n=>n.Id == idquestion)
                .Include(n=>n.Answers).SingleOrDefaultAsync();
            return View(getQuestion);
        }
        #region handle
        public async Task<IActionResult> GetListChapterByGrade(int idgrade)
        {
            var list = await _context.Chapters.Where(n=>n.GradeId == idgrade).ToListAsync();
            return Json(new {status = true, data = list });
        }
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionRequest request)
        {
            if (string.IsNullOrEmpty(request.Content))
            {
                return Json(new { status = false, msg = "Nội dung câu hỏi không được để trống" });
            }

            if (request.Choices == null || !request.Choices.Any())
            {
                return Json(new { status = false, msg = "Câu hỏi phải có ít nhất một lựa chọn" });
            }

            if (request.Choices.Count(c => c.IsCorrect) == 0)
            {
                return Json(new { status = false, msg = "Phải có ít nhất một lựa chọn đúng" });
            }

            // Tạo đối tượng câu hỏi
            var question = new Question
            {
                Content = request.Content,
                GradeId = request.GradeId,
                Description = request.Description,
                ChapterId = request.ChapterId,
                CategoryId = request.CategoryId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Status = true 
            };

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            int questionId = question.Id;
            var answers = request.Choices.Select(choice => new Answer
            {
                Content = choice.Content,
                IsCorrect = choice.IsCorrect,
                QuestionId = questionId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();

            _context.Answers.AddRange(answers);
            await _context.SaveChangesAsync();

            return Json(new { status = true });
        }
        public async Task<IActionResult> UpdateQuestion([FromBody] QuestionRequest request)
        {
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(request.Content))
            {
                return Json(new { status = false, msg = "Nội dung câu hỏi không được để trống" });
            }

            if (request.Choices == null || !request.Choices.Any())
            {
                return Json(new { status = false, msg = "Câu hỏi phải có ít nhất một lựa chọn" });
            }

            if (request.Choices.Count(c => c.IsCorrect) == 0)
            {
                return Json(new { status = false, msg = "Phải có ít nhất một lựa chọn đúng" });
            }

            // Tìm câu hỏi cần cập nhật
            var question = await _context.Questions
                .Include(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == request.Id);

            if (question == null)
            {
                return Json(new { status = false, msg = "Câu hỏi không tồn tại" });
            }

            // Cập nhật thông tin câu hỏi
            question.Content = request.Content;
            question.GradeId = request.GradeId;
            question.Description = request.Description;
            question.ChapterId = request.ChapterId;
            question.CategoryId = request.CategoryId;
            question.UpdatedAt = DateTime.Now; // Cập nhật thời gian sửa

            // Xóa các lựa chọn cũ
            _context.Answers.RemoveRange(question.Answers);

            // Tạo lại các đáp án mới
            var answers = request.Choices.Select(choice => new Answer
            {
                Content = choice.Content,
                IsCorrect = choice.IsCorrect,
                QuestionId = question.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            }).ToList();

            // Thêm các đáp án mới vào cơ sở dữ liệu
            _context.Answers.AddRange(answers);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return Json(new { status = true, msg = "Cập nhật câu hỏi thành công" });
        }
        public async Task<IActionResult> RemoveQuestion(int idquestion)
        {
            var getuser = await _context.Questions.FindAsync(idquestion);
            if (getuser == null)
            {
                return Json(new { status = false });
            }
            getuser.Status = !getuser.Status;
            _context.SaveChanges();
            return Json(new { status = true });
        }


        #endregion
    }
}
