// File: TypeWiseAlertTests.cs
using System;
using Xunit;
using Moq;

public class TypeWiseAlertTests
{
    // Edge Case Tests
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

    // Mock Tests for Controller Communication
    [Fact]
    public void SendToController_CorrectMessageSent()
    {
        // Capture the output sent to Console
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            TypewiseAlert.SendToController(TypewiseAlert.BreachType.TOO_HIGH);

            var result = sw.ToString().Trim();
            Assert.Contains("0xfeed : TOO_HIGH", result);
        }
    }

    [Theory]
    [InlineData(TypewiseAlert.BreachType.TOO_LOW, "Hi, the temperature is too low")]
    [InlineData(TypewiseAlert.BreachType.TOO_HIGH, "Hi, the temperature is too high")]
    public void SendToEmailTests(TypewiseAlert.BreachType breachType, string expectedMessage)
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            TypewiseAlert.SendToEmail(breachType);
            var result = sw.ToString().Trim();
            Assert.Contains(expectedMessage, result);
        }
    }

    // Edge case where no action is needed for NORMAL breach type
    [Fact]
    public void SendToEmail_NormalBreach_NoEmailSent()
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            TypewiseAlert.SendToEmail(TypewiseAlert.BreachType.NORMAL);
            var result = sw.ToString().Trim();
            Assert.Empty(result); // No email should be sent for NORMAL breach
        }
    }

    // Mock Test for CheckAndAlert to ensure correct function flow
    [Fact]
    public void CheckAndAlert_ToController_InvokesControllerCommunication()
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            var batteryChar = new TypewiseAlert.BatteryCharacter { CoolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING, Brand = "BrandX" };
            TypewiseAlert.CheckAndAlert(TypewiseAlert.AlertTarget.TO_CONTROLLER, batteryChar, 40);
            
            var result = sw.ToString().Trim();
            Assert.Contains("0xfeed", result); // Ensure controller communication is invoked
        }
    }

    [Fact]
    public void CheckAndAlert_ToEmail_InvokesEmailCommunication()
    {
        using (var sw = new System.IO.StringWriter())
        {
            Console.SetOut(sw);
            var batteryChar = new TypewiseAlert.BatteryCharacter { CoolingType = TypewiseAlert.CoolingType.PASSIVE_COOLING, Brand = "BrandX" };
            TypewiseAlert.CheckAndAlert(TypewiseAlert.AlertTarget.TO_EMAIL, batteryChar, 40);

            var result = sw.ToString().Trim();
            Assert.Contains("To: a.b@c.com", result); // Ensure email communication is invoked
        }
    }
}
