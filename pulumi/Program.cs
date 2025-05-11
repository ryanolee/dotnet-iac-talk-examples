using System.Collections.Generic;
using Brumdotnet.Resources;
using Pulumi;

return await Deployment.RunAsync(() =>
{
    // Add your resources here
    // e.g. var resource = new Resource("name", new ResourceArgs { });
    new WebsiteFunction("website");

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["outputKey"] = "outputValue"
    };
});
