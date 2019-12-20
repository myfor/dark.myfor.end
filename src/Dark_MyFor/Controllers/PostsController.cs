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
