using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;
using System.Text;

namespace ChurrascoChallenge.Controllers
{
    public class AccountController : Controller
    {
        private readonly MySqlConnection _dbConnection;
        private readonly ILogger<AccountController> _logger;

        public AccountController(MySqlConnection dbConnection, ILogger<AccountController> logger)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string usernameOrEmail, string password)
        {
            try
            {
                string passwordHash = ComputeSha256Hash(password);

                string query = @"SELECT * FROM user WHERE (username = @UsernameOrEmail OR email = @UsernameOrEmail) 
                             AND password = @passwordHash AND role = 'admin' AND active = true";

                using (var cmd = new MySqlCommand(query, _dbConnection))
                {
                    cmd.Parameters.AddWithValue("@UsernameOrEmail", usernameOrEmail);
                    cmd.Parameters.AddWithValue("@passwordHash", passwordHash);
                    await _dbConnection.OpenAsync();

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            HttpContext.Session.SetString("User", usernameOrEmail);
                            return RedirectToAction("Index", "Products");
                        }
                    }
                }

                ModelState.AddModelError("", "ERROR");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR");
                ModelState.AddModelError("", "ERROR");
                return View();
            }
            finally
            {
                await _dbConnection.CloseAsync();
            }
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
