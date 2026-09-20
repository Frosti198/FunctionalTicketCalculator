using Xunit;
using FunctionalTicketCalculator;

namespace FunctionalTicketCalculator.Tests;

public class TicketCalculatorTests
{
    [Theory]
    [InlineData(5000, 5, false, TicketType.Standard, DayType.Weekday, 0.00)]
    [InlineData(5000, 10, false, TicketType.Standard, DayType.Weekday, 2500.00)]
    [InlineData(5000, 20, true, TicketType.Standard, DayType.Weekday, 4250.00)]
    [InlineData(5000, 65, false, TicketType.Standard, DayType.Weekday, 3500.00)]
    [InlineData(5000, 30, false, TicketType.Vip, DayType.Weekday, 6250.00)]
    [InlineData(5000, 30, false, TicketType.Standard, DayType.Weekend, 5500.00)]
    [InlineData(5000, 20, true, TicketType.Vip, DayType.Weekend, 5843.75)]
    public void CalculateFinalPrice_DocumentedCases_ReturnExpectedResults(
        decimal basePrice,
        int age,
        bool isStudent,
        TicketType ticketType,
        DayType dayType,
        decimal expected)
    {
        decimal actual = Program.CalculateFinalPrice(basePrice, age, isStudent, ticketType, dayType);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ChildUnderSix_WithVipAndWeekend_RemainsFree()
    {
        decimal result = Program.CalculateFinalPrice(5000, 4, false, TicketType.Vip, DayType.Weekend);
        Assert.Equal(0.00m, result);
    }

    [Fact]
    public void RulePriority_StudentTakesPrecedenceOverSenior()
    {
        decimal result = Program.CalculateFinalPrice(5000, 65, true, TicketType.Standard, DayType.Weekday);
        Assert.Equal(4250.00m, result);
    }

    [Theory]
    [InlineData(6, 2500.00)]
    [InlineData(12, 2500.00)]
    [InlineData(13, 5000.00)]
    [InlineData(59, 5000.00)]
    [InlineData(60, 3500.00)]
    public void CategoryAgeBoundaries_CalculateExpectedDiscount(int age, decimal expected)
    {
        decimal result = Program.CalculateFinalPrice(5000, age, false, TicketType.Standard, DayType.Weekday);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ZeroBasePrice_ReturnsZero()
    {
        decimal result = Program.CalculateFinalPrice(0, 25, false, TicketType.Standard, DayType.Weekday);
        Assert.Equal(0.00m, result);
    }

    [Fact]
    public void ApplyPricingRule_HigherOrderFunction_AppliesGivenDelegate()
    {
        decimal result = Program.ApplyPricingRule(100m, p => p * 1.5m);
        Assert.Equal(150m, result);
    }
}
