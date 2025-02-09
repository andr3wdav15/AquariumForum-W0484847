using AquariumForum.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquariumForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly AquariumForumContext _context;

        public HomeController(AquariumForumContext context) // initializes with database context
        {
            _context = context;
        }

        public async Task<IActionResult> Index() // displays list of discussions, descending order
        {
            var discussions = await _context
                .Discussion.Include(d => d.Comments)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View(discussions);
        }

        public async Task<IActionResult> GetDiscussion(int? id) // displays details of specific discussion including comments for that discussion
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussion = await _context
                .Discussion.Include(d => d.Comments!.OrderByDescending(c => c.CreateDate))
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        public IActionResult Privacy() // displays privacy policy
        {
            return View();
        }
    }
}
