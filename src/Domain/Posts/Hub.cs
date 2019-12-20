using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Posts
{
    public class Hub
    {

        public async Task<Resp> GetLiseAsync(int index, int rows)
        {
            Paginator pager = new Paginator
            {
                Index = index,
                Rows = rows
            };

            using var db = new DB.DarkContext();
            pager.TotalRows = await db.Posts.CountAsync();
            pager.List = await db.Posts.AsNoTracking()
                                       .Skip(pager.GetSkip())
                                       .Take(pager.Rows)
                                       .Include(p => p.Comments)
                                       .OrderByDescending(p => p.CreateDate)
                                       .Select(p => new Results.PostItem
                                       {
                                           Id = p.Id,
                                           NickName = p.Creator,
                                           Content = p.Content,
                                           Date = p.CreateDate.ToString("HH:mm"),
                                           Comments = p.Comments.OrderByDescending(c => c.CreateDate)
                                                                .Take(5)
                                                                .Select(c => new Comments.Results.CommentItem
                                                                {
                                                                    NickName = c.Creator,
                                                                    Content = c.Content,
                                                                    Date = c.CreateDate.ToString("HH:mm"),
                                                                    Images = Files.GetImagesPath(c.Images.SplitToInt(','))
                                                                })
                                                                .ToList(),
                                           Images = Files.GetImagesPath(p.Images.SplitToInt(','))
                                       })
                                       .ToListAsync();
            return Resp.Success(pager, "");
        }

        public async Task<Resp> NewPostsAsync(Models.NewPostInfo info)
        {
            (bool isValid, string msg) = info.IsValid();
            if (!isValid)
                return Resp.Fault(Resp.NONE, msg);

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
                return Resp.Fault(Resp.NONE, "成功");
            return Resp.Fault(Resp.NONE, "提交失败");
        }
    }
}
