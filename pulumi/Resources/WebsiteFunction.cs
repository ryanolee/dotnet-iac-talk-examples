using Pulumi;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;

namespace Brumdotnet.Resources;
using Pulumi;
using Pulumi.AzureNative.Storage;

public class WebsiteFunction: Pulumi.ComponentResource
{
    public WebsiteFunction(string name, ComponentResourceOptions? options = null) : base("pkg:brumdotnent:WebsiteFunction", name, options)
    {
        var resourceGroup = new ResourceGroup("resource-group", new ResourceGroupArgs {
           Location = "uk"
        });

        var storageAccount = new StorageAccount("storage-account", new StorageAccountArgs
        {
            AccountName = "brumdotnet",
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuArgs
            {
                Name = SkuName.Standard_LRS
            },
            Kind = Kind.StorageV2
        });

        var codeContainer = new BlobContainer("zip-storage", new BlobContainerArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountName = storageAccount.Name,
        });

        var codeZip = new Blob("zip", new BlobArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountName = storageAccount.Name,
            ContainerName = codeContainer.Name,
        });

        var plan = new AppServicePlan("plan", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuDescriptionArgs
            {
                Name = "y1",
                Tier = "dynamic",
            }
        });

        var webapp = new WebApp("function-app", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = plan.Id,
            Kind = "functionapp",
            SiteConfig = new SiteConfigArgs
            {
                Http20Enabled = true,
                AppSettings = {
                    new NameValuePairArgs{ Name = "FUNCTIONS_WORKER_RUNTIME", Value = "dotnet-isolated"},
                    new NameValuePairArgs{ Name = "WEBSITE_RUN_FROM_PACKAGE", Value = "AnotherValue"}
                }
            }
        });
    }
    // {@see https://github.com/martinjt/pulumi-dotnet5/blob/main/infra/FunctionApp.cs}
    private static Output<string> SignedBlobReadUrl(Blob blob, BlobContainer container, StorageAccount account, Output<string> resourceGroupName)
    {
        return Output.Tuple<string, string, string, string>(
            blob.Name, container.Name, account.Name, resourceGroupName).Apply(t =>
        {
            (string blobName, string containerName, string accountName, string resourceGroupName) = t;

            var blobSAS = ListStorageAccountServiceSAS.InvokeAsync(new ListStorageAccountServiceSASArgs
            {
                AccountName = accountName,
                Protocols = HttpProtocol.Https,
                SharedAccessStartTime = "2021-01-01",
                SharedAccessExpiryTime = "2030-01-01",
                Resource = SignedResource.C,
                ResourceGroupName = resourceGroupName,
                Permissions = Permissions.R,
                CanonicalizedResource = "/blob/" + accountName + "/" + containerName,
                ContentType = "application/json",
                CacheControl = "max-age=5",
                ContentDisposition = "inline",
                ContentEncoding = "deflate",
            });
            return Output.Format($"https://{accountName}.blob.core.windows.net/{containerName}/{blobName}?{blobSAS.Result.ServiceSasToken}");
        });
    }
    
    // {@see https://github.com/martinjt/pulumi-dotnet5/blob/main/infra/FunctionApp.cs}
    private static Output<string> GetConnectionString(Input<string> resourceGroupName, Input<string> accountName)
    {
        // Retrieve the primary storage account key.
        var storageAccountKeys = Output.All<string>(resourceGroupName, accountName).Apply(t =>
        {
            var resourceGroupName = t[0];
            var accountName = t[1];
            return ListStorageAccountKeys.InvokeAsync(
                new ListStorageAccountKeysArgs
                {
                    ResourceGroupName = resourceGroupName,
                    AccountName = accountName
                });
        });
        return storageAccountKeys.Apply(keys =>
        {
            var primaryStorageKey = keys.Keys[0].Value;

                // Build the connection string to the storage account.
                return Output.Format($"DefaultEndpointsProtocol=https;AccountName={accountName};AccountKey={primaryStorageKey}");
        });
    }
}