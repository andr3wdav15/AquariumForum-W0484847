using Microsoft.EntityFrameworkCore;

namespace AquariumForum.Data
{
    public class AquariumForumContext : DbContext
    {
        public AquariumForumContext(DbContextOptions<AquariumForumContext> options)
            : base(options) { }

        public DbSet<AquariumForum.Models.Comment> Comment { get; set; } = default!;
        public DbSet<AquariumForum.Models.Discussion> Discussion { get; set; } = default!;
    }
}
