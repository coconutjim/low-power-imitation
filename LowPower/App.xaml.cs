using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Media;
using System.Windows.Media.Animation;

namespace LowPower
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0104; // filter for system keys (such as alt)
        private const int L_ALT_CODE = 164; // found by experience
        private const int R_ALT_CODE = 165; // found by experience
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;


        [STAThread]
        public static void Main()
        {
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p =>
                             p.ProcessName == proc.ProcessName).Count();
            if (count > 1)
            {
                MessageBox.Show("Already running!");
                Application.Current.Shutdown();
            }
            else
            {
                _hookID = SetHook(_proc);
                MessageBox.Show("Started");
                var application = new App();
                application.InitializeComponent();
                application.Run();
                UnhookWindowsHookEx(_hookID);
            }
        }


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }


        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
   

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == R_ALT_CODE)
                {
                    var window = App.Current.MainWindow as MainWindow;
                    if (! window.IS_BEING_SHOWING)
                    {
                        window.IS_BEING_SHOWING = true;
                        var dispatcherTimer = new DispatcherTimer();
                        dispatcherTimer.Tick += (object sender, EventArgs e) => {
                            dispatcherTimer.Stop();
                            Storyboard sb = window.FindResource("showMe") as Storyboard;
                            sb.Begin(window);
                            // question sound is configured as needed sound on my computer
                            SystemSounds.Question.Play();
                        };
                        dispatcherTimer.Interval = new TimeSpan(0, 0, 5);
                        dispatcherTimer.Start();
                    }
                }
                if (vkCode == L_ALT_CODE)
                {
                    Application.Current.Shutdown();
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

   
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
