using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SecretSantaApp.Models
{
    public class DesignTimeDbContextFactory// : IDesignTimeDbContextFactory<CodingBlastDbContext>
    {
        //public CodingBlastDbContext CreateDbContext(string[] args)
        //{
        //    IConfigurationRoot configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();
        //    var builder = new DbContextOptionsBuilder<CodingBlastDbContext>();
        //    var connectionString = configuration.GetConnectionString("DefaultConnection");
        //    builder.UseSqlServer(connectionString);
        //    return new CodingBlastDbContext(builder.Options);
        //}
    }
}
