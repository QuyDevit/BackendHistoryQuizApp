using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models.EF;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    [CustomAuthorize("Admin")]
    public class LessonController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public LessonController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/lesson")]
        public async Task< IActionResult> Index(int gradeid = 1)
        {
            ViewBag.SelectedGrade = gradeid;
            var listChapterIds = await _context.Chapters
               .Where(n => n.GradeId == gradeid)
               .Select(n => n.Id) 
               .ToListAsync();
            var listlesson = await _context.Lessons
                .Where(n=> listChapterIds.Contains(n.ChapterId.Value))
                .Include(n=>n.Chapter)
                .ToListAsync();
            return View(listlesson);
        }
        [Route("manage/lesson/add")]
        public  IActionResult AddLesson()
        {
            return View();
        }
        [Route("manage/lesson/edit")]
        public async Task< IActionResult> EditLesson(int idlesson)
        {
            var data = await _context.Lessons.Include(n=>n.Chapter).FirstOrDefaultAsync(n=>n.Id == idlesson);
            var gradeId = await _context.Chapters.FindAsync(data.ChapterId);
            ViewBag.GradeId = gradeId.GradeId;
            return View(data);
        }
        public async Task< IActionResult> CreateLesson(string name, string content,int chapter)
        {
            var checklesson = await _context.Lessons.FirstOrDefaultAsync(n => n.ChapterId == chapter);
            if (checklesson != null) {
                return Json(new { status = false,msg = "Bài học đã tồn tại!" });
            }
            var newLeson = new Lesson
            {
                ChapterId = chapter,
                Content = content,
                Name = name,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };
            _context.Lessons.Add(newLeson);
            _context.SaveChanges();
            return Json(new {status = true});
        }
        public async Task<IActionResult> UpdateLesson(int idlesson,string name, string content)
        {
            var checklesson = await _context.Lessons.FindAsync(idlesson);
            if (checklesson == null)
            {
                return Json(new { status = false, msg = "Bài học đã tồn tại!" });
            }
            checklesson.Name = name;
            checklesson.Content = content;
            checklesson.UpdatedAt = DateTime.Now;
            _context.SaveChanges();
            return Json(new { status = true });
        }

    }
}
