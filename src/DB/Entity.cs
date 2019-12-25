using System;
using System.ComponentModel.DataAnnotations;

namespace DB
{
    public class Entity
    {
        public int Id { get; set; }
        [Required]
        public int State { get; set; } = 1;
        [Required, StringLength(64)]
        public string Creator { get; set; } = "";
        [Required]
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
    }
}
