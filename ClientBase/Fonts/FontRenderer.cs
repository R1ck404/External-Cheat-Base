using System.Drawing.Text;
using System.IO;
using Client.ClientBase.Utils;

namespace Client.ClientBase.Fonts
{
    public static class FontRenderer
    {
        public static FontFamily comfortaaFamily = FontFamily.GenericSerif;
        public static FontFamily sigmaFamily = FontFamily.GenericSerif;

        public static void Init()
        {
            Console.WriteLine("Loading fonts...");
            PrivateFontCollection fontCollection = new PrivateFontCollection();

            foreach (string filePath in Directory.GetFiles(Program.fontPath, "*.ttf"))
            {
                fontCollection.AddFontFile(filePath);
            }

            FontFamily comfortaaFamily = fontCollection.Families.FirstOrDefault(f => f.Name == "Comfortaa");
            FontFamily sigmaFamily = fontCollection.Families.FirstOrDefault(f => f.Name == "SF UI Display");

            FontRenderer.comfortaaFamily = comfortaaFamily;
            FontRenderer.sigmaFamily = sigmaFamily;

            Console.WriteLine("Fonts loaded.");
        }
    }
}
