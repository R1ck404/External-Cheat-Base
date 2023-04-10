using System.Collections;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Client.ClientBase.Modules
{
    public class AimAssist : Module
    {

        const int heightOffset = -7; //height offset
        const int shotKnockback = 1; //shot knockback
        public static bool autokill = false; //triggerbot
        public static bool is_aiming = false; //aiming
        const float speed = .25f; //0.35 slow
        const int msBetweenShots = 350;
        // COLOR
        const int color = 0xc60912;//0xc60912 for shatterline; 0xf94f54 for valorant;
        const int colorVariation = 15; //15 for shatterline, 50 for valorant

        const double size = 75;  // DONT CHANGE
        const int maxCount = 3;

        public static System.DateTime last_aim = System.DateTime.Now; //aiming

        public AimAssist() : base("AimAssist", (char)0x4B, "Combat", "Finds the closest entity and aims at it", true) //0x4B = K
        {
        }

        public override void OnTick()
        {
            // Console.WriteLine("[DEBUG] AimAssist.cs: OnTick()");
            System.DateTime lastshot = System.DateTime.Now;

            Task.Delay(1); // ANTI CRASH

            if (GetAsyncKeyState(0x02) != 0)
            {
                Rectangle rect = new Rectangle((Program.xSize - Program.maxX) / 2, (Program.ySize - Program.maxY) / 2, Program.maxX, Program.maxY);

                var l = Client.ClientBase.Utils.Screen.FindPixels(rect, Color.FromArgb(color), colorVariation);

                if (l.Length > 0)
                {
                    var q = l.OrderBy(t => t.Y).ToArray();

                    List<Vector2> forbidden = new List<Vector2>();

                    for (int i = 0; i < q.Length; i++)
                    {
                        Vector2 current = new Vector2(q[i].X, q[i].Y);
                        if (forbidden.Where(t => (t - current).Length() < size || Math.Abs(t.X - current.X) < size).Count() < 1)
                        {
                            forbidden.Add(current);
                            if (forbidden.Count > maxCount)
                            {
                                break;
                            }
                        }
                    }

                    var closes = forbidden.Select(t => (t - new Vector2(Program.xSize / 2, Program.ySize / 2))).OrderBy(t => t.Length()).ElementAt(0) + new Vector2(3, -3);

                    Move((int)(closes.X * (speed)), (int)((closes.Y * (speed)) + 2));
                }
            }
        }

        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(int vKey);

        public static void Move(int xDelta, int yDelta)
        {
            mouse_event(0x0001, xDelta, yDelta + shotKnockback, 0, UIntPtr.Zero);
        }
    }
}