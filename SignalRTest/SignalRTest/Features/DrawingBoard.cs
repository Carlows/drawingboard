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
        private static ConcurrentStack<Line> _points = GetEmptyPoints();

        private static ConcurrentStack<Line> GetEmptyPoints()
        {
            var points = new ConcurrentStack<Line>();
            return points;
        }

        public override Task OnConnected()
        {
            Interlocked.Increment(ref _usersCount);
            Task updatecount = Clients.All.UpdateClientCount(_usersCount);
            Task listPoints = Clients.Caller.UpdateBoard(_points);

            return updatecount.ContinueWith(_ => listPoints);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            Interlocked.Decrement(ref _usersCount);
            return Clients.All.UpdateClientCount(_usersCount);
        }
        
        public Task BroadcastArray(List<Point> array, string color, short size)
        {
            Line newLine = new Line{
                 points = array,
                 color = color,
                 size = size
            };

            _points.Push(newLine);

            return Clients.Others.DrawArray(array, color, size);
        }

        public Task BroadcastClear()
        {
            _points.Clear();
            return Clients.Others.Clear();
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
}