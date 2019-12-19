using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using System.ComponentModel;
using System.Net.Http;
using Newtonsoft.Json;

namespace Botpress
{
    public class SendMessage : CodeActivity
    {
        private class ResponsePayload
        {
            public string text { get; set; }
            public object message { get; set; }
            public string channel { get; set; }
            public string target { get; set; }
            public string botId { get; set; }
        }

        private static readonly HttpClient client = new HttpClient();

        [Category("Connection")]
        [RequiredArgument]
        public InArgument<string> ExternalURL { get; set; }

        [Category("Message")]
        [RequiredArgument]
        public InArgument<object> Message { get; set; }

        [Category("Response")]
        [RequiredArgument]
        public InArgument<string> BotpressToken { get; set; }

        [Category("Response")]
        [RequiredArgument]
        public InArgument<string> Channel { get; set; }

        [Category("Response")]
        [RequiredArgument]
        public InArgument<string> BotId { get; set; }

        [Category("Response")]
        [RequiredArgument]
        public InArgument<string> Target { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            var content = new ResponsePayload
            {
                message = Message.Get(context),
                target = Target.Get(context),
                channel = Channel.Get(context),
                botId = BotId.Get(context)
            };

            var url = $"{ExternalURL.Get(context)}/api/v1/bots/___/mod/uipath/message";

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            {
                request.Headers.Add("Authorization", $"bearer {BotpressToken.Get(context)}");
                var json = JsonConvert.SerializeObject(content);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    using (var response = client.SendAsync(request).Result)
                    {
                        response.EnsureSuccessStatusCode();
                    }
                }
            }
        }
    }
}
