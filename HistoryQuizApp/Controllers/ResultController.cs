using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task< IActionResult> Index()
        {
            var listTest = await _context.Tests
                .Include(n=>n.Grade)
                .Include(n=>n.User)
                .ToListAsync();
            return View(listTest);
        }
    }
}
