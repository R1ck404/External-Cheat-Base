using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using Client.ClientBase.Fonts;
using Client.ClientBase.Utils;

namespace Client.ClientBase.Modules
{
    public class ArrayList : VisualModule
    {
        private Point uiPosition = new Point(0, 0);
        private readonly Font uiFont = new Font(FontRenderer.sigmaFamily, 16);
        private List<string> lastModuleNames = new List<string>();
        private readonly Dictionary<string, float> modulePositions = new Dictionary<string, float>();

        public ArrayList() : base("ArrayList", (char)0x2D, "UI", "Shows all enabled modules from longest name to shortest", false) // 0x2D = INSERT
        {
        }

        public override void OnDraw(Graphics graphics)
        {
            List<Module> enabledModules = ModuleManager.GetEnabledModules();
            List<string> moduleNames = enabledModules.Select(m => m.name).OrderByDescending(n => n.Length).ToList();

            int lineHeight = (int)graphics.MeasureString("M", uiFont).Height;
            int maxWidth = 0;
            foreach (string moduleName in moduleNames)
            {
                int width = (int)graphics.MeasureString(moduleName, uiFont).Width + 10;
                if (width > maxWidth) maxWidth = width;
            }

            float uiXPosition = Program.xSize - maxWidth - 10;
            float uiYPosition = uiPosition.Y + 10;

            foreach (string moduleName in moduleNames)
            {
                int width = (int)graphics.MeasureString(moduleName, uiFont).Width + 10;
                RectangleF textBoxRect = new RectangleF(uiXPosition + maxWidth - width, uiYPosition, width, lineHeight);

                bool isNewModule = !lastModuleNames.Contains(moduleName);
                if (isNewModule)
                {
                    modulePositions.Add(moduleName, Program.xSize);
                }

                if (modulePositions.TryGetValue(moduleName, out float xPosition))
                {
                    float targetXPosition = uiXPosition + maxWidth - width;
                    float delta = targetXPosition - xPosition;
                    if (Math.Abs(delta) > 0.1)
                    {
                        xPosition += delta * 0.2f;
                        modulePositions[moduleName] = xPosition;
                        textBoxRect.X = xPosition;
                    }
                    else
                    {
                        modulePositions.Remove(moduleName);
                    }
                }

                RectangleF gradientRect = new(textBoxRect.Right, textBoxRect.Top + 1, 3, textBoxRect.Height - 2);
                using (LinearGradientBrush gradientBrush = new(gradientRect, Color.FromArgb(150, 122, 183, 255), Color.FromArgb(122, 183, 255), LinearGradientMode.Vertical))
                {
                    graphics.FillRectangle(gradientBrush, gradientRect);
                }

                Rectangle textBoxRect2 = new(
                    (int)textBoxRect.X,
                    (int)textBoxRect.Y,
                    (int)textBoxRect.Width,
                    (int)textBoxRect.Height
                );

                using (GraphicsPath path = RenderUtil.CustomRoundedRectangle(textBoxRect2, 2, 0, 0, 4))
                {
                    graphics.FillPath(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), path);
                }


                graphics.DrawString(moduleName, uiFont, new SolidBrush(Color.White), textBoxRect, new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                uiYPosition += lineHeight;
            }

            lastModuleNames = moduleNames;
        }
    }
}