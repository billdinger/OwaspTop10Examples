using System;
using System.Linq;
using System.Threading.Tasks;
using Blog.Cryptography;
using Blog.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Microsoft.Extensions.Logging;

namespace Blog.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserContext _context;

        private readonly ICryptographicService _cryptographicServices;

        private readonly ILogger<UsersController> _logger;

        public UsersController(UserContext context,
            ICryptographicService cryptographicServices,
            ILogger<UsersController> logger)
        {
            _context = context;
            _cryptographicServices = cryptographicServices;
            _logger = logger;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.User.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.Id == id);
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


        /// <summary>
        /// A3 - Sensitive data exposure - this method calls ToString() on user which outputs the user's password, 
        /// exposing a critical part of user data to the log files.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Username,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _logger.LogInformation($"Creating the user {user.ToString()}");
                user.Salt = Guid.NewGuid().ToString("N");
                user.Password = _cryptographicServices.HashPassword(user.Password, user.Salt);

                // add to our database.
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(user);
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        /// <summary>
        /// A8RC - Cross Site Request Forgery - This method is vulnerable to CSRF as it's missing a [ValidateAntiForgeryToken] attribute.
        /// This was considered for the top 10 as part of the RC but was removed from the final version as it was felt the prevelance
        /// of Anti CSRF frameworks, like ASP.NETs, removed the need for it. Preserved here for historical purposes.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Username,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(user);
        }

        /// <summary>
        /// A7RC - Insufficient Attack Protection - This method properly tracks bad requests and rejects 
        /// them once they have reached too many requests. This top 10 was considered for the Release Candidate but was left
        /// off the final list, preserved here for historical purposes.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [BadRequestFilter(MaxAttemptsAllowed = 100)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .SingleOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }


        /// <summary>
        /// A10 - Insufficient Logging & Monitoring - This action lets us delete a user without any sort of logging or monitoring
        /// that it's occuring. The action simply occurs without anyone knowing about it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }


    }
}
