using Colourful;
using Constructs;
using HashiCorp.Cdktf;
using openhue.Light;
using openhue.Provider;

namespace OpenHueCSharpExample
{
    class MainStack : TerraformStack
    {
        public MainStack(Construct scope, string id) : base(scope, id)
        {
            new OpenhueProvider(this, "OpenHueProvider", new OpenhueProviderConfig {
                Cache = true
            });
            
           
            // L1 Construct usage 
            //new Light(this, "Light", new LightConfig {
            //    Name = "lamp_2",
            //    Brightness = 0,
            //    On = true
            //});
            
            // L2 Construct Usage
            new ColouredLight(this, "ColoredLight", new ColoredLightConfig
                {
                    Name = "lamp_2",
                    Brightness = 100,
                    Color = new RGBColor(0, 0, 1),
                }
            );
            
            



        }
    }
}