using System.ComponentModel.DataAnnotations;

namespace DB.Tables
{
    public class Comment : Entity
    {
        /// <summary>
        /// 所属帖子
        /// </summary>
        [Required]
        public int PostId { get; set; }
        /// <summary>
        /// 所属帖子
        /// </summary>
        public Post Post { get; set; }
        /// <summary>
        /// 评论
        /// </summary>
        [Required, StringLength(512)]
        public string Content { get; set; } = "";
        [Required, StringLength(64)]
        public string Images { get; set; } = "";
    }
}
