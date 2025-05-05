using Colourful;
using Constructs;
using openhue.Light;

namespace OpenHueCSharpExample;

public class ColoredLightConfig
{
    public string Name { get; set; }
    public int? Brightness { get; set; }
    public bool? On { get; set; }
    public RGBColor Color { get; set; }
}

public class ColouredLight: Construct
{
    public Light Light { get; set; }

    public ColouredLight(Construct scope, string id, ColoredLightConfig config) : base(scope, id)
    {
        IColorConverter<RGBColor, xyYColor> converter =  new ConverterBuilder()
            .FromRGB(RGBWorkingSpaces.sRGB)
            .ToxyY(Illuminants.D65)
            .Build();
        
        xyYColor xyYColor = converter.Convert(config.Color);
        
        this.Light = new Light(scope, "Light", new LightConfig
        {
            Name = config.Name,
            Brightness = config.Brightness,
            On = config.On,
            Color = new LightColor
            {
                X = xyYColor.x,
                Y = xyYColor.y,
                Z = xyYColor.Luminance
            }
        });
    }

    
}