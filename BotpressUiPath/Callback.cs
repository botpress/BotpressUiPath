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
    public class Callback : CodeActivity
    {
        private class ResponsePayload
        {
            public string text { get; set; }
            public string channel { get; set; }
            public string target { get; set; }
            public string botId { get; set; }
        }

        private static readonly HttpClient client = new HttpClient();

        [Category("Connection")]
        [RequiredArgument]
        public InArgument<string> Protocol { get; set; }

        [Category("Connection")]
        [RequiredArgument]
        public InArgument<string> Host { get; set; }

        [Category("Connection")]
        [RequiredArgument]
        public InArgument<int> Port { get; set; }

        [Category("Message")]
        [RequiredArgument]
        public InArgument<string> Text { get; set; }

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
            var protocol = Protocol.Get(context);
            var host = Host.Get(context);
            var port = Port.Get(context);
            var text = Text.Get(context);

            var payload = new ResponsePayload
            {
                text = text,
                target = Target.Get(context),
                channel = Channel.Get(context),
                botId = BotId.Get(context)
            };

            string json = JsonConvert.SerializeObject(payload);
            Console.WriteLine(json);

            var url = $"{protocol}://{host}:{port}/api/v1/bots/___/mod/uipath/callback";

            var response = client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json")).Result;
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Content);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                Console.WriteLine(responseString);
            } else
            {
                throw new System.InvalidOperationException($"Botpress responded with status code: {response.StatusCode}");
            }
        }
    }
}
