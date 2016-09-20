using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ZombieNet
{
    class ZombieNet
    {
        public static string nickname = "";
        static void Main()
        {
            //pop version info
            const string version = "0.1 rib";
            Console.WriteLine($"Welcome to ZombieNet [ v{version} ]");

            //credidentials
            Console.Write("\nNickname: ");
            nickname = Console.ReadLine();

            Console.Clear();
            Console.Write("Role: \n-# Client\n-# Server\n#: ");
            string realm = Console.ReadLine();

            Console.Clear();
            if(realm == "Client")
            {
                Console.Write("Enter IP address: ");
                string ipstring = Console.ReadLine();

                Console.Clear();
                Console.Write("Enter port: ");
                int port = Console.Read();

                InitClient(ipstring, port);
            }
            else
            {
                Console.Write("Enter port: ");

                int port = Console.Read();
                InitServer(port);
            }
        }

        static void InitServer(int port)
        {
            Console.Clear();
            Console.WriteLine("# Server Initialized - Awaiting Connections #");

            IPHostEntry iphostinfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress hostip = null;

            //find suitable address
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    hostip = ip;
                }
            }
            IPEndPoint ipendpoint = new IPEndPoint(hostip, 27015);
            
            //receiver (us)
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(ipendpoint);
            listener.Listen(16);

            //all senders (handlers)
            string data = null;
            Socket[] handlers = new Socket[16];
            byte[] bytes = new byte[1024];
            byte[] msg = Encoding.ASCII.GetBytes($"{nickname} <EOF>");
            while (true)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (handlers[i] == null)
                    {
                        handlers[i] = listener.Accept();
                        Console.WriteLine("New connection!");
                        break;
                    }
                    else
                    {
                        int received = handlers[i].Receive(bytes);
                        data += Encoding.ASCII.GetString(bytes, 0, received);
                        if (data.IndexOf("<EOF>") > -1)
                        {
                            Console.WriteLine(data);
                            data = null;
                            break;
                        }
                    }
                }
            }
        }

        static void InitClient(string ipstring, int port)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Clear();
            Console.WriteLine("# Client Initialized #");

            IPAddress ip = IPAddress.Parse(ipstring);
            IPEndPoint ipendpoint = new IPEndPoint(ip, 27015);
            Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sender.Connect(ipendpoint);

            byte[] msg = Encoding.ASCII.GetBytes($"{nickname} <EOF>");
            sender.Send(msg);

            byte[] bytes = new byte[1024];
            while (true)
            {
                string text = Console.ReadLine();
                Console.Write($"{nickname}: ");

                msg = Encoding.ASCII.GetBytes($"{nickname}: {text} <EOF>");
                sender.Send(msg);
            }
        }
    }
}
