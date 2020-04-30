using NetworkSocket;
using NetworkSocket.Plugs;
using NetworkSocket.WebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace AMG_Cloud
{
    public class WebSocketServerMiddleware : WebSocketMiddlewareBase
    {
        protected sealed override void OnBinary(IContenxt context, FrameRequest frame)
        {
            try
            {
                var text = Encoding.UTF8.GetString(frame.Content);
                var jsonResult = (Newtonsoft.Json.Linq.JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(text);
                var ipMessage = jsonResult["ipMessage"];
                //var uip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
                var uuip = context.Session.RemoteEndPoint.ToString();
                var hostname = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
                if (jsonResult["hostName"] != null)
                {
                    hostname = jsonResult["hostName"].ToString();
                }
                //预处理IP
                var IPMessage = new Dictionary<string, string>();
                foreach (JProperty jp in ipMessage)
                {
                    var ip = hostname + " : " + jp.Name;
                    var request = jp.Value.ToString();
                    if (IPMessage.ContainsKey(ip))
                    {
                        IPMessage[ip] = request;
                    }
                    else
                    {
                        IPMessage.Add(ip, request);
                    }
                }
                //Console.WriteLine(uip);
                //处理IP主机
                if (Globle.ServeIPMessage.ContainsKey(uuip))
                {
                    Globle.ServeIPMessage[uuip] = IPMessage;
                }
                else
                {
                    Globle.ServeIPMessage.Add(uuip, IPMessage);
                }


                //var session = (WebSocketSession)context.Session.Wrapper;
                //session.SendBinary(byteArray);
                //item.SendBinary(byteArray);
            }
            catch (Exception ex)
            {
                var log = "[WSS]服务端发生错误 " + ex.Message + ":" + ex.StackTrace;
                Console.WriteLine(log);
                //Globle.AddDataLog(log);
            }
        }
    }

    public class CustomWSServerPlug : PlugBase
    {
        protected sealed override void OnConnected(object sender, IContenxt context)
        {
            var log = string.Format("[WSS]时间:{0} 用户:{1} 连接", DateTime.Now.ToString("mm:ss"), context.Session.ToString());
            //Debug.Log(log);
            Console.WriteLine(log);
        }

        protected sealed override void OnDisconnected(object sender, IContenxt context)
        {
            var log = string.Format("[WSS]时间:{0} 用户:{1} 断开连接", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString()); 
            //var uip = ((System.Net.IPEndPoint)context.Session.RemoteEndPoint).Address.ToString();
            var uuip = context.Session.RemoteEndPoint.ToString();
            Globle.ServeIPMessage.Remove(uuip);
            Console.WriteLine(log);
            //Debug.Log(log);
            /*var RemoteIPMessage = Globle.RemoteIPMessage;
            foreach (KeyValuePair<string, string> kvp in RemoteIPMessage)
            {
                if (kvp.Key.IndexOf(context.Session.RemoteEndPoint.ToString()) > 0)
                {
                    Globle.RemoteIPMessage.Remove(kvp.Key);
                }
            }
            Globle.AddDataLog(log);
            Globle.globleIPChanged = true;*/
            //要处理
        }

        protected sealed override void OnException(object sender, Exception exception)
        {
            var log = string.Format("[WSS]时间:{0} 发生错误：{1} {2}", DateTime.Now.ToString("mm:ss"), exception.Message, exception.StackTrace);
            //Debug.Log(log);
            //Globle.AddDataLog(log);
            //Globle.IPMessage.Remove(context.Session.RemoteEndPoint.ToString());
            Console.WriteLine(log);
        }

        /*protected sealed override void OnRequested(object sender, IContenxt context)
        {

            var request = context.StreamReader.ReadString(Encoding.ASCII).Replace("\n", "");
            var log = string.Format("[WSS]时间:{0} 用户:{1} 信息:{2}", DateTime.Now.ToString("mm:ss"), context.Session.RemoteEndPoint.ToString(), request);
            //Debug.Log(log);
            Console.WriteLine(log);
        }*/
    }


}
