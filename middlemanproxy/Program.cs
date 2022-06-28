using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace proxy
{
  internal class Program
  {
    private const int LISTEN_PORT = 11236;
    private const string SERVER_IP = "127.0.0.1";
    private const int SERVER_PORT = 11235;
    private static string s_remoteIp = null;
    private static string s_remotePort = null;
    private static string s_clientIp = null;
    private static string s_clientPort = null;


    static void Main(string[] args)
    {
      StartListener();
    }

    private static void StartListener()
    {
      var listener = new UdpClient(LISTEN_PORT);
      var remoteEndPoint = new IPEndPoint(IPAddress.Any, LISTEN_PORT);

      try
      {
        while (true)
        {
          
          Console.WriteLine("Waiting for broadcast");
          byte[] bytesReceiveArray = listener.Receive(ref remoteEndPoint);
          Console.WriteLine($"Received broadcast from {remoteEndPoint}.");
          
          Console.Write(bytesReceiveArray);

          s_remoteIp = remoteEndPoint.Address.ToString();
          s_remotePort = remoteEndPoint.Port.ToString();
          
          if (remoteEndPoint.Port == SERVER_PORT)
          {
            ForwardToClient(listener, bytesReceiveArray, s_clientIp, s_clientPort);
          }

          if (remoteEndPoint.Port != SERVER_PORT)
          {
            s_clientIp = s_remoteIp;
            s_clientPort = s_remotePort;
            ForwardToServer(listener, bytesReceiveArray);
          }
        }
      }
      catch (SocketException e)
      {
        Console.WriteLine(e);
        Console.ReadLine();
      }
      finally
      {
        listener.Close();
      }
    }

    private static void ForwardToServer(UdpClient listener, byte[] bytesReceiveArray)
    {
      var broadcast = IPAddress.Parse(SERVER_IP);
      var response = new IPEndPoint(broadcast, SERVER_PORT);
      listener.Send(bytesReceiveArray, bytesReceiveArray.Length, response);
      
      Console.WriteLine("\n Forwarded to Server on port: " + SERVER_PORT.ToString());

    }
    private static void ForwardToClient(UdpClient listener, byte[] bytesReceiveArray, string remoteIp, string remotePort)
    {
      var broadcast = IPAddress.Parse(remoteIp);
      var response = new IPEndPoint(broadcast, int.Parse(remotePort));
      listener.Send(bytesReceiveArray, bytesReceiveArray.Length, response);
      
      Console.WriteLine("* * * Forwarded to client: " + remoteIp + ":" + remotePort);
    }
  }
}
