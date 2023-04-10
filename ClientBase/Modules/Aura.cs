using System.Collections;
using System.Drawing.Imaging;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Client.ClientBase.Modules
{
    public class Aura : Module
    {

        const int heightOffset = 7;
        const int shotKnockback = 1;
        public static bool is_aiming = false;
        const float speed = 0.5f;
        const int msBetweenShots = 350;
        // COLOR
        const int color = 0xc60912;
        const int colorVariation = 10;

        const double size = 75;
        const int maxCount = 3;
        public static bool is_shooting = false;
        public static System.DateTime last_aim = System.DateTime.Now;
        public readonly Pen _pen = new Pen(Color.Green, 3);
        public static Vector2 lastTargetPosition = Vector2.Zero;
        public static bool foundLastTarget = false;

        public Aura() : base("Aura", (char)0x4C, "Combat", "Finds the closest entity and aims and shoots at it", true) //0x4C = L
        {
        }

        public override void OnTick()
        {
            DateTime lastshot = DateTime.UtcNow;
            Rectangle rect = new((Program.xSize) / 4, (Program.ySize - Program.maxY) / 2, Program.xSize / 2, 80);
            var l = Client.ClientBase.Utils.Screen.FindPixels(rect, Color.FromArgb(color), colorVariation);

            if (l.Length > 0)
            {
                var q = l.OrderBy(t => t.Y).ToArray();
                List<Vector2> forbidden = new();

                for (int i = 0; i < q.Length; i++)
                {
                    Vector2 current = new(q[i].X, q[i].Y);
                    if (!forbidden.Any(t => (t - current).Length() < size || Math.Abs(t.X - current.X) < size))
                    {
                        forbidden.Add(current);
                        if (forbidden.Count > maxCount)
                        {
                            break;
                        }
                    }
                }

                Vector2 currentTargetPosition = Vector2.Zero;
                if (foundLastTarget)
                {
                    float closestDist = float.MaxValue;
                    foreach (var target in forbidden)
                    {
                        float dist = (target - lastTargetPosition).Length();
                        if (dist < closestDist)
                        {
                            closestDist = dist;
                            currentTargetPosition = target;
                        }
                    }
                }
                else
                {
                    currentTargetPosition = forbidden[0];
                    foundLastTarget = true;
                }

                var delta = currentTargetPosition - new Vector2(Program.xSize / 2, Program.ySize / 2);
                Move((int)(delta.X * speed), (int)((delta.Y * speed) + 1));
                aimIn();
                lastTargetPosition = currentTargetPosition;

                if (!is_shooting)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
                    is_shooting = true;
                }
            }
            else
            {
                aimOut();
                if (is_shooting)
                {
                    mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
                    is_shooting = false;
                }
                foundLastTarget = false;
            }
        }


        public static void aimIn()
        {
            if (!is_aiming)
            {
                mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
                is_aiming = true;
                last_aim = System.DateTime.Now;
            }
        }

        public static void aimOut()
        {
            if (DateTime.Now.Subtract(last_aim).TotalMilliseconds < 500)
            {
                return;
            }

            if (is_aiming)
            {
                mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
                is_aiming = false;
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

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
    }
}