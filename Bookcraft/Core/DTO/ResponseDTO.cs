using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTO
{
    public class ResponseDTO
    {
        public class OpenAiChatRequest
        {
            public string model { get; set; }
            public List<Message> messages { get; set; }
            public double temperature { get; set; } = 0.7;
        }

        public class Message
        {
            public string role { get; set; } // system | user | assistant
            public string content { get; set; }
        }

        public class OpenAiChatResponse
        {
            public List<Choice> choices { get; set; }
        }

        public class Choice
        {
            public Message message { get; set; }
        }

    }
}
