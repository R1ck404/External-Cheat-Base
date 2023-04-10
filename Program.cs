using Client.ClientBase.UI.Notification;
using Client.ClientBase.UI.Overlay;
using Client.ClientBase.Utils;
using Client.ClientBase.Fonts;
using Client.ClientBase;

namespace Client
{
    partial class Program
    {
        //SIZE
        public static int xSize = 1920;
        public static int ySize = 1080;
        public const int maxX = 110;
        public const int maxY = 110;
        public static Overlay form = new();
        private static readonly System.Windows.Forms.Timer redrawTimer = new();
        private static readonly ManualResetEvent mainThreadEvent = new(false);
        public static readonly string fontPath = "FOLDER TO TYPES HERE";
        public static readonly string client_name = "Rice";


        static void Main(string[] args)
        {
            if (fontPath == "FOLDER TO TYPES HERE")
            {
                Console.WriteLine("Please set the font path in Program.cs (Line: 21)");
                return;
            }

            Console.WriteLine("Starting " + client_name + " Client");
            Console.WriteLine("I like " + client_name);

            Application.ApplicationExit += OnApplicationExit;

            Thread uiApp = new(() =>
            {
                FontRenderer.Init();
                ModuleManager.Init();

                form = new Overlay();

                NotificationManager.Init(form);

                Console.WriteLine("Starting Keymapper...");
                Keymap.StartListening();
                Keymap.KeyPressed += ListenForKeybinds;
                Console.WriteLine("Keymapper started.");

                redrawTimer.Interval = 20;
                redrawTimer.Tick += (sender, e) => form.Invalidate();
                redrawTimer.Start();

                System.Windows.Forms.Application.Run(form);
            });

            uiApp.Start();

            Thread moduleThread = new(() =>
            {
                while (true)
                {
                    try
                    {
                        ModuleManager.OnTick();
                        Thread.Sleep(1);
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            });

            moduleThread.Start();

            mainThreadEvent.WaitOne();
        }

        // [System.Runtime.InteropServices.LibraryImport("user32.dll", EntryPoint = "GetForegroundWindowA")]
        // private static partial IntPtr GetForegroundWindow();

        // [System.Runtime.InteropServices.LibraryImport("user32.dll", EntryPoint = "GetWindowThreadProcessIdA", SetLastError = true)]
        // private static partial uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        // public static bool IsGameForeground()
        // {
        //     IntPtr hwnd = GetForegroundWindow();
        //     GetWindowThreadProcessId(hwnd, out int pid);
        //     Process process = Process.GetProcessById(pid);

        //     return pid == 19688 || process.ProcessName == "Shatterline";
        // }

        private static void ListenForKeybinds(char key)
        {
            // if (!IsGameForeground() && key != 0x2D) return;
            ModuleManager.OnKeyPress(key);
        }

        private static void OnApplicationExit(object sender, EventArgs e)
        {
            redrawTimer.Stop();
            Keymap.StopListening();
            mainThreadEvent.Set();
        }
    }
}
