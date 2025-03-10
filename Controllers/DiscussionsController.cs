using AquariumForum.Data;
using AquariumForum.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AquariumForum.Controllers
{
    [Authorize]
    public class DiscussionsController : Controller
    {
        private readonly AquariumForumContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        // initializes the controller with the database context and user manager
        public DiscussionsController(
            AquariumForumContext context,
            UserManager<ApplicationUser> userManager // injecting the usermanager
        )
        {
            _context = context;
            _userManager = userManager;
        }

        // displays list of discussions by current user
        public async Task<IActionResult> Index()
        {
            var discussions = await _context
                .Discussion.Where(m => m.ApplicationUserId == _userManager.GetUserId(User))
                .Include(d => d.Comments)
                .OrderByDescending(d => d.CreateDate)
                .ToListAsync();

            return View(discussions);
        }

        // displays details of specific discussion including comments for that discussion
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // get discussion with comments and user
            var discussion = await _context
                .Discussion.Include(d => d.Comments)
                .ThenInclude(c => c.ApplicationUser)
                .Include(d => d.ApplicationUser)
                .FirstOrDefaultAsync(m => m.DiscussionId == id);

            if (discussion == null)
            {
                return NotFound();
            }

            // get id of current user
            var userId = _userManager.GetUserId(User);

            // return 403
            if (discussion.ApplicationUserId != userId)
            {
                return Forbid();
            }

            return View(discussion);
        }

        // displays form for new discussion
        public IActionResult Create()
        {
            return View();
        }

        // handles form submission for new discussion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("DiscussionId,Title,Content,ImageFile,CreateDate")] Discussion discussion
        )
        {
            // return 401 if no user
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            // assign discussion to user
            discussion.ApplicationUserId = userId;

            if (ModelState.IsValid)
            {
                // save image file
                if (discussion.ImageFile != null)
                {
                    string uniqueFileName =
                        Guid.NewGuid().ToString() + "_" + discussion.ImageFile.FileName;

                    string uploadsFolder = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        "images"
                    );

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await discussion.ImageFile.CopyToAsync(fileStream);
                    }

                    discussion.ImageFilename = uniqueFileName;
                }

                _context.Add(discussion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            return View(discussion);
        }

        // displays form for editing discussion
        public async Task<IActionResult> Edit(int? id)
        {
            // return 404 if no id
            if (id == null)
            {
                return NotFound();
            }

            // get discussion
            var discussion = await _context.Discussion.FindAsync(id);

            // return 404 if no discussion content
            if (discussion == null)
            {
                return NotFound();
            }

            // get id of current user
            var userId = _userManager.GetUserId(User);

            // if user isnt the poster, return 403
            if (discussion.ApplicationUserId != userId)
            {
                return Forbid();
            }

            return View(discussion);
        }

        // handles form submission for editing discussion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("DiscussionId,Title,Content,ImageFilename,CreateDate")] Discussion discussion
        )
        {
            // return 404 if id doesnt match
            if (id != discussion.DiscussionId)
            {
                return NotFound();
            }

            // get existing discussion
            var existingDiscussion = await _context
                .Discussion.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DiscussionId == id);

            // return 404 if no discussion
            if (existingDiscussion == null)
            {
                return NotFound();
            }

            // get id of current user
            var userId = _userManager.GetUserId(User);

            // if user isnt the poster, return 403
            if (existingDiscussion.ApplicationUserId != userId)
            {
                return Forbid();
            }

            // assign user to discussion
            if (ModelState.IsValid)
            {
                _context.Update(discussion);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(discussion);
        }

        // displays form for deleting discussion
        public async Task<IActionResult> Delete(int? id)
        {
            // return 404 if no id
            if (id == null)
            {
                return NotFound();
            }

            // get discussion
            var discussion = await _context.Discussion.FirstOrDefaultAsync(m =>
                m.DiscussionId == id
            );

            // return 404 if no discussion
            if (discussion == null)
            {
                return NotFound();
            }

            // get id of current user
            var userId = _userManager.GetUserId(User);

            // if user isnt the poster, return 403
            if (discussion.ApplicationUserId != userId)
            {
                return Forbid();
            }

            return View(discussion);
        }

        // handles form submission for deleting discussion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // get discussion
            var discussion = await _context.Discussion.FindAsync(id);

            // return 404 if no discussion
            if (discussion == null)
            {
                return NotFound();
            }

            // get id of current user
            var userId = _userManager.GetUserId(User);

            // if user isnt the poster, return 403
            if (discussion.ApplicationUserId != userId)
            {
                return Forbid();
            }

            // delete image file
            if (!string.IsNullOrEmpty(discussion.ImageFilename))
            {
                var imagePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "images",
                    discussion.ImageFilename
                );
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            // delete discussion and save
            _context.Discussion.Remove(discussion);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionExists(int id)
        {
            return _context.Discussion.Any(e => e.DiscussionId == id);
        }
    }
}
