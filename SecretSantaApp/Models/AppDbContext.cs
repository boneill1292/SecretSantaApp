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

    public DbSet<Group> Group { get; set; }
    //public DbSet<Pie> Pies { get; set; }
    //public DbSet<Category> Categories { get; set; }
    //public DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }
    //public DbSet<Order> Orders { get; set; }
    //public DbSet<OrderDetail> OrderDetails { get; set; }
  }
}
