using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Comments
{
    public class Hub
    {
        public async Task<Resp> GetLiseAsync(int postId, int index, int rows)
        {
            Paginator pager = new Paginator
            { 
                Index = index,
                Rows = rows
            };

            using var db = new DB.DarkContext();

            pager.TotalRows = await db.Comments.CountAsync(c => c.PostId == postId);
            pager.List = await db.Comments.AsNoTracking()
                                          .Where(c => c.PostId == postId)
                                          .Skip(pager.GetSkip())
                                          .Take(pager.Rows)
                                          .Select(c => new Results.CommentItem
                                          {
                                              NickName = c.Creator,
                                              Content = c.Content,
                                              Date = c.CreateDate.ToString("HH:mm"),
                                              Images = Files.GetImagesPath(c.Images.SplitToInt(','))
                                          })
                                          .ToListAsync();
            return Resp.Success(pager);
        }

        public async Task<Resp> NewCommentsAsync(Models.NewCommentInfo info)
        {
            (bool isValid, string msg) = info.IsValid();
            if (!isValid)
                return Resp.Fault(Resp.NONE, msg);

            Files file = new Files();
            List<DB.Tables.File> files = await file.SaveImagesAsync(info.Images);

            DB.Tables.Comment newComment = new DB.Tables.Comment
            {
                PostId = info.PostId,
                CreateDate = DateTimeOffset.Now,
                Creator = info.NickName,
                Content = info.Content,
                Images = string.Join(", ", files.Select(f => f.Id))
            };

            using var db = new DB.DarkContext();
            db.Comments.Add(newComment);
            int suc = await db.SaveChangesAsync();
            if (suc == 1)
                return Resp.Fault(Resp.NONE, "成功");
            return Resp.Fault(Resp.NONE, "提交失败");
        }
    }
}
