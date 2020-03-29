using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Project
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {

            // これは反応しなかったセマフォ。動くようならこっちにしたい
            /*
            const string SemaphoreName = "Kotono_Home_Semaphore";

            using(var semaphore = new System.Threading.Semaphore(1, 1, SemaphoreName, out var createdNew))
            {
                if (!createdNew)
                {
                    return;
                }
            }
            */

            App app = new App();
            app.InitializeComponent();
            app.Run();
        }

        // Mutexで多重起動禁止を実装。セマフォに変えたいね

        private Mutex Mutex = new Mutex(false, "Kotono_Home_Semaphore");

        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            if(!this.Mutex.WaitOne(0, false))
            {
                IntPtr hMWnd = NativeMethods.FindWindow(null, "Kotono_Home");
                if(hMWnd != null && NativeMethods.IsWindow(hMWnd))
                {
                    var hCWnd = NativeMethods.GetLastActivePopup(hMWnd);
                    if(hCWnd != null && NativeMethods.IsWindow(hCWnd) && NativeMethods.IsWindowVisible(hCWnd))
                    {
                        NativeMethods.ShowWindow(hCWnd, (int)SW.SHOWNORMAL);
                        NativeMethods.SetForegroundWindow(hCWnd);
                    }
                }

                this.Mutex.Close();
                this.Mutex = null;
                Shutdown();

            }
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            if(this.Mutex != null)
            {
                this.Mutex.ReleaseMutex();
                this.Mutex.Close();
            }
        }
    }
}
