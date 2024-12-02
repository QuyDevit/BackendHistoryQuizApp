using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public CategoryController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/category")]
        public async Task<IActionResult> Index()
        {
            var listcategory = await _context.Categories.ToListAsync();
            return View(listcategory);
        }
        #region handle
        public async Task<IActionResult> AddCategory(string name, string description)
        {
            if ( string.IsNullOrEmpty(name))
            {
                return Json(new { status = false, msg = "Vui lòng nhập đầy đủ!" });
            }
            var checkcategory= await _context.Categories.SingleOrDefaultAsync(n => n.CategoryName == name);
            if (checkcategory != null)
            {
                return Json(new { status = false });
            }
            var newcategory = new Categories
            {
                CategoryName = name,
                Description = description == null ? "" : description
            };
            _context.Categories.Add(newcategory);
            _context.SaveChanges();
            return Json(new { status = true });
        }
        public async Task<IActionResult> GetInfoCategory(int idcategory)
        {
            var getdata = await _context.Categories.FindAsync(idcategory);
            if (getdata == null)
            {
                return Json(new { status = false });
            }

            return Json(new { status = true, data = getdata });
        }
        public async Task<IActionResult> SaveCategory(int idcategory, string name, string description)
        {
            var getdata = await _context.Categories.FindAsync(idcategory);
            if (getdata == null)
            {
                return Json(new { status = false });
            }
            getdata.CategoryName = name;
            getdata.Description = description == null ? "" : description;
            _context.SaveChanges();

            return Json(new { status = true });
        }
        public async Task<IActionResult> RemoveCategory(int idcategory)
        {
            var getdata = await _context.Categories.FindAsync(idcategory);
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
