using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models;
using HistoryQuizApp.Models.EF;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    [CustomAuthorize("Admin")]
    public class ChapterController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public ChapterController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/chapter")]
        public async Task<IActionResult> Index()
        {
            var listchapter = await _context.Chapters.Include(n=>n.Grade).ToListAsync();
            ViewBag.ListGrade = await _context.Grades.ToListAsync();
            return View(listchapter);
        }
        #region handle
        public async Task<IActionResult> AddChapter(int idgrade, string name, string description)
        {
            if (idgrade == 0 || string.IsNullOrEmpty(name)) {
                return Json(new { status = false ,msg = "Vui lòng nhập đầy đủ!"});
            }
            var checkchapter = await _context.Chapters.SingleOrDefaultAsync(n=>n.ChapterName == name && n.GradeId == idgrade);
            if (checkchapter != null) {
                return Json(new { status = false });
            }
            var newchapter = new Chapter
            {
                 GradeId = idgrade,
                 ChapterName = name,
                 Description = description == null ? "":description
            };
            _context.Chapters.Add(newchapter);
            _context.SaveChanges();
            return Json(new {status = true});
        }
        public async Task<IActionResult> GetInfoChapter(int idchapter)
        {
            var getdata = await _context.Chapters.FindAsync(idchapter);
            if (getdata == null)
            {
                return Json(new { status = false });
            }

            return Json(new { status = true, data = getdata });
        }
        public async Task<IActionResult> SaveChapter(int idchapter, int idgrade, string name, string description)
        {
            var getdata = await _context.Chapters.FindAsync(idchapter);
            if (getdata == null)
            {
                return Json(new { status = false });
            } 
            getdata.ChapterName = name;
            getdata.Description = description == null ? "" : description;
            getdata.GradeId = idgrade;
            _context.SaveChanges();

            return Json(new { status = true });
        }
        public async Task<IActionResult> RemoveChapter(int idchapter)
        {
            var getdata = await _context.Chapters.FindAsync(idchapter);
            if (getdata == null)
            {
                return Json(new { status = false });
            }
            getdata.Status = !getdata.Status;
            _context.SaveChanges();
            return Json(new { status = true });
        }
        #endregion
    }
}
