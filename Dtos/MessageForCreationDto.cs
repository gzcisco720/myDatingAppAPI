using System;

namespace myDotnetApp.API.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int RecipientId { get; set; }
        public DateTime MessageSend { get; set; }
        public string Content { get; set; }
        public MessageForCreationDto()
        {
            MessageSend = DateTime.Now;
        }
    }
}