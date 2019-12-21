using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

            Comments.Hub commentHub = new Comments.Hub();

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
                                           Date = p.CreateDate.ToStandardTimeString(),
                                           Comments = commentHub.GetComments(p.Id, 1, 5),
                                           Images = File.GetImagesPath(p.Images.SplitToInt(','))
                                       })
                                       .ToListAsync();
            return Resp.Success(pager, "");
        }

        public async Task<Resp> NewPostsAsync(Models.NewPostInfo info)
        {
            (bool isValid, string msg) = info.IsValid();
            if (!isValid)
                return Resp.Fault(Resp.NONE, msg);

            List<File> files = await File.SaveImagesAsync(info.Images);

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
