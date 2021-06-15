using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EduHome.Models
{
    public class BlogDetail
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string LeaveReply { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("Blog")]
        public int BlogId { get; set; }

        public Blog Blog { get; set; }
    }
}
