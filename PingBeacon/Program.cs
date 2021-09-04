using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace PingBeacon
{
    class Program
    {
        static void Main()
        {
            try
            {
                IPAddress gatewayAddress = GetGatewayAddress();
                SendPing(gatewayAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(-1);
            }
        }

        static IPAddress GetGatewayAddress()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in networkInterfaces)
            {
                if (networkInterface.Description.Contains("Ethernet"))
                {
                    IPInterfaceProperties networkInterfaceProperties = networkInterface.GetIPProperties();
                    GatewayIPAddressInformationCollection gatewayIPAddresses = networkInterfaceProperties.GatewayAddresses;
                    if (gatewayIPAddresses.Count > 0)
                    {
                        foreach (var gatewayIPAddress in gatewayIPAddresses)
                        {
                            IPAddress.TryParse(gatewayIPAddress.Address.ToString(), out IPAddress gatewayIP);
                            if (gatewayIP.AddressFamily is AddressFamily.InterNetwork)
                            {
                                return gatewayIP;
                            }
                        }
                    }
                }
            }

            return null;
        }

        static void SendPing(IPAddress gatewayAddress)
        {
            Ping ping = new Ping();
            Console.WriteLine($"Disparando echo message para endereço {gatewayAddress}:");
            PingReply reply;
            while (true)
            {
                reply = ping.Send(gatewayAddress);
                Console.WriteLine($"Resposta de {gatewayAddress}: tempo={reply.RoundtripTime} TTL={reply.Options.Ttl}");
                System.Threading.Thread.Sleep(5000);
            }
        }
    }
}
