using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Server server = new Server();
            await server.StartAsync();
        }
    }
}
