using AquariumForum.Data;
using AquariumForum.Models;
using Microsoft.AspNetCore.Mvc;

namespace AquariumForum.Controllers
{
    public class CommentsController : Controller
    {
        private readonly AquariumForumContext _context;

        public CommentsController(AquariumForumContext context) // initialize with database context
        {
            _context = context;
        }

        public async Task<IActionResult> Create(int discussionId) // displays form for new comment
        {
            var discussion = await _context.Discussion.FindAsync(discussionId);
            if (discussion == null)
            {
                return NotFound();
            }

            ViewBag.DiscussionId = discussionId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content, int discussionId) // handles form submission
        {
            var discussion = await _context.Discussion.FindAsync(discussionId);
            if (discussion == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var comment = new Comment
                {
                    Content = content,
                    DiscussionId = discussionId,
                    CreateDate = DateTime.Now,
                };

                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("GetDiscussion", "Home", new { id = discussionId });
            }

            ViewBag.DiscussionId = discussionId;
            return View();
        }
    }
}
