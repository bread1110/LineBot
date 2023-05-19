using System.Web.Http;
using static WebAPI.Models.LineBot;
using WebAPI.Models;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using System.Net;

namespace WebAPI.Controllers
{
    public class WebPushController : ApiController
    {
        /// <summary>
        /// 主動推播訊息
        /// </summary>
        /// <param name="pushMessage">HTTP POST中 Body的Json字串轉成之物件</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task PostAsync([FromBody] Send2BackMessage_Push pushMessage)
        {
            var lineBot = new LineBot();
            var json = JsonConvert.SerializeObject(pushMessage);
            await lineBot.PushMessageAsync(json);
        }
    }
}