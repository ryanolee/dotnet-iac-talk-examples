using System.Collections.Generic;
using System.Diagnostics;
using Pulumi;
using Pulumi.AzureNative.Authorization;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using ManagedServiceIdentityType = Pulumi.AzureNative.Web.ManagedServiceIdentityType;

namespace Brumdotnet.Resources;
using Pulumi;
using Pulumi.AzureNative.Storage;

public class WebsiteFunction: Pulumi.ComponentResource
{
    private const string BLOB_STORAGE_PRICIPAL =
        "/providers/Microsoft.Authorization/roleDefinitions/ba92f5b4-2d11-453d-a403-e96b0029c9fe";
    
    public readonly Output<string> functionName;
    
    public WebsiteFunction(string name, ComponentResourceOptions? options = null) : base("pkg:brumdotnent:WebsiteFunction", name, options)
    {
        var resourceGroup = new ResourceGroup("resource-group", new ResourceGroupArgs {
           Location = "uksouth"
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

        var codeZip = new Blob($"zip-{{DateTime.UtcNow:ddMMyyyyhhmmss}}", new BlobArgs
        {
            ResourceGroupName = resourceGroup.Name,
            AccountName = storageAccount.Name,
            ContainerName = codeContainer.Name,
            Type = BlobType.Block,
            Source = new FileArchive("../publish")
        });

        var plan = new AppServicePlan("plan", new AppServicePlanArgs
        {
            ResourceGroupName = resourceGroup.Name,
            Sku = new SkuDescriptionArgs
            {
                Name = "FC1",
                Tier = "FlexConsumption",
            },
            Reserved = true
        });

        var webapp = new WebApp("function-app", new WebAppArgs
        {
            ResourceGroupName = resourceGroup.Name,
            ServerFarmId = plan.Id,
            Kind = "functionapp,linux",
            Identity = new ManagedServiceIdentityArgs
            {
                Type = ManagedServiceIdentityType.SystemAssigned

            },
            FunctionAppConfig = new FunctionAppConfigArgs
            {
                Deployment = new FunctionsDeploymentArgs
                {
                    Storage = new FunctionsDeploymentStorageArgs
                    {
                        Value = Output.Format($"{storageAccount.PrimaryEndpoints.Apply((response => response.Blob))}{codeContainer.Name}"),
                        Type = "blobContainer",
                        Authentication = new FunctionsDeploymentAuthenticationArgs
                        {
                            Type = "SystemAssignedIdentity"
                        }
                            
                    }
                },
                Runtime = new FunctionsRuntimeArgs
                {
                    Name = "dotnet-isolated",
                    Version = "8.0"
                },
                ScaleAndConcurrency = new FunctionsScaleAndConcurrencyArgs
                {
                    InstanceMemoryMB = 
                        512,
                    MaximumInstanceCount = 40,
                }  
            },
            CustomDomainVerificationId = "brum-dot-net-usergroup-2025",
            SiteConfig = new SiteConfigArgs
            {
                Http20Enabled = true,
                AppSettings = {
                    new NameValuePairArgs{ Name = "AzureWebJobsStorage__accountName", Value = storageAccount.Name},
                    new NameValuePairArgs{ Name = "FUNCTIONS_EXTENSION_VERSION", Value = "~4" }
                }
            }
        });

        new RoleAssignment("function-blob-storage-role-assign", new RoleAssignmentArgs
        {
            RoleDefinitionId = WebsiteFunction.BLOB_STORAGE_PRICIPAL,
            Scope = storageAccount.Id,
            PrincipalId = webapp.Identity.Apply(identity =>
            {
                Debug.Assert(identity != null, nameof(identity) + " != null");
                return identity.PrincipalId;
            }),
            PrincipalType = "ServicePrincipal"
        });

        this.functionName = webapp.Name;
        this.RegisterOutputs(new Dictionary<string, object?>
        {
            { "appName", webapp.Name }
        });
    }
}