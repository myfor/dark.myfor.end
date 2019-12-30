using Dark_MyFor.Share;
using Domain;
using Domain.Posts;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Dark_MyFor.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : DarkBaseController
    {
#if DEBUG
        [HttpGet("test")]
        public ActionResult Test()
        {
            string s1 = $"DELETE FROM {nameof(DB.DarkContext.Comments)}";
            string s2 = $"DELETE FROM {nameof(DB.DarkContext.Posts)}";
            string s3 = $"DELETE FROM {nameof(DB.DarkContext.Files)}";
            return Ok(s1 + s2 + s3);
        }
#endif

        [HttpGet]
        public async Task<ActionResult> GetLiseAsync(int index, int rows)
        {
            Hub hub = new Hub();
            Resp r = await hub.GetLiseAsync(index, rows);

            return Pack(r);
        }

        [HttpPost]
        public async Task<ActionResult> NewPostsAsync([FromForm]Models.NewPostInfo info)
        {
            Hub hub = new Hub();
            Resp r = await hub.NewPostsAsync(info);

            return Pack(r);
        }

    }
}
