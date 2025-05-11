using System.Collections.Generic;
using Brumdotnet.Resources;
using Pulumi;
using Pulumi.Utilities;

return await Deployment.RunAsync(() =>
{
    // Add your resources here
    // e.g. var resource = new Resource("name", new ResourceArgs { });
    var webFunction = new WebsiteFunction("website");
    
    

    // Export outputs here
    return new Dictionary<string, object?>
    {
        ["functionName"] = webFunction.functionName
    };
});
