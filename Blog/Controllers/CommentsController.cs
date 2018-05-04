using System.Linq;
using System.Threading.Tasks;
using Audit.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Microsoft.Extensions.Logging;

namespace Blog.Controllers
{
    public class CommentsController : Controller
    {
        private readonly CommentContext _context;
        private readonly ILogger _logger;

        public CommentsController(CommentContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Comment.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .SingleOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// A1 - Injection - This is an example of a SQL Injection attack! OWASP #1 - DO NOT
        /// EVER USE THIS IN PRODUCTION CODE. In this case any arbitrary values entered
        /// by our user in the Body or Author fields on our form will get posted ot SQL
        /// Exposing us to not only SQL Injection by Stored XSS.
        /// </summary>
        /// <param name="comment">A comment</param>
        /// <returns>Redirects user to action.</returns>
        [HttpPost]
        public async Task<IActionResult> Create([Bind("Id,Body,Author")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                await _context.Database
                    .ExecuteSqlCommandAsync
                    ($"insert into comments Id,Body, Author Values ('{comment.Id}','{comment.Body}',''{comment.Author}");

                // Correct way to add a user that's not vulnerable to SQL Injection:
                //_context.Add(comment);
                //await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(comment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment);
        }

        /// <summary>
        /// A10 - Insufficient logging & Auditing. This uses the Audit.net framework for more comprehensive logging of the
        /// action being taken.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,Author")] Comment comment)
        {
            using (AuditScope.Create("Edit", () => comment))
            {
                if (id != comment.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(comment);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!CommentExists(comment.Id))
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
                return View(comment);
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .SingleOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        /// <summary>
        /// A10 - Insufficent Logging & Monitoring - this method properly logs and audits all actions taken on it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogInformation($"Deleting the comment id {0}", id);
            var comment = await _context.Comment.SingleOrDefaultAsync(m => m.Id == id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool CommentExists(int id)
        {
            return _context.Comment.Any(e => e.Id == id);
        }
    }
}
