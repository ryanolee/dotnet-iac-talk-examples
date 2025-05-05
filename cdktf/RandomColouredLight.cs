using Colourful;
using Constructs;
using openhue.Light;

namespace OpenHueCSharpExample;

public class RandomColoredLightConfig
{
    public string Name { get; set; }
    public int? Brightness { get; set; }
    public bool? On { get; set; }
}

public class RandomColouredLight: Construct
{
    public Light Light { get; set; }

    public RandomColouredLight(Construct scope, string id, ColoredLightConfig config) : base(scope, id)
    {
        
    }

    
}