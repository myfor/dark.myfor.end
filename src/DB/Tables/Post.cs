using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DB.Tables
{
    public class Post : Entity
    {
        [Required]
        public string Content { get; set; } = "";
        [Required, StringLength(64)]
        public int ImageId { get; set; } = 1;
        public File Image { get; set; }
        public List<Comment> Comments { get; set; }
    }
}
