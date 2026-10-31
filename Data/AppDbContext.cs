using Asp_net_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Asp_net_auth.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options):DbContext(options)
    {
        public DbSet<User> user { get; set; }
    }
}
