using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics.CodeAnalysis;

namespace D20Tek.DiceNotation.UnitTests.Helpers;

[ExcludeFromCodeCoverage]
public static class AssertExtensions
{
    public static void InRange(this Assert _, int actual, int expectedMin, int expectedMax)
    {
        Console.WriteLine($"Actual value is {actual}: checking in range {expectedMin} - {expectedMax}");
        Assert.IsTrue(actual >= expectedMin && actual <= expectedMax);
    }
}
