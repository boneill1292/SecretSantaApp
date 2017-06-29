using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SecretSantaApp.Models
{
  public class AppDbContext : IdentityDbContext<IdentityUser>
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }

    public DbSet<Group> Groups { get; set; }
    public DbSet<CustomUser> CustomUsers { get; set; }
    public DbSet<GroupMemberDetails> GroupMemberDetails { get; set; }
  }
}
