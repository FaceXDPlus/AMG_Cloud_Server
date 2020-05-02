using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AMG_Server_GUI
{
    class SocketManager
    {
        private NetworkSocket.TcpListener P2Plistener;
        public bool P2PServerStatus = false;
        public void P2PServerStart()
        {
            try
            {
                P2Plistener = new NetworkSocket.TcpListener();
                //P2Plistener.Use<HttpMiddleware>();
                P2Plistener.Use<WebSocketServerMiddleware>();
                P2Plistener.UsePlug<CustomWSServerPlug>();
                P2Plistener.Start(1314);
                this.P2PServerStatus = true;
                Program.AppendText("[WSS]本地服务器已经启动");
            }
            catch (Exception ex)
            {
                Program.AppendText("[WSS]本地服务器发生错误 " + ex.Message + " : " + ex.StackTrace);
            }
        }

        public void P2PServerStop()
        {
            this.P2PServerStatus = false;
            P2Plistener.Dispose();
            Program.AppendText("[WSS]本地服务器已经关闭");
        }

        public void SendBinaryToClients()
        {
            //Console.WriteLine(JsonConvert.SerializeObject(Globle.ServeIPMessage));
            var webSocketSessions = P2Plistener.SessionManager.FilterWrappers<WebSocketSession>();
            foreach (var item in webSocketSessions)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(SendToClient));//创建线程

                thread.Start(item);
            }
            /*
                
            */
        }

        public void SendToClient(object ssession)
        {
            try
            {
                var item = (WebSocketSession)ssession;
                //var uip = ((System.Net.IPEndPoint)item.RemoteEndPoint).Address.ToString();
                var uuip = item.RemoteEndPoint.ToString();
                var rIPMessage = new Dictionary<string, string>();
                var aServeIPMessage = ObjectCopier.Clone(Globle.ServeIPMessage);
                //Console.WriteLine(JsonConvert.SerializeObject(aServeIPMessage));
                aServeIPMessage.Remove(uuip);
                //Console.WriteLine(JsonConvert.SerializeObject(aServeIPMessage));
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in aServeIPMessage)
                {
                    foreach (KeyValuePair<string, string> kkvp in kvp.Value)
                    {
                        if (rIPMessage.ContainsKey(kkvp.Key))
                        {
                            rIPMessage[kkvp.Key] = kkvp.Value;
                        }
                        else
                        {
                            rIPMessage.Add(kkvp.Key, kkvp.Value);
                        }
                    }
                }
                var aa = new { keyboardAttached = new ArrayList(), ipMessage = rIPMessage };
                byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(aa));
                //Console.WriteLine(JsonConvert.SerializeObject(aa));
                item.SendBinary(byteArray);
            }
            catch (Exception ex)
            {
                Program.AppendText("[WSS]发生错误：" + ex.Message + " | " + ex.StackTrace);
            }
        }

    }
}

/*
 *  try
            {
                var item = (WebSocketSession)ssession;
                var uip = ((System.Net.IPEndPoint)item.RemoteEndPoint).Address.ToString();
                var uuip = item.RemoteEndPoint.ToString();
                var rIPMessage = new Dictionary<string, string>();
                var aServeIPMessage = ObjectCopier.Clone(Globle.ServeIPMessage);
                //var aServeIPMessage = Globle.ServeIPMessage;
                aServeIPMessage.Remove(uip);
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in aServeIPMessage)
                {
                    foreach (KeyValuePair<string, string> kkvp in kvp.Value)
                    {
                        if (rIPMessage.ContainsKey(kkvp.Key))
                        {
                            rIPMessage[kkvp.Key] = kkvp.Value;
                        }
                        else
                        {
                            rIPMessage.Add(kkvp.Key, kkvp.Value);
                        }
                    }
                }
                var aa = new { keyboardAttached = new ArrayList(), ipMessage = rIPMessage };
                byte[] byteArray = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(aa));
                item.SendBinary(byteArray);
            } 
            catch(Exception ex)
            {
                Console.WriteLine("[WSS]发生错误：" + ex.Message + " | " + ex.StackTrace);
            }
 * */