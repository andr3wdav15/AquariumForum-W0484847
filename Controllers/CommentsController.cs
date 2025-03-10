using System.Threading.Tasks;
using AquariumForum.Data;
using AquariumForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AquariumForum.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly AquariumForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // initializes the controller with the database context and user manager
        public CommentsController(
            AquariumForumContext context,
            UserManager<ApplicationUser> userManager // injecting the usermanager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // displays form for new comment
        public async Task<IActionResult> Create(int discussionId)
        {
            // get discussion by id
            var discussion = await _context.Discussion.FindAsync(discussionId);

            // return 404
            if (discussion == null)
            {
                return NotFound();
            }

            // create new comment
            var comment = new Comment { DiscussionId = discussionId };

            return View(comment);
        }

        // handles post request for new comment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comment comment)
        {
            // get discussion by id
            var discussion = await _context.Discussion.FindAsync(comment.DiscussionId);

            // return 404
            if (discussion == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // get id of current user
                var userId = _userManager.GetUserId(User);

                // return 401 if user not authenticated
                if (userId == null)
                {
                    return Unauthorized();
                }

                // set comment properties
                comment.ApplicationUserId = userId;
                comment.CreateDate = DateTime.Now;

                // add comment and save changes
                _context.Add(comment);
                await _context.SaveChangesAsync();

                // redirect to discussion
                return RedirectToAction("GetDiscussion", "Home", new { id = comment.DiscussionId });
            }

            return View(comment);
        }
    }
}
