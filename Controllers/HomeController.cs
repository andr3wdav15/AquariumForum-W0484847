using AquariumForum.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquariumForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly AquariumForumContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // initializes with database context
        public HomeController(
            AquariumForumContext context,
            SignInManager<ApplicationUser> signInManager // injecting for button
        )
        {
            _context = context;
            _signInManager = signInManager;
        }

        // displays list of discussions
        public async Task<IActionResult> Index()
        {
            // get discussions with comments and user
            var discussions = await _context
                .Discussion.Include(d => d.Comments.OrderByDescending(c => c.CreateDate))
                .Include(d => d.ApplicationUser)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View(discussions);
        }

        // displays details of specific discussion including comments for that discussion
        public async Task<IActionResult> GetDiscussion(int? id)
        {
            // return 404
            if (id == null)
            {
                return NotFound();
            }

            // get discussion with comments and user by id
            var discussion = await _context
                .Discussion.Include(d => d.Comments.OrderByDescending(c => c.CreateDate))
                .ThenInclude(c => c.ApplicationUser) // user for comment
                .Include(d => d.ApplicationUser) // user for discussion
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            // return 404
            if (discussion == null)
            {
                return NotFound();
            }

            return View(discussion);
        }

        // displays profile details for the given user
        public async Task<IActionResult> Profile(string id)
        {
            // return 404
            if (id == null)
            {
                return NotFound();
            }

            // get user by id
            var user = await _context.Users.FindAsync(id);

            // return 404
            if (user == null)
            {
                return NotFound();
            }

            // get discussions with comments and user by user id
            var discussions = await _context
                .Discussion.Where(d => d.ApplicationUserId == id)
                .Include(d => d.Comments)
                .Include(d => d.ApplicationUser)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            // pass user to view
            ViewBag.User = user;
            return View(discussions);
        }

        // displays privacy policy
        public IActionResult Privacy()
        {
            return View();
        }
    }
}
