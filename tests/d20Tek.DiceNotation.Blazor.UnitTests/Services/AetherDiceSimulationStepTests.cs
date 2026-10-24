namespace d20Tek.DiceNotation.Blazor.UnitTests.Services;

[TestClass]
public class AetherDiceSimulationStepTests
{
    private static AetherDiceSimulation CreateSimulation(DiceSimulationSettings? settings = null) =>
        new(settings, new Random(4321));

    private static void RunToCompletion(AetherDiceSimulation simulation)
    {
        var settings = new DiceSimulationSettings();
        var maxIterations = (int)(settings.MaxDurationSeconds / settings.FixedStep) + 10;

        for (var i = 0; i < maxIterations && !simulation.IsSettled; i++)
        {
            simulation.Step(settings.FixedStep);
        }
    }

    [TestMethod]
    public void Step_BeforeStartRoll_PerformsNoWork()
    {
        // Arrange
        using var simulation = CreateSimulation();

        // Act
        var steps = simulation.Step(1f);

        // Assert
        Assert.AreEqual(0, steps);
        Assert.AreEqual(0f, simulation.ElapsedSeconds);
    }

    [TestMethod]
    public void Step_WithFixedStepElapsed_PerformsSingleStep()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        var steps = simulation.Step(settings.FixedStep);

        // Assert
        Assert.AreEqual(1, steps);
        Assert.AreEqual(settings.FixedStep, simulation.ElapsedSeconds, 0.0001f);
    }

    [TestMethod]
    public void Step_WithPartialElapsed_AccumulatesUntilFixedStepReached()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        var firstSteps = simulation.Step(settings.FixedStep / 2f);
        var secondSteps = simulation.Step(settings.FixedStep / 2f);

        // Assert
        Assert.AreEqual(0, firstSteps);
        Assert.AreEqual(1, secondSteps);
    }

    [TestMethod]
    public void Step_WithLargeElapsed_ClampsToMaxCatchUpSteps()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxCatchUpSteps = 3 };
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        var steps = simulation.Step(60f);

        // Assert
        Assert.AreEqual(3, steps);
        Assert.AreEqual(settings.FixedStep * 3f, simulation.ElapsedSeconds, 0.0001f);
    }

    [TestMethod]
    public void Step_AfterLargeElapsed_DropsBacklogInsteadOfCatchingUpLater()
    {
        // Arrange
        var settings = new DiceSimulationSettings { MaxCatchUpSteps = 2 };
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);
        simulation.Step(60f);

        // Act
        var steps = simulation.Step(settings.FixedStep);

        // Assert
        Assert.AreEqual(1, steps);
    }

    [TestMethod]
    [DataRow(0f)]
    [DataRow(-1f)]
    [DataRow(float.NaN)]
    [DataRow(float.PositiveInfinity)]
    public void Step_WithNonPositiveOrNonFiniteElapsed_PerformsNoStep(float elapsed)
    {
        // Arrange
        using var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);

        // Act
        var steps = simulation.Step(elapsed);

        // Assert
        Assert.AreEqual(0, steps);
        Assert.AreEqual(0f, simulation.ElapsedSeconds);
    }

    [TestMethod]
    public void Step_WhenRunToCompletion_SettlesWithinMaxDuration()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(6), DiceTraySize.Default);

        // Act
        RunToCompletion(simulation);

        // Assert
        Assert.IsTrue(simulation.IsSettled);
        Assert.IsLessThanOrEqualTo(settings.MaxDurationSeconds, simulation.ElapsedSeconds);
    }

    [TestMethod]
    public void Step_WhenSettled_PerformsNoFurtherWork()
    {
        // Arrange
        var settings = new DiceSimulationSettings();
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(2), DiceTraySize.Default);
        RunToCompletion(simulation);
        var settledTime = simulation.ElapsedSeconds;

        // Act
        var steps = simulation.Step(settings.FixedStep);

        // Assert
        Assert.AreEqual(0, steps);
        Assert.AreEqual(settledTime, simulation.ElapsedSeconds);
    }

    [TestMethod]
    public void Step_WithUnreachableSettleThresholds_StopsAtMaxDuration()
    {
        // Arrange
        var settings = new DiceSimulationSettings
        {
            LinearSleepThreshold = 0f,
            AngularSleepThreshold = 0f,
            LinearDamping = 0f,
            AngularDamping = 0f,
            Restitution = 1f,
            Friction = 0f,
            MaxDurationSeconds = 1f,
            MaxCatchUpSteps = int.MaxValue,
        };
        using var simulation = CreateSimulation(settings);
        simulation.StartRoll(VisualDiceFactory.Create(3), DiceTraySize.Default);

        // Act
        simulation.Step(5f);

        // Assert
        Assert.IsTrue(simulation.IsSettled);
        Assert.IsTrue(simulation.IsTimedOut);
        Assert.IsLessThan(settings.MaxDurationSeconds + settings.FixedStep, simulation.ElapsedSeconds);
    }

    [TestMethod]
    public void Step_WhenRunToCompletion_KeepsDiceInsideTray()
    {
        // Arrange
        using var simulation = CreateSimulation();
        simulation.StartRoll(VisualDiceFactory.Create(10), DiceTraySize.Default);

        // Act
        RunToCompletion(simulation);

        // Assert
        Assert.IsTrue(simulation.AllDiceInsideTray());
    }

    [TestMethod]
    public void Step_WhenRunToCompletion_PreservesAssignedValues()
    {
        // Arrange
        using var simulation = CreateSimulation();
        var dice = VisualDiceFactory.CreateFromValues(2, 4, 6, 1, 3);
        simulation.StartRoll(dice, DiceTraySize.Default);

        // Act
        RunToCompletion(simulation);

        // Assert
        Assert.AreSequenceEqual([.. dice.Select(d => d.Value)], [.. simulation.GetSnapshot().Dice.Select(d => d.Value)]);
    }
}
