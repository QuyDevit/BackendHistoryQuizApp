using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models.EF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    public class HistoricalFigureController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public HistoricalFigureController(HistoryQuizAppContext context)
        {
            _context = context;
        }
        [Route("manage/historicalfigure")]
        public async Task< IActionResult> Index()
        {
            var listHistoricalFigure = await _context.HistoricalFigures.ToListAsync();
            return View(listHistoricalFigure);
        }
        [Route("manage/historicalfigure/add")]
        public IActionResult AddHistoricalFigure()
        {
            return View();
        }

        [Route("manage/historicalfigure/edit")]
        public async Task<IActionResult> EditHistoricalFigure(int id)
        {
            var getHistoricalFigure = await _context.HistoricalFigures.FindAsync(id);
            if(getHistoricalFigure == null)
            {
                return RedirectToAction("Index");
            }
            return View(getHistoricalFigure);
        }
        public async Task <IActionResult> CreateHistoricalFigure(string name, string content, IFormFile avatar, string urlvideo)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content) || avatar.Length == 0 || string.IsNullOrEmpty(urlvideo))
                {
                    return Json(new { status = false });
                }
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "src", "img");
                var fileExtension = Path.GetExtension(avatar.FileName);
                var uniqueFileName = Guid.NewGuid() + fileExtension;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await avatar.CopyToAsync(fileStream);
                }
                var newHistoricalFigure = new HistoricalFigure
                {
                    Content = content,
                    ImageUrl= $"/src/img/{uniqueFileName}",
                    Name = name,
                    VideoUrl = urlvideo,
                };
                _context.HistoricalFigures.Add(newHistoricalFigure);
                await _context.SaveChangesAsync();
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false });
            }
        }

        public async Task<IActionResult> UpdateHistoricalFigure(int id,string name, string content, IFormFile avatar, string urlvideo)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(content) || string.IsNullOrEmpty(urlvideo))
                {
                    return Json(new { status = false });
                }
                var existingHistoricalFigure = await _context.HistoricalFigures.FindAsync(id);

                if (existingHistoricalFigure == null)
                {
                    return Json(new { status = false, message = "Không tìm thấy bản ghi." });
                }
                existingHistoricalFigure.Name = name;
                existingHistoricalFigure.Content = content;
                existingHistoricalFigure.VideoUrl = urlvideo;

                if (avatar != null && avatar.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "src", "img");
                    var fileExtension = Path.GetExtension(avatar.FileName);
                    var uniqueFileName = Guid.NewGuid() + fileExtension;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    await using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await avatar.CopyToAsync(fileStream);
                    }
                    existingHistoricalFigure.ImageUrl = $"/src/img/{uniqueFileName}";
                }
                await _context.SaveChangesAsync();
                return Json(new { status = true });
            }
            catch (Exception ex)
            {
                return Json(new { status = false });
            }

        }
    }
}
