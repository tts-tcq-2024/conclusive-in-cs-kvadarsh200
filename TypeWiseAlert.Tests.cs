using System;
using Xunit;

public class TypeWiseAlertTests
{
    [Theory]
    [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, 0, TypewiseAlert.BreachType.NORMAL)]
    [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, 35, TypewiseAlert.BreachType.NORMAL)]
    [InlineData(TypewiseAlert.CoolingType.PASSIVE_COOLING, -10, TypewiseAlert.BreachType.TOO_LOW)]
    [InlineData(TypewiseAlert.CoolingType.HI_ACTIVE_COOLING, 100, TypewiseAlert.BreachType.TOO_HIGH)]
    public void ClassifyTemperatureBreach_EdgeCases(TypewiseAlert.CoolingType coolingType, double temperatureInC, TypewiseAlert.BreachType expectedBreach)
    {
        TypewiseAlert.BreachType actualBreach = TypewiseAlert.ClassifyTemperatureBreach(coolingType, temperatureInC);
        Assert.Equal(expectedBreach, actualBreach);
    }

    [Theory]
    [InlineData(TypewiseAlert.BreachType.TOO_LOW, "Hi, the temperature is too low")]
    [InlineData(TypewiseAlert.BreachType.TOO_HIGH, "Hi, the temperature is too high")]
    public void SendToEmailTests(TypewiseAlert.BreachType breachType, string expectedMessage)
    {
        var result = CaptureConsoleOutput(() => TypewiseAlert.SendToEmail(breachType));
        Assert.Contains(expectedMessage, result);
    }

    // Refactored Helper Method
    private string CaptureConsoleOutput(Action action)
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            action();
            return sw.ToString().Trim();
        }
    }

    [Fact]
    public void SendToEmail_NormalBreach_NoEmailSent()
    {
        var result = CaptureConsoleOutput(() => TypewiseAlert.SendToEmail(TypewiseAlert.BreachType.NORMAL));
        Assert.Empty(result); // No email should be sent for NORMAL breach
    }

     [Fact]
    public void SendToController_CorrectMessageSent()
    {
        var result = CaptureConsoleOutput(() => TypewiseAlert.SendToController(TypewiseAlert.BreachType.TOO_HIGH));
        Assert.Contains("feed : TOO_HIGH", result); // Matching the formatted output
    }

    [Fact]
    public void CheckAndAlert_ToController_InvokesControllerCommunication()
    {
        var batteryChar = new TypewiseAlert.BatteryCharacter { CoolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING, Brand = "BrandX" };
        var result = CaptureConsoleOutput(() => TypewiseAlert.CheckAndAlert(TypewiseAlert.AlertTarget.TO_CONTROLLER, batteryChar, 40));
        Assert.Contains("feed", result); // Matching the formatted output
    }

    [Fact]
    public void CheckAndAlert_ToEmail_InvokesEmailCommunication()
    {
        var batteryChar = new TypewiseAlert.BatteryCharacter { CoolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING, Brand = "BrandX" };
        var result = CaptureConsoleOutput(() => TypewiseAlert.CheckAndAlert(TypewiseAlert.AlertTarget.TO_EMAIL, batteryChar, 40));
        Assert.Contains("To: a.b@c.com", result); // Ensure email communication is invoked
    }
}

