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
    public class VotesController : Controller
    {
        private readonly QuestionDbContext _context;

        public VotesController(QuestionDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var questionDbContext = _context.Votes.Include(v => v.Answer).Include(v => v.Question).Include(v => v.User);
            return View(await questionDbContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Votes == null)
            {
                return NotFound();
            }

            var vote = await _context.Votes
                .Include(v => v.Answer)
                .Include(v => v.Question)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vote == null)
            {
                return NotFound();
            }

            return View(vote);
        }
        public IActionResult Create()
        {
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,IsUpvote,UserId,QuestionId,AnswerId")] Vote vote)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", vote.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", vote.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", vote.UserId);
            return View(vote);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Votes == null)
            {
                return NotFound();
            }

            var vote = await _context.Votes.FindAsync(id);
            if (vote == null)
            {
                return NotFound();
            }
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", vote.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", vote.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", vote.UserId);
            return View(vote);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IsUpvote,UserId,QuestionId,AnswerId")] Vote vote)
        {
            if (id != vote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VoteExists(vote.Id))
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
            ViewData["AnswerId"] = new SelectList(_context.Answers, "Id", "Id", vote.AnswerId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", vote.QuestionId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Email", vote.UserId);
            return View(vote);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Votes == null)
            {
                return NotFound();
            }

            var vote = await _context.Votes
                .Include(v => v.Answer)
                .Include(v => v.Question)
                .Include(v => v.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vote == null)
            {
                return NotFound();
            }

            return View(vote);
        }
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Votes == null)
            {
                return Problem("Entity set 'QuestionDbContext.Votes'  is null.");
            }
            var vote = await _context.Votes.FindAsync(id);
            if (vote != null)
            {
                _context.Votes.Remove(vote);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool VoteExists(int id)
        {
          return (_context.Votes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
