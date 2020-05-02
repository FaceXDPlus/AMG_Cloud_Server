using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMG_Server_GUI
{
    static class Program
    {
        public static MainForm Form;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(Form = new MainForm());
        }

        public static void AppendText(String message)
        {
            DateTime timestamp = DateTime.Now;
            Form.LogBox.AppendText(Environment.NewLine + timestamp.ToLongTimeString() + "\t" + message);
        }
    }
}
