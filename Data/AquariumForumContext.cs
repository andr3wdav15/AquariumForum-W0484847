using AquariumForum.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AquariumForum.Data
{
    public class AquariumForumContext : IdentityDbContext<ApplicationUser>
    {
        public AquariumForumContext(DbContextOptions<AquariumForumContext> options)
            : base(options) { }

        public DbSet<Comment> Comment { get; set; } = default!;
        public DbSet<Discussion> Discussion { get; set; } = default!;
    }
}
