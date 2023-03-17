using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Server
{
    private const int SERVER_PORT = 8080;
    private const string SERVER_HOST = "127.0.0.1";
    private readonly TcpListener _listener;

    public Server()
    {
        _listener = new TcpListener(IPAddress.Parse(SERVER_HOST), SERVER_PORT);
    }

    
}
}