using System;
using System.Collections.Generic;

public class TypewiseAlert
{
    public enum BreachType
    {
        NORMAL,
        TOO_LOW,
        TOO_HIGH
    }

    public static BreachType InferBreach(double value, double lowerLimit, double upperLimit)
    {
        if (value < lowerLimit) return BreachType.TOO_LOW;
        if (value > upperLimit) return BreachType.TOO_HIGH;
        return BreachType.NORMAL;
    }

    public enum CoolingType
    {
        PASSIVE_COOLING,
        HI_ACTIVE_COOLING,
        MED_ACTIVE_COOLING
    }

    private static Dictionary<CoolingType, (int lower, int upper)> coolingLimits = new Dictionary<CoolingType, (int lower, int upper)>
    {
        { CoolingType.PASSIVE_COOLING, (0, 35) },
        { CoolingType.HI_ACTIVE_COOLING, (0, 45) },
        { CoolingType.MED_ACTIVE_COOLING, (0, 40) }
    };

    public static BreachType ClassifyTemperatureBreach(CoolingType coolingType, double temperatureInC)
    {
        var limits = coolingLimits[coolingType];
        return InferBreach(temperatureInC, limits.lower, limits.upper);
    }

    public enum AlertTarget
    {
        TO_CONTROLLER,
        TO_EMAIL
    }

    public struct BatteryCharacter
    {
        public CoolingType CoolingType;
        public string Brand;
    }

    public static void CheckAndAlert(AlertTarget alertTarget, BatteryCharacter batteryChar, double temperatureInC)
    {
        BreachType breachType = ClassifyTemperatureBreach(batteryChar.CoolingType, temperatureInC);
        Alert(alertTarget, breachType);
    }

    private static void Alert(AlertTarget alertTarget, BreachType breachType)
    {
        if (alertTarget == AlertTarget.TO_CONTROLLER)
            SendToController(breachType);
        else if (alertTarget == AlertTarget.TO_EMAIL)
            SendToEmail(breachType);
    }

    public static void SendToController(BreachType breachType)
    {
        const ushort header = 0xfeed;
        Console.WriteLine("{0:x} : {1}\n", header, breachType); // Format header as hexadecimal
    }

    private static Dictionary<BreachType, string> breachMessages = new Dictionary<BreachType, string>
    {
        { BreachType.TOO_LOW, "Hi, the temperature is too low" },
        { BreachType.TOO_HIGH, "Hi, the temperature is too high" }
    };

    public static void SendToEmail(BreachType breachType)
    {
        string recepient = "a.b@c.com";
        if (breachMessages.ContainsKey(breachType))
        {
            Console.WriteLine($"To: {recepient}\n");
            Console.WriteLine($"{breachMessages[breachType]}\n");
        }
    }
}
