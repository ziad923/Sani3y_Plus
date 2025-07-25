﻿using MimeKit;

namespace Sani3y_.Helpers
{
    public class Message
    {
        public Message(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();

            To.AddRange(to.Select(x => new MailboxAddress(null, x))); // add all the reicpents inside it
            Subject = subject;
            Content = content;
        }
        public List<MailboxAddress> To {  get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }
       
    }
}
