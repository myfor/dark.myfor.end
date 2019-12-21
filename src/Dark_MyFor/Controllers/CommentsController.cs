using Dark_MyFor.Share;
using Domain;
using Domain.Comments;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dark_MyFor.Controllers
{
    [Route("api/[controller]")]
    public class CommentsController : DarkBaseController
    {
        [HttpGet]
        public async Task<ActionResult> GetLiseAsync(int postId, int index, int rows)
        {
            Hub hub = new Hub();
            Resp r = await hub.GetLiseAsync(postId, index, rows);
            return Pack(r);
        }

        [HttpPost]
        public async Task<ActionResult> NewCommentsAsync([FromForm]Models.NewCommentInfo info)
        {
            if (info.Images is null)
                info.Images = new List<IFormFile>();

            Hub hub = new Hub();
            Resp r = await hub.NewCommentsAsync(info);
            return Pack(r);
        }

    }
}
