using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JFGM20250320.AppWebMVC.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JFGM20250320.AppWebMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly Test20250320DbContext _context;

        public UsersController(Test20250320DbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Username,Email,PasswordHash,paswordConfir,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.PasswordHash = CalcularHashMD5(user.PasswordHash);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,Username,Email,Role")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            var userUpdate = await _context.Users.FirstOrDefaultAsync(m => m.UserId == user.UserId);
            try
            {
                userUpdate.Username = user.Username;
                userUpdate.Email = user.Email;
                userUpdate.Role = user.Role;
                _context.Update(userUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }



        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        public async Task<IActionResult> CerrarSession()
        {
 
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            user.PasswordHash = CalcularHashMD5(user.PasswordHash);
            var usuarioAuth = await _context.
                Users.
                FirstOrDefaultAsync(s => s.Email == user.Email && s.PasswordHash == user.PasswordHash);
            if (usuarioAuth != null && usuarioAuth.UserId > 0 && usuarioAuth.Email == user.Email)
            {
                var claims = new[] {
        new Claim(ClaimTypes.Name, usuarioAuth.Email),
        new Claim("UserId", usuarioAuth.UserId.ToString()),
         new Claim("Username", usuarioAuth.Username),
        new Claim(ClaimTypes.Role, usuarioAuth.Role)
        };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "El email o contraseña estan incorrectos");
                return View();
            }
        }

        public async Task<IActionResult> Perfil()
        {

            var idStr = User.FindFirst("UserId")?.Value;
            int id = int.Parse(idStr);
            var user = await _context.Users.FindAsync(id);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(int id, [Bind("UserId,Username,Email,PasswordHash,paswordConfir,Role")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            var userUpdate = await _context.Users.FirstOrDefaultAsync(m => m.UserId == user.UserId);
            try
            {
                userUpdate.Username = user.Username;
                userUpdate.Email = user.Email;
                userUpdate.PasswordHash = CalcularHashMD5(user.PasswordHash);
                userUpdate.Role = user.Role;
                _context.Update(userUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                {
                    return NotFound();
                }
                else
                {
                    return View(user);
                }
            }
        }

        private string CalcularHashMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); 
                }
                return sb.ToString();
            }
        }


    }
}
