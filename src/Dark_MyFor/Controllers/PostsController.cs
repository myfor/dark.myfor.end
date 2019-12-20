using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dark_MyFor.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : ControllerBase
    {

        [HttpGet]
        public async Task<ActionResult> GetLiseAsync(int index, int rows)
        {
            Paginator pager = new Paginator
            { 
                Index = index,
                Rows = rows
            };

            using var db = new DB.DarkContext();
            pager.TotalRows = await db.Posts.CountAsync();
            List<Domain.Posts.Results.PostItem> list = await db.Posts.AsNoTracking()
                                       .Skip(pager.GetSkip())
                                       .Take(pager.Rows)
                                       .Include(p => p.Comments)
                                       .OrderByDescending(p => p.CreateDate)
                                       .Select(p => new Domain.Posts.Results.PostItem
                                       { 
                                           Id = p.Id,
                                           NickName = p.Creator,
                                           Content = p.Content,
                                           Date = p.CreateDate.ToString("yyyy-MM-dd HH:mm"),
                                           Comments = p.Comments.OrderByDescending(c => c.CreateDate)
                                                                .Take(5)
                                                                .Select(c => new Domain.Comments.Results.CommentItem
                                                                { 
                                                                    NickName = c.Creator,
                                                                    Content = c.Content,
                                                                    Date = c.CreateDate.ToString("yyyy-MM-dd HH:mm")
                                                                })
                                                                .ToList()
                                       })
                                       .ToListAsync();

            return Ok(pager);
        }

        [HttpPost]
        public async Task<ActionResult> NewPostsAsync([FromForm]Domain.Posts.Models.NewPostInfo info)
        {
            (bool isValid, string msg) = info.IsValid();
            if (!isValid)
                return BadRequest(msg);

            Files file = new Files();
            List<DB.Tables.File> files = await file.SaveImagesAsync(info.Images);

            DB.Tables.Post newPost = new DB.Tables.Post
            {
                CreateDate = DateTimeOffset.Now,
                Creator = info.NickName,
                Content = info.Content,
                Images = string.Join(", ", files.Select(f => f.Id))
            };
            using var db = new DB.DarkContext();
            db.Posts.Add(newPost);
            int suc = await db.SaveChangesAsync();
            if (suc == 1)
                return Ok(Resp.Fault(Resp.NONE, "成功"));
            return Accepted(Resp.Fault(Resp.NONE, "提交失败"));
        }

    }
}
