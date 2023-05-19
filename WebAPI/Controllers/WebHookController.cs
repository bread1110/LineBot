using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using WebAPI.Models;
using static WebAPI.Models.LineBot;

namespace YourWebApiProject.Controllers
{
    public class LineWebhookController : ApiController
    {

        LineBot m_LineBot = new LineBot();

        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            RecvByBackMessage_Reply ReplyObj = default(RecvByBackMessage_Reply);
            Record recordObj = new Record();
            // Read request content
            string jsonContent = await Request.Content.ReadAsStringAsync();



            // Strore message from line message api(JSON string)
            //await recordObj.StoreJsonAsync(jsonContent, "WebhookRecord.txt");

            // Validate signature
            if (!await ValidateSignatureAsync(Request.Headers, jsonContent))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            // Deserialize JSON
            var Obj = JsonConvert.DeserializeObject<LineWebHook>(jsonContent);

            // Handle events
            foreach (var lineEvent in Obj.events)
            {
                // Handle different event types here
                if (default != lineEvent.message && "text" == lineEvent.message.type)
                {
                    if (default != (ReplyObj = m_LineBot.FormatData(lineEvent)))
                    {
                        #region Strore reply to line message api(JSON string)

                        //string content = JsonConvert.SerializeObject(ReplyObj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                        //await recordObj.StoreJsonAsync(content, "RelpyRecord.txt");

                        #endregion

                        await m_LineBot.Reply(ReplyObj);
                    }
                }
            }

            // Return success response
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task<bool> ValidateSignatureAsync(System.Net.Http.Headers.HttpRequestHeaders headers, string content)
        {
            // 暫不檢驗
            return true;
            // Implement signature validation here
            // Return true if the signature is valid, false otherwise
            try
            {
                var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(m_LineBot.strGetChannelSecret()));
                var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(await Request.Content.ReadAsStringAsync()));
                var contentHash = Convert.ToBase64String(computeHash);
                var headerHash = Request.Headers.GetValues("X-Line-Signature").First();

                return contentHash == headerHash;
            }
            catch
            {
                return false;
            }
        }
    }
}
