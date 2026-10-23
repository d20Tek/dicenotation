using D20Tek.DiceNotation.PhysicsSpike;

// Milestone 1 - Physics spike.
// Runs several headless dice rolls to verify that dice collide, stay inside the
// tray, and settle reliably within the maximum animation duration.
var settings = new SpikeSettings();
var animationRandom = new Random();
var resultRandom = new Random();

Console.WriteLine("Aether.Physics2D dice tray spike");
Console.WriteLine($"Tray: {settings.TrayWidth} x {settings.TrayHeight} units, step: {settings.FixedStep:F4}s");
Console.WriteLine();

int[] diceCounts = [1, 2, 4, 6, 10, 20];
var allPassed = true;

foreach (var count in diceCounts)
{
    using var simulation = new DiceTraySimulation(settings, animationRandom);

    // Results are determined before the animation starts and never change afterwards.
    var values = Enumerable.Range(0, count).Select(_ => resultRandom.Next(1, 7)).ToArray();
    simulation.StartRoll(values);

    while (!simulation.IsSettled && !simulation.IsTimedOut)
    {
        simulation.Step();
    }

    var snapshot = simulation.GetSnapshot();
    var insideTray = simulation.AllDiceInsideTray();
    var valuesIntact = snapshot.Dice.Select(d => d.Value).SequenceEqual(values);
    var passed = snapshot.IsSettled && insideTray && valuesIntact;
    allPassed &= passed;

    Console.WriteLine(
        $"{count,3} dice | settled: {snapshot.IsSettled,-5} | in tray: {insideTray,-5} | " +
        $"values intact: {valuesIntact,-5} | time: {snapshot.ElapsedSeconds:F2}s | {(passed ? "PASS" : "FAIL")}");

    foreach (var die in snapshot.Dice.Take(3))
    {
        Console.WriteLine(
            $"      {die.Id}: value={die.Value} pos=({die.X,6:F2},{die.Y,6:F2}) rot={die.Rotation,6:F2}");
    }
}

Console.WriteLine();
Console.WriteLine(allPassed ? "Spike exit criteria met." : "Spike exit criteria NOT met; tune settings.");
return allPassed ? 0 : 1;
