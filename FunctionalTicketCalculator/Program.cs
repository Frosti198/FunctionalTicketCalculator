using System;
using System.Globalization;

namespace FunctionalTicketCalculator;

public enum TicketType
{
    Standard,
    Vip
}

public enum DayType
{
    Weekday,
    Weekend
}

public class Program
{
    public static decimal ApplyPricingRule(
        decimal price,
        Func<decimal, decimal> rule) =>
        rule(price);

    public static Func<decimal, decimal> GetCategoryDiscountRule(int age, bool isStudent)
    {
        if (age < 6)
            return _ => 0m;

        if (age <= 12)
            return p => p * 0.50m;

        if (isStudent)
            return p => p * 0.85m;

        if (age >= 60)
            return p => p * 0.70m;

        return p => p;
    }

    public static decimal CalculateFinalPrice(
        decimal basePrice,
        int age,
        bool isStudent,
        TicketType ticketType,
        DayType dayType)
    {
        Func<decimal, decimal> categoryRule = GetCategoryDiscountRule(age, isStudent);
        Func<decimal, decimal> vipRule = ticketType == TicketType.Vip ? (p => p * 1.25m) : (p => p);
        Func<decimal, decimal> weekendRule = dayType == DayType.Weekend ? (p => p * 1.10m) : (p => p);

        decimal priceAfterCategory = ApplyPricingRule(basePrice, categoryRule);
        decimal priceAfterVip = ApplyPricingRule(priceAfterCategory, vipRule);
        decimal priceAfterWeekend = ApplyPricingRule(priceAfterVip, weekendRule);

        decimal nonNegativePrice = Math.Max(0m, priceAfterWeekend);
        return Math.Round(nonNegativePrice, 2, MidpointRounding.AwayFromZero);
    }

    public static void Main(string[] args)
    {
        if (args.Length == 5)
        {
            ProcessInput(args[0], args[1], args[2], args[3], args[4]);
            return;
        }

        Console.Write("Enter base price: ");
        string? priceInput = Console.ReadLine();

        Console.Write("Enter age: ");
        string? ageInput = Console.ReadLine();

        Console.Write("Is student (true/false): ");
        string? studentInput = Console.ReadLine();

        Console.Write("Enter ticket type (Standard/Vip): ");
        string? ticketInput = Console.ReadLine();

        Console.Write("Enter day type (Weekday/Weekend): ");
        string? dayInput = Console.ReadLine();

        ProcessInput(priceInput, ageInput, studentInput, ticketInput, dayInput);
    }

    public static void ProcessInput(
        string? priceInput,
        string? ageInput,
        string? studentInput,
        string? ticketInput,
        string? dayInput)
    {
        if (string.IsNullOrWhiteSpace(priceInput) ||
            !decimal.TryParse(priceInput, CultureInfo.InvariantCulture, out decimal basePrice))
        {
            Console.WriteLine("Error: Base price must be a valid decimal number.");
            return;
        }

        if (basePrice < 0m)
        {
            Console.WriteLine("Error: Base price cannot be negative.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ageInput) ||
            !int.TryParse(ageInput, out int age))
        {
            Console.WriteLine("Error: Age must be a valid integer.");
            return;
        }

        if (age < 0)
        {
            Console.WriteLine("Error: Age cannot be negative.");
            return;
        }

        if (string.IsNullOrWhiteSpace(studentInput) ||
            !bool.TryParse(studentInput, out bool isStudent))
        {
            Console.WriteLine("Error: Student status must be either 'true' or 'false'.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ticketInput) ||
            !Enum.TryParse<TicketType>(ticketInput, ignoreCase: true, out TicketType ticketType))
        {
            Console.WriteLine("Error: Ticket type must be either 'Standard' or 'Vip'.");
            return;
        }

        if (string.IsNullOrWhiteSpace(dayInput) ||
            !Enum.TryParse<DayType>(dayInput, ignoreCase: true, out DayType dayType))
        {
            Console.WriteLine("Error: Day type must be either 'Weekday' or 'Weekend'.");
            return;
        }

        decimal finalPrice = CalculateFinalPrice(basePrice, age, isStudent, ticketType, dayType);
        Console.WriteLine($"Final price: {finalPrice.ToString("F2", CultureInfo.InvariantCulture)}");
    }
}
