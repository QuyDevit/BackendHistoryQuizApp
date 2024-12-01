using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HistoryQuizApp.Controllers
{
    [CustomAuthorize("Admin")]
    public class AccountController : Controller
    {
        private readonly HistoryQuizAppContext _context;
        public AccountController(HistoryQuizAppContext context)
        {
            _context = context;
        }

        [Route("manage/account")]
        public async Task< IActionResult> Index()
        {
            var listuser = await _context.Users.Where(n => n.Id != 2)
                .Include(u => u.Grade)
                .ToListAsync();
            ViewBag.ListGrade = await _context.Grades.ToListAsync();
            return View(listuser);
        }
        [AllowAnonymous]
        public ActionResult VerifyEmail(string token, string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email.Equals(email) && u.LinkToken.Equals(token));
            if (user != null)
            {
                user.IsActive = true;
                _context.SaveChanges();
                ViewBag.IsCheck = true; 
                ViewBag.Notification = "Xác thực Email thành công!";
                ViewBag.Content = "Email của bạn đã được xác minh thành công. Bây giờ bạn có thể đăng nhập vào tài khoản của mình.";
            }
            else
            {
                ViewBag.IsCheck = false; 
                ViewBag.Notification = "Xác thực Email không thành công!";
                ViewBag.Content = "Đã xảy ra lỗi khi xác minh email của bạn. Hãy đảm bảo rằng bạn đã nhấp vào đúng liên kết xác minh hoặc liên hệ với bộ phận hỗ trợ.";
            }
            return View();
        }

        public async Task<IActionResult> BLockAcount(int iduser)
        {
            var getuser = await _context.Users.FindAsync(iduser);
            if (getuser == null) {
                return Json(new { status = false });
            }
            getuser.Status = !getuser.Status;
            _context.SaveChanges();
            return Json(new { status = true });
        }

    }
}
