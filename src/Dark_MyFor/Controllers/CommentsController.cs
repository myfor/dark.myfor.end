using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Domain;

namespace Dark_MyFor.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> GetLiseAsync(int postId, int index, int rows)
        {
            

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> NewCommentsAsync([FromForm]Domain.Comments.Models.NewCommentInfo info)
        {
            (bool isValid, string msg) = info.IsValid();
            if (!isValid)
                return BadRequest(msg);

            Files file = new Files();
            List<DB.Tables.File> files = await file.SaveImagesAsync(info.Images);

            DB.Tables.Comment newComment = new DB.Tables.Comment
            {
                CreateDate = DateTimeOffset.Now,
                Creator = info.NickName,
                Content = info.Content,
                Images = string.Join(", ", files.Select(f => f.Id))
            };

            using var db = new DB.DarkContext();
            db.Comments.Add(newComment);
            int suc = await db.SaveChangesAsync();
            if (suc == 1)
                return Ok(Resp.Fault(Resp.NONE, "成功"));
            return Accepted(Resp.Fault(Resp.NONE, "提交失败"));
        }

    }
}
