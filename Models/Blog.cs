using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EduHome.Models
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Image { get; set; }

        [Required]
        public string Author { get; set; }

        public int CommentCount { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastModification { get; set; }

        public bool IsDeleted { get; set; }

        public BlogDetail BlogDetail { get; set; }

        public ICollection<CategoryBlog> CategoryBlogs { get; set; }

        public ICollection<Comment> Comments { get; set; }

        [NotMapped]
        public IFormFile Photo { get; set; }
    }
}
