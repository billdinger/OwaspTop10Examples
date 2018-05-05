using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Controllers
{
    public class BlogEntriesController : Controller
    {
        private readonly BlogEntryContext _context;

        public BlogEntriesController(BlogEntryContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            return View(await _context.BlogEntry.ToListAsync());
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogEntry = await _context.BlogEntry
                .SingleOrDefaultAsync(m => m.Id == id);
            if (blogEntry == null)
            {
                return NotFound();
            }

            return View(blogEntry);
        }


        public IActionResult Create()
        {
            return View();
        }


        /// <summary>
        /// A5 - Broken access control - This entry should be decorated with an Authorize attribute so that only logged
        /// in users cant submit blog entries.
        /// </summary>
        /// <param name="blogEntry">The blog entry to create</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Body,Author,Title,Published")] BlogEntry blogEntry)
        {
            if (ModelState.IsValid)
            {
                _context.Add(blogEntry);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(blogEntry);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogEntry = await _context.BlogEntry.SingleOrDefaultAsync(m => m.Id == id);
            if (blogEntry == null)
            {
                return NotFound();
            }
            return View(blogEntry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body,Author,Title,Published")] BlogEntry blogEntry)
        {
            if (id != blogEntry.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogEntry);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogEntryExists(blogEntry.Id))
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
            return View(blogEntry);
        }

        // GET: BlogEntries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogEntry = await _context.BlogEntry
                .SingleOrDefaultAsync(m => m.Id == id);
            if (blogEntry == null)
            {
                return NotFound();
            }

            return View(blogEntry);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blogEntry = await _context.BlogEntry.SingleOrDefaultAsync(m => m.Id == id);
            _context.BlogEntry.Remove(blogEntry);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool BlogEntryExists(int id)
        {
            return _context.BlogEntry.Any(e => e.Id == id);
        }
    }
}
