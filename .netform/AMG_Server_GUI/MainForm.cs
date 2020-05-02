using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMG_Server_GUI
{
    public static class Globle
    {
        public static string APPName = "AMG_Cloud_Server";
        public static string APPVersion = "Alpha 0.1";
        public static int ModelNum = 1;
        public static int IPNum = 0;
        public static Dictionary<string, string> ModelToIP;
        public static Dictionary<string, string> IPMessage;
        public static Dictionary<string, string> RemoteIPMessage;

        public static Dictionary<string, Dictionary<string, string>> ServeIPMessage;
        //<主机名,<处理后ip,传输信息>>
        public static Dictionary<string, int> ServeIPMessageTime;
        //主机名, 更新时间

    }
    public partial class MainForm : Form
    {
        private static SocketManager myP2PServer;

        public MainForm()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private static void pTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (myP2PServer.P2PServerStatus == true)
                {
                    myP2PServer.SendBinaryToClients();
                }
            }
            catch { }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LogBox.Text = DateTime.Now.ToLongTimeString() + "\t" + "启动中";

            Program.AppendText(Globle.APPName + " 运行版本:" + Globle.APPVersion);
            Globle.ServeIPMessage = new Dictionary<string, Dictionary<string, string>>();
            Globle.ServeIPMessageTime = new Dictionary<string, int>();

            myP2PServer = new SocketManager();
            myP2PServer.P2PServerStart();

            var ms = 1000 / 60;
            System.Timers.Timer pTimer = new System.Timers.Timer(ms);
            pTimer.Elapsed += pTimer_Elapsed;
            pTimer.AutoReset = true;
            pTimer.Enabled = true;

            //Console.Read();
            //myP2PServer.P2PServerStop();
        }

        private void LogBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
