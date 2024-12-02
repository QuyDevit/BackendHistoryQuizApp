using HistoryQuizApp.DatabaseContext;
using HistoryQuizApp.Models;
using HistoryQuizApp.Models.EF;
using HistoryQuizApp.Security;
using HistoryQuizApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.IdentityModel.Tokens.Jwt;
using BcryptNet = BCrypt.Net.BCrypt;

namespace HistoryQuizApp.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly CookieAuth _cookieAuth;
        private readonly HistoryQuizAppContext _context;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(CookieAuth cookieAuth, HistoryQuizAppContext context, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _cookieAuth = cookieAuth;
            _context = context;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }   
        public IActionResult Index()
        {
            return Ok(new { Message = "API works!" });
        }
        [HttpPost("CheckLogin")]
        public async Task<IActionResult> CheckLogin([FromBody] LoginRequest request)
        {
            if (string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                return Json(new { status = false, msg = "Vui lòng nhập mật khẩu và tài khoản" });
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => (u.UserName == request.Username || u.Email == request.Username) && u.Status);

            if (user == null)
            {
                return Json(new { status = false, msg = "Sai mật khẩu hoặc tài khoản" });
            }
            if (!user.IsActive)
            {
                return Json(new { status = false, msg = "Vui lòng xác thực email!" });
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                if (!BcryptNet.Verify(request.Password, user.Password))
                {
                    return Json(new { status = false, msg = "Sai mật khẩu hoặc tài khoản" });
                }
            }
            else
            {
                return Json(new { status = false, msg = "Sai mật khẩu hoặc tài khoản" });
            }
            var acToken = _cookieAuth.GenerateToken(user);

            HttpContext.Response.Cookies.Append("accesstoken", acToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMonths(6)
            });
            return Json(new { status = true,token = acToken });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest user)
        {
            if (string.IsNullOrEmpty(user.FullName) || string.IsNullOrEmpty(user.UserName)
                || string.IsNullOrEmpty(user.Password) || string.IsNullOrEmpty(user.Email)
                || string.IsNullOrEmpty(user.School) || user.GradeId == 0)
            {
                return Json(new { status = false, msg = "Vui lòng nhập thông tin đầy đủ!" });
            }

            var checkuser = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == user.UserName || u.Email == user.Email);
            if (checkuser != null)
            {
                return Json(new { status = false, msg = "Tài khoản hoặc Email đã tồn tại!" });
            }
            var verifyToken = Guid.NewGuid().ToString();
            var domain = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}";
            var confirmationLink = $"{domain}/Account/VerifyEmail?token={verifyToken}&email={user.Email}";
            SendEmail(user.Email, "Xác thực tài khoản EduQuiz", confirmationLink);
            var newUser = new User
            {
                GradeId = user.GradeId, 
                UserName = user.UserName,
                Email = user.Email,
                FullName = user.FullName,
                Password = BcryptNet.HashPassword(user.Password),
                LinkToken = verifyToken,
                School = user.School,
                Status = true,        
                IsActive = false,
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Json(new { status = true });
        }
        [HttpPost("UpdateInfo")]
        public async Task<IActionResult> UpdateInfo(UpdateRequest user)
        {
            var getuser = await _context.Users.FindAsync(user.Id);
            if (getuser == null)
            {
                return Json(new { status = false });
            }

            getuser.School = user.School;
            getuser.FullName = user.FullName;
            getuser.Email = user.Email;
            getuser.GradeId = user.GradeId;

            _context.SaveChanges();
            return Json(new { status = true });
        }
        [HttpPost("GetInfoUser")]
        public async Task<IActionResult> GetInfoUser(UserRequest user)
        {
            var getuser = await _context.Users.FindAsync(user.Id);
            if (getuser == null)
            {
                return Json(new { status = false });
            }

            return Json(new { status = true,data = getuser });
        }
        [HttpPost("GetInfoByUser")]
        public async Task<IActionResult> GetInfoByUser()
        {
            var authCookie = Request.Cookies["accesstoken"];
            if (authCookie == null)
            {
                return Json(new { status = false });
            }
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(authCookie);
            var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
            int userid = int.Parse(userId);
            var getuser = await _context.Users.FindAsync(userid);
            if (getuser == null)
            {
                return Json(new { status = false });
            }
            return Json(new { status = true, data = getuser });
        }
        private void SendEmail(string recipientEmail, string subject, string token)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/src/template", "verificationmail.html");
            string emailBody;
            try
            {
                emailBody = System.IO.File.ReadAllText(templatePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi đọc file", ex);
            }
            
            emailBody = emailBody.Replace("{{verification_link}}", token);

            // Gửi email sử dụng dịch vụ email
            _emailService.SendEmail(recipientEmail, subject, emailBody);
        }
    }
}
