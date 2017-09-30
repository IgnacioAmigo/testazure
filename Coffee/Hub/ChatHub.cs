using System;
using System.Threading.Tasks;
using System.Web;
using System.Collections;
using Microsoft.AspNet.SignalR;
using System.Collections.Generic;
using Coffee;

namespace SignalRChat
{
    public class ChatHub : Hub
    {
        public struct Message {
            public string name;
            public string text;
        }

        private static List<Message> buf = new List<Message>();
        public void Send(string _name, string _text)
        {
            buf.Add(new Message
            {
                name = _name,
                text = _text,
            });

            var DBContext = new CoffeeEntities();
            var mes = new Messages();
            mes.timestamp = DateTime.Now;
            mes.message = _text;
            mes.sender = _name;

            DBContext.Messages.Add(mes);
            DBContext.SaveChanges();

            Clients.All.newMsg(_name, _text);

        }

        public override Task OnConnected()
        {
            foreach (var m in buf) {
                Clients.Caller.newMsg(m.name, m.text);
            }
            return base.OnConnected();
        }
    }
}