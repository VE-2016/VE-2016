using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinExplorer
{
    public class MSTestServer
    {
        private static readonly TcpListener listener = new TcpListener(IPAddress.Any, 6543);

        static public bool running = false;
        
        public MSTestServer()
        {
            
            listener.Start();
            Console.WriteLine("Started MSTest TCP server.");

            running = true;

            while (running)
            {
                Console.WriteLine("Waiting for connection...");

                var client = listener.AcceptTcpClient();
                Console.WriteLine("Connected MSTest!");

                // each connection has its own thread
                new Thread(ServeMSTestData).Start(client);

                break;
            }
        }
        public void Shutdown()
        {
            listener.Stop();
        }
        private void ServeMSTestData(object clientSocket)
        {
            Console.WriteLine("Started MSTest thread  " + Thread.CurrentThread.ManagedThreadId);

            
            running = true;

            var rnd = new Random();
            try
            {
                var client = (TcpClient)clientSocket;
                var stream = client.GetStream();
                var msg = new byte[1000000];
                int i = 0;
                ArrayList es = new ArrayList();
                ArrayList ws = new ArrayList();
                ArrayList me = new ArrayList();

                string prev = null;

                while (running)
                {
                    
                    {
                        int c = 0;
                        try
                        {
                            c = stream.Read(msg, 0, msg.Length);
                        }
                        catch(Exception e)
                        {
                            Shutdown();
                            return;
                        }
                        string s = Encoding.ASCII.GetString(msg, 0, c);

                        s = s.Replace("\r", "");

                        string[] cc = s.Trim().Split("$".ToCharArray());

                        if (prev != null)
                        {
                            cc[0] = prev + cc[0];
                            prev = null;
                        }

                        if (s.EndsWith("\n") == false)
                        {
                            prev = cc[cc.Length - 1];
                            cc[cc.Length - 1] = null;
                        }
                        else prev = null;

                       
                        foreach (string b in cc)
                        {
                            if (b == null)
                                continue;
                            //if (b.StartsWith("$") == false)
                            //    continue;
                            
                            {
                                string[] dd = b.Trim().Replace("\r", "").Split("\n".ToCharArray());
                                
                                string[] gg = new string[dd.Length];
                                int p = 0;
                                foreach (string w in dd)
                                {
                                    if (p == 0)
                                        if (w == "")
                                            continue;

                                    gg[p++] = w;
                                    Debug.WriteLine(w);
                                    ws.Add(w);
                                }

                                dd = gg;

                            
                                i++;
                            }
                   
                
                        
                        }
                    }
                
                
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket exception in thread {0}: {1}", Thread.CurrentThread.ManagedThreadId, e);
            }
        }

        public void Close()
        {
            listener.Stop();
        }
    }

}
