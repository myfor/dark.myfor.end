using System;
using System.ComponentModel.DataAnnotations;

namespace Dark_MyFor.DB
{
    public class Entity
    {
        public int Id { get; set; }
        [Required]
        public int State { get; set; } = 1;
        [Required]
        public int CreateById { get; set; } = 0;
        [Required]
        public DateTimeOffset CreateDate { get; set; } = DateTimeOffset.Now;
        public int? ModifyById { get; set; } = default;
        public DateTimeOffset? ModifyDate { get; set; } = default;
    }
}
