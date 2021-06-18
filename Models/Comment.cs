using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class Comment
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreationDate { get; set; }

        public int BlogId { get; set; }

        public Blog Blog { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }
    }
}
