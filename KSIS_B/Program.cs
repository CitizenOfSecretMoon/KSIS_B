using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace KSIS_B
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine($"Введите адресс построения маршрута:");
            String Name;
            Name = Console.ReadLine();

            Tracert(Name);

            static void ipControll(EndPoint ep)
            {
                string aep = ep.ToString().Remove(ep.ToString().IndexOf(":"));
                IPAddress address;
                bool ControlIP = IPAddress.TryParse(aep, out address);

                try
                {
                    if (ControlIP)
                    {
                        IPAddress addres = IPAddress.Parse(aep);
                        IPHostEntry entry = Dns.GetHostEntry(addres);
                        Console.Write(" [" + entry.HostName + "]");

                    }
                }
                catch (SocketException) {/*(-_-)*/}

            }

            void Tracert(String remoteHost)
            {
                bool EndControl = false;
                byte[] data = new byte[1024];
                int recv = 0;
                int packetsize;
                int sequence;
                Random random = new Random();
                Socket host = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
                IPHostEntry iphe = Dns.GetHostEntry(remoteHost);
                IPEndPoint iep = new IPEndPoint(iphe.AddressList[0], 0);
                EndPoint ep = (EndPoint)iep;
                ICMP packet = new ICMP();
                Size size = new Size();

                host.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 3000);

                Console.WriteLine($"Трассировка маршрута к {remoteHost}");

                for (int i = 1; i < 30 && !EndControl; i++)
                {
                    host.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.IpTimeToLive, i);
                    string TTL = String.Format("{0:d2}", i);
                    Console.Write(TTL);
                    bool ipPoint = false;

                    for (int j = 0; j < 3; j++)
                    {
                        sequence = random.Next();
                        packet = packet.PacketMade(sequence);
                        packetsize = size.SizeCount(packet);
                        DateTime timestart = DateTime.Now;
                        host.SendTo(packet.getBytes(), packetsize, SocketFlags.None, iep);

                        try
                        {
                            data = new byte[1024];
                            recv = host.ReceiveFrom(data, ref ep);
                            TimeSpan timestop = DateTime.Now - timestart;
                            ICMP response = new ICMP(data, recv);

                            if (response.Type == 11 || response.Type == 0)
                            {
                                Console.Write($"{timestop.Milliseconds.ToString(),5} ms");
                                ipPoint = true;
                                if (j == 2)
                                {

                                    Console.Write("   "+ep.ToString());

                                    ipControll(ep);

                                }
                            }

                            if (response.Type == 0 && j == 2)
                            {
                                Console.WriteLine("\n" + "Трассировка завершена.");
                                EndControl = true;
                                break;
                            }

                        }

                        catch (SocketException)
                        {
                            Console.Write($"     *  ");
                            if (j == 2 && !ipPoint)
                                Console.Write("   Превышен интервал ожидания для запроса.");
                            if (j == 2 && ipPoint)
                            {
                                Console.Write(ep.ToString());
                                ipControll(ep);
                            }
                        }

                    }
                    Console.Write("\n");
                }
                host.Close();
            }
            Console.ReadLine();
        }
    }
}
