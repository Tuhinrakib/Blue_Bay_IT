using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Project__Mini_Stack_Overflow.Models;

namespace Project__Mini_Stack_Overflow.Controllers
{
    [Authorize]
    public class AnswersController : Controller
    {
        private readonly QuestionDbContext _context;

        public AnswersController(QuestionDbContext context)
        {
            _context = context;
        }

        // GET: Answers
        public async Task<IActionResult> Index()
        {
            var answers = _context.Answers.Include(a => a.Question).Include(a => a.User);
            return View(await answers.ToListAsync());
        }

        // GET: Answers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers
                .Include(a => a.Question)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // GET: Answers/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }

        // POST: Answers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AnswerText,CreatedAt,UserId,QuestionId")] Answer answer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(answer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", answer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Create/{questionId}
        public IActionResult CreateForQuestion(int questionId)
        {
            var question = _context.Questions
                                  .Include(q => q.Answers)
                                  .FirstOrDefault(q => q.Id == questionId);
            if (question == null)
            {
                return NotFound();
            }

            ViewData["QuestionTitle"] = question.Title;
            ViewData["QuestionId"] = question.Id;
            return View();
        }

        // GET: Answers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", answer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", answer.UserId);
            return View(answer);
        }

        // POST: Answers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnswerText,CreatedAt,UserId,QuestionId")] Answer answer)
        {
            if (id != answer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerExists(answer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Title", answer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", answer.UserId);
            return View(answer);
        }

        // GET: Answers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Answers == null)
            {
                return NotFound();
            }

            var answer = await _context.Answers
                .Include(a => a.Question)
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (answer == null)
            {
                return NotFound();
            }

            return View(answer);
        }

        // POST: Answers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Answers == null)
            {
                return Problem("Entity set 'QuestionDbContext.Answers'  is null.");
            }

            var answer = await _context.Answers.FindAsync(id);
            if (answer != null)
            {
                _context.Answers.Remove(answer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnswerExists(int id)
        {
            return (_context.Answers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
