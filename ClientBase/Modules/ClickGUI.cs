using System;
using System.Linq;
using System.Windows.Forms;

namespace Client.ClientBase.Modules
{
    public class ModuleList : VisualModule
    {
        private bool uiVisible = false;
        private Point uiPosition = new Point(10, 10);
        private Font uiFont = new Font(FontFamily.GenericSansSerif, 15);

        public ModuleList() : base("ModuleList", (char)0x2D, "UI", "Displays a list of all modules", false)
        {
        }

        public override void OnDraw(Graphics graphics)
        {
            var modules = ModuleManager.GetModules().OrderByDescending(x => x.name.Length).ThenBy(x => x.name);
            var maxCategoryWidth = (int)Math.Ceiling(modules.Max(x => graphics.MeasureString(x.category, uiFont).Width));
            var maxModuleWidth = (int)Math.Ceiling(modules.Max(x => graphics.MeasureString($"[{x.keybind}] {x.name}", uiFont).Width));

            var x = uiPosition.X;
            var y = uiPosition.Y;

            foreach (var group in modules.GroupBy(x => x.category).OrderBy(x => x.Key))
            {
                string category = group.Key;
                var moduleCount = group.Count();

                var categoryHeight = (int)Math.Ceiling(graphics.MeasureString(category, uiFont).Height);
                var moduleHeight = (int)Math.Ceiling(graphics.MeasureString($"[{group.First().keybind}] {group.First().name}", uiFont).Height);
                var blockPadding = 5;
                var blockWidth = maxModuleWidth + (2 * blockPadding);
                var blockHeight = categoryHeight + (moduleHeight * moduleCount) + (blockPadding * (moduleCount + 1));

                graphics.FillRectangle(Brushes.Gray, new Rectangle(x, y, blockWidth, blockHeight));
                graphics.DrawString(category, uiFont, Brushes.White, new PointF(x + blockPadding, y + blockPadding));
                var moduleY = y + categoryHeight + (blockPadding * 2);
                foreach (var module in group)
                {
                    string name = $"[{module.keybind}] {module.name}";
                    graphics.DrawString(name, uiFont, Brushes.White, new PointF(x + blockPadding, moduleY));
                    moduleY += moduleHeight + blockPadding;
                }

                x += blockWidth + blockPadding;
            }
        }
    }
}