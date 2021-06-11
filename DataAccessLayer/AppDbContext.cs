using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EduHome.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EduHome.DataAccessLayer
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Bio> Bios { get; set; }

        public DbSet<Contact> Contacts { get; set; }

        public DbSet<Slider> Sliders { get; set; }

        public DbSet<About> About { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<CourseDetail> CourseDetails { get; set; }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<BlogDetail> BlogDetails { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventDetail> EventDetails { get; set; }

        public DbSet<Speaker> Speakers { get; set; }

        public DbSet<EventSpeaker> EventSpeakers { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<TeacherDetail> TeacherDetails { get; set; }

        public DbSet<Profession> Professions { get; set; }

        public DbSet<TeacherProfession> TeacherProfessions { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<TeacherContactInfo> TeacherContactInfos { get; set; }

        public DbSet<SocialMedia> SocialMedias { get; set; }
    }
}
