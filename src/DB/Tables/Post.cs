using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB.Tables
{
    public class Post : Entity
    {
        [Required]
        public string Content { get; set; } = "";
        [Required, StringLength(64)]
        public string Images { get; set; } = "";
        public List<Comment> Comments { get; set; }
    }
}
