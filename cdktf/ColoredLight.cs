using System;
using HashiCorp.Cdktf;

namespace OpenHueCSharpExample
{
    class ColoredLight
    {
        public static void Main(string[] args)
        {
            App app = new App();
            new MainStack(app, "OpenHueStack");
            app.Synth();
            Console.WriteLine("App synth complete");
        }
    }
}