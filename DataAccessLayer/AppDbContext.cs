using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Models;
using Microsoft.EntityFrameworkCore;

namespace EduHome.DataAccessLayer
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bio> Bios { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<About> About { get; set; }
    }
}
