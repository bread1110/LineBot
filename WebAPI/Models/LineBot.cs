using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;
using static WebAPI.Models.LineBot.LineWebHook;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Models
{
    public class LineBot
    {
        private readonly string m_channelAccessToken = default;
        private readonly HttpClient m_httpClient = default;
        private readonly string m_ChannelSecret = default;

        public LineBot()
        {
            m_ChannelSecret = "4fcb9a6e43b5fdc3a2a4674d801842a6";
            m_channelAccessToken = "KWDBFflMsxpuGefemywxjAoRbuSaZ/C1QORz+cOViSGiB1qwTK9rNOJd519uw1Mc8YTTpYjWEdd31QLv//BDuO4uHt8YiuYX8TRSNlpSLc4XsFLON/P/5vC2yxBVnDU9eL2UENERaUvzsvEll6fNawdB04t89/1O/w1cDnyilFU=";
            m_httpClient = new HttpClient();
            m_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", m_channelAccessToken);
        }

        public RecvByBackMessage_Reply FormatData(Event eventObj)
        {
            if (default == eventObj ||
                default == eventObj.message ||
                string.IsNullOrWhiteSpace(eventObj.message.text) || // 空白不處理
                string.IsNullOrWhiteSpace(eventObj.replyToken))  // 無replyToken視為異常
                return default;

            RecvByBackMessage_Reply replyObj = new RecvByBackMessage_Reply();

            // 去除前後空白
            string Message = eventObj.message.text.Trim(' ');

            replyObj.replyToken = eventObj.replyToken;
            switch(Message)
            {
                case "登記":
                    {
                        replyObj.messages.Add(new RecvByBackMessage_Reply.Message()
                        {
                            type="text",
                            text = "https://forms.gle/7w7THCK2MVeuQUEd7"
                        });
                        break;
                    }
                case "裝備":
                    {
                        replyObj.messages.Add(new RecvByBackMessage_Reply.Message()
                        {
                            type="text",
                            text = "https://docs.google.com/spreadsheets/d/1rz5HdCMSEhZaF5WyRjQdZAHYGRmKkUuk-_rIOQZbbXU/edit#gid=629249301"
                        });
                        break;
                    }


                default: break;
            }

            return replyObj;
        }

        public string strGetChannelSecret()
        {
            return m_ChannelSecret;
        }

        /// <summary>
        /// 主動推播訊息
        /// </summary>
        /// <param name="strPushMessage">Line Json格式訊息</param>
        /// <returns></returns>
        public async Task PushMessageAsync(string strPushMessage)
        {
            var content = new StringContent(strPushMessage, Encoding.UTF8, "application/json");

            var response = await m_httpClient.PostAsync("https://api.line.me/v2/bot/message/push", content);
        }

        /// <summary>
        /// 回覆用戶訊息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Reply(RecvByBackMessage_Reply message)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", m_channelAccessToken);

            var content = JsonConvert.SerializeObject(message, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("https://api.line.me/v2/bot/message/reply"),
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
            var response = await client.SendAsync(requestMessage);
        }

        public class Send2BackMessage_Push
        {
            public string to { get; set; }
            public List<Send2BackMessage> messages { get; set; }
            public class Send2BackMessage
            {
                public string type { get; set; }
                public string text { get; set; }
            }
        }

        public class RecvByBackMessage_Reply
        {
            public string replyToken { get; set; }
            public List<Message> messages { get; set; } = new List<Message>();
            public class Message
            {
                public string type { get; set; }
                public string text { get; set; }
                public string originalContentUrl { get; set; }
                public string previewImageUrl { get; set; }
                public int? duration { get; set; }
                public string title { get; set; }
                public string address { get; set; }
                public double? latitude { get; set; }
                public double? longitude { get; set; }
                public string packageId { get; set; }
                public string stickerId { get; set; }
                public string altText { get; set; }
                public Template template { get; set; }
                public class Template
                {
                    public string type { get; set; }
                    public List<Column> columns { get; set; }
                    public class Column
                    {
                        public string thumbnailImageUrl { get; set; }
                        public string title { get; set; }
                        public string text { get; set; }
                        public List<Action> actions { get; set; }
                        public class Action
                        {
                            public string type { get; set; }
                            public string label { get; set; }
                            public string data { get; set; }
                            public string uri { get; set; }
                        }

                    }
                }
            }
        }

        public class LineWebHook
        {
            public string destination { get; set; }
            public List<Event> events { get; set; }

            public class Event
            {
                public string type { get; set; }
                public Message message { get; set; }
                public string webhookEventId { get; set; }
                public DeliveryContext deliveryContext { get; set; }
                public long timestamp { get; set; }
                public Source source { get; set; }
                public string replyToken { get; set; }
                public string mode { get; set; }

                public class DeliveryContext
                {
                    public bool isRedelivery { get; set; }
                }

                public class Message
                {
                    public string type { get; set; }
                    public string id { get; set; }
                    public string text { get; set; }
                }

                public class Source
                {
                    public string type { get; set; }
                    public string userId { get; set; }
                }
            }

        }

        public class LineBotInfo
        {
            public string token = string.Empty;



        }

        public class Linage2M_BossInfo
        {
            /// <summary>預計下一次出王時間</summary>
            public DateTime NextBornTime { get; set; } = default(DateTime);

            /// <summary>Boss名稱</summary>
            public string BossName { get; set; } = string.Empty;

            /// <summary>重生間隔</summary>
            public TimeSpan After { get; set; }

            /// <summary>重生機率</summary>
            public decimal Probability { get; set; }
        }
        /// 20230507 進度:
        /// 完成串接LINE Message API 但尚未串 Notify API
        /// 尚未訂定JSON格式 目前尚缺:王表、整體儲存
        /// 尚未確認重生時間與時間間隔應該分別用什麼格式比較好
    }

}