using Xunit;
using HashiCorp.Cdktf;
using System;
using System.Collections.Generic;

namespace OpenHueCSharpExample;
    // The tests below are example tests, you can find more information at
    // https://cdk.tf/testing
public class TestColoredLight{
    private static TerraformStack stack = new TerraformStack(Testing.app(), "stack");
    private static MyApplicationsAbstraction appAbstraction = new MyApplicationsAbstraction(stack, "resource");
    private static string synthesized = Testing.synth(stack);

}

