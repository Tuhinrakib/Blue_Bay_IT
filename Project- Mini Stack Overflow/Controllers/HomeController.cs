using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project__Mini_Stack_Overflow.Models;
using Project__Mini_Stack_Overflow.Models.ViewModels;
using System.Threading.Tasks;

namespace Project__Mini_Stack_Overflow.Controllers
{
    public class HomeController : Controller
    {
        private readonly QuestionDbContext _context;

        public HomeController(QuestionDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
