using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;

namespace SignalRTest.Features
{
    public class DrawingBoard : Hub
    {
        public Task StartBroadcastPoint(int x, int y)
        {
            return Clients.Others.StartPoint(x, y);
        }

        public Task BroadcastPoint(int x, int y)
        {
            string color = Clients.Caller.color;
            return Clients.Others.DrawPoint(x, y, color);
        }

        public Task BroadcastClear()
        {
            return Clients.Others.Clear();
        }
    }
}