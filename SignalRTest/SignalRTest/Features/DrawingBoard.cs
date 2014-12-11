using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace SignalRTest.Features
{
    public class DrawingBoard : Hub
    {
        private static int _usersCount = 0;
        private static ConcurrentQueue<Line> _points = GetEmptyPoints();
        private static ConcurrentQueue<Message> _messages = GetEmptyMessages();

        private static ConcurrentQueue<Message> GetEmptyMessages()
        {
            var messages = new ConcurrentQueue<Message>();
            return messages;
        }

        private static ConcurrentQueue<Line> GetEmptyPoints()
        {
            var points = new ConcurrentQueue<Line>();
            return points;
        }

        public override Task OnConnected()
        {
            Interlocked.Increment(ref _usersCount);

            Task updatecount  =  Clients.All.UpdateClientCount(_usersCount);
            Task clientConnected = Clients.Others.ClientConnected();
            Task listPoints   =  Clients.Caller.UpdateBoard(_points);
            Task listMessages =  Clients.Caller.UpdateChat(_messages);

            return updatecount
                .ContinueWith(_ => listPoints)
                .ContinueWith(_ => clientConnected)
                .ContinueWith(_ => listMessages);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            if (_usersCount > 0)
            {
                Interlocked.Decrement(ref _usersCount);
            }

            Task updatecount  =  Clients.All.UpdateClientCount(_usersCount);
            Task clientDisconnected = Clients.All.ClientDisconnected();

            return updatecount.ContinueWith(_ => clientDisconnected);
        }
        
        public Task BroadcastArray(List<Point> array, string color, short size)
        {
            Line newLine = new Line{
                 points = array,
                 color = color,
                 size = size
            };
            _points.Enqueue(newLine);

            return Clients.Others.DrawArray(array, color, size);
        }

        public Task BroadcastClear()
        {
            _points = GetEmptyPoints();
            return Clients.Others.Clear();
        }

        public Task BroadcastMessage(string message)
        {
            var name = Clients.Caller.name;

            Message newMessage = new Message(){
                Name = name,
                mMessage = message
            };

            _messages.Enqueue(newMessage);

            return Clients.Others.AddMessage(name, message);
        }
    }

    public class Line
    {
        public List<Point> points { get; set; }
        public string color { get; set; }
        public short size { get; set; }
    }

    public class Point
    {
       public short x { get; set; }
       public short y { get; set; }
    }

    public class Message
    {
        public string Name { get; set; }
        public string mMessage { get; set; }
    }
}