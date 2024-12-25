using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project__Mini_Stack_Overflow.Models;

namespace Project__Mini_Stack_Overflow.Controllers
{
    [Authorize]
    public class QuestionsController : Controller
    {
        private readonly QuestionDbContext _context;

        public QuestionsController(QuestionDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            // Check if the user is an admin or if they can only view their own questions
            bool isAdmin = User.IsInRole("Admin");

            IQueryable<Question> questionDbContext;

            if (isAdmin)
            {
                questionDbContext = _context.Questions.Include(q => q.User);
            }
            else
            {
                questionDbContext = _context.Questions
     .Select(q => new Question
     {
         Id = q.Id,
         Title = q.Title,
         UserId = q.UserId,
         User = q.User,
         Tags = q.Tags, 
         TrackingId = q.TrackingId,
         AnswerCount = _context.Answers.Where(a => a.QuestionId == q.Id).Count()
     });
            }
            return View(await questionDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.User)
                .Include(q => q.Answers) 
                    .ThenInclude(a => a.User) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Tags,CreatedAt,UserId")] Question question)
        {

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            question.TrackingId = currentUserId;
            _context.Add(question);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", question.UserId);
            return View(question);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", question.UserId);
            return View(question);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Tags,CreatedAt,UserId")] Question question)
        {
            if (id != question.Id)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            question.TrackingId = currentUserId;
            try
            {
                _context.Update(question);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(question.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", question.UserId);
            return View(question);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Questions == null)
            {
                return Problem("Entity set 'QuestionDbContext.Questions'  is null.");
            }
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionExists(int id)
        {
            return (_context.Questions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
