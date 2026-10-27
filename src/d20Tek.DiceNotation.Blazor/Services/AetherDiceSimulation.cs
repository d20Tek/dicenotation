using d20Tek.DiceNotation.Blazor.Models;
using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

namespace d20Tek.DiceNotation.Blazor.Services;

/// <summary>
/// Implements <see cref="IDiceSimulation"/> using the Aether.Physics2D rigid-body engine.
/// Dice are simulated as square dynamic bodies inside a tray built from four static walls.
/// </summary>
/// <remarks>
/// This type holds mutable physics state, so each component instance should own its own
/// simulation rather than sharing one.
/// </remarks>
internal sealed class AetherDiceSimulation : IDiceSimulation
{
    private readonly DiceSimulationSettings _settings;
    private readonly Random _random;
    private readonly List<SimulatedDie> _dice = [];
    private World? _world;
    private DiceTraySize _traySize = DiceTraySize.Default;
    private float _accumulator;
    private float _elapsedSeconds;
    private float _restingSeconds;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AetherDiceSimulation"/> class.
    /// </summary>
    /// <param name="settings">
    /// The physics settings to use, or <see langword="null"/> to use the defaults.
    /// </param>
    /// <param name="random">
    /// The random source used for animation motion only. It must be independent of the
    /// source that generates die results, because changing it must never change a result.
    /// </param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when any setting is invalid.</exception>
    public AetherDiceSimulation(DiceSimulationSettings? settings = null, Random? random = null)
    {
        _settings = settings ?? new DiceSimulationSettings();
        _settings.Validate();
        _random = random ?? Random.Shared;
    }

    /// <inheritdoc />
    public bool IsSettled =>
        _dice.Count > 0 &&
        (_restingSeconds >= _settings.SettleHoldSeconds || _elapsedSeconds >= _settings.MaxDurationSeconds);

    /// <inheritdoc />
    public float ElapsedSeconds => _elapsedSeconds;

    /// <summary>
    /// Gets a value indicating whether the roll stopped because it reached the configured
    /// maximum duration rather than coming to rest naturally.
    /// </summary>
    public bool IsTimedOut => _elapsedSeconds >= _settings.MaxDurationSeconds;

    /// <inheritdoc />
    public void StartRoll(IReadOnlyList<VisualDie> dice, DiceTraySize traySize)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        ArgumentNullException.ThrowIfNull(dice);
        ArgumentNullException.ThrowIfNull(traySize);
        ValidateDice(dice);
        traySize.Validate();

        _traySize = traySize;
        _accumulator = 0f;
        _elapsedSeconds = 0f;
        _restingSeconds = 0f;
        _dice.Clear();

        _world ??= new World(Vector2.Zero);
        _world.Clear();

        CreateWalls();
        for (var i = 0; i < dice.Count; i++)
        {
            _dice.Add(new SimulatedDie(dice[i], CreateBody(dice[i], i, dice.Count)));
        }
    }

    /// <inheritdoc />
    public int Step(float elapsedSeconds)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_world is null || _dice.Count == 0 || IsSettled)
        {
            return 0;
        }

        // Ignore negative or non-finite deltas, which a browser frame callback can produce.
        if (float.IsFinite(elapsedSeconds) && elapsedSeconds > 0f)
        {
            _accumulator += elapsedSeconds;
        }

        var step = _settings.FixedStep;
        var steps = 0;

        while (_accumulator >= step && steps < _settings.MaxCatchUpSteps && !IsSettled)
        {
            _world.Step(step);
            _accumulator -= step;
            _elapsedSeconds += step;
            steps++;

            _restingSeconds = AllDiceAtRest() ? _restingSeconds + step : 0f;
        }

        // Drop any remaining backlog so a long pause cannot accumulate into future frames.
        if (_accumulator >= step)
        {
            _accumulator = 0f;
        }

        // The rest thresholds allow a small residual velocity, so the bodies are brought
        // to a hard stop once the scene settles. Without this the final rendered frame
        // would catch the dice mid-drift.
        if (IsSettled)
        {
            FreezeDice();
        }

        return steps;
    }

    /// <inheritdoc />
    public DiceSceneState GetSnapshot()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (_dice.Count == 0)
        {
            return DiceSceneState.Empty;
        }

        var states = new DieState[_dice.Count];
        for (var i = 0; i < _dice.Count; i++)
        {
            var die = _dice[i];
            states[i] = new DieState(
                die.Die.Id,
                die.Die.Value,
                die.Body.Position.X,
                die.Body.Position.Y,
                die.Body.Rotation,
                _settings.DieSize,
                _settings.DieSize,
                CalculateEnergy(die.Body),
                die.Die.Sides,
                die.Body.LinearVelocity.X,
                die.Body.LinearVelocity.Y,
                die.Die.MinValue);
        }

        return new DiceSceneState(states, _elapsedSeconds, IsSettled);
    }

    /// <summary>
    /// Determines whether every die is still inside the tray, allowing a small tolerance
    /// for resting contact with the walls.
    /// </summary>
    /// <returns><see langword="true"/> when no die has escaped the tray.</returns>
    public bool AllDiceInsideTray()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var maxX = (_traySize.Width / 2f) + _settings.DieSize;
        var maxY = (_traySize.Height / 2f) + _settings.DieSize;

        return _dice.TrueForAll(d => IsInsideTray(d.Body.Position.X, d.Body.Position.Y, maxX, maxY));
    }

    /// <summary>
    /// Determines whether a single position lies within the tray bounds on both axes.
    /// </summary>
    /// <param name="x">The horizontal position to test.</param>
    /// <param name="y">The vertical position to test.</param>
    /// <param name="maxX">The maximum allowed distance from the tray center on the X axis.</param>
    /// <param name="maxY">The maximum allowed distance from the tray center on the Y axis.</param>
    /// <returns><see langword="true"/> when the position is inside the tray.</returns>
    internal static bool IsInsideTray(float x, float y, float maxX, float maxY) => Math.Abs(x) <= maxX && Math.Abs(y) <= maxY;

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _world?.Clear();
        _world = null;
        _dice.Clear();
        _disposed = true;
    }

    private void ValidateDice(IReadOnlyList<VisualDie> dice)
    {
        if (dice.Count == 0)
        {
            throw new ArgumentException("At least one die is required to start a roll.", nameof(dice));
        }

        if (dice.Count > _settings.MaxDiceCount)
        {
            throw new ArgumentException(
                $"The number of dice ({dice.Count}) exceeds the configured maximum of {_settings.MaxDiceCount}.",
                nameof(dice));
        }

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var die in dice)
        {
            if (die is null)
            {
                throw new ArgumentException("Dice collections cannot contain null entries.", nameof(dice));
            }

            if (!ids.Add(die.Id))
            {
                throw new ArgumentException($"Duplicate die identifier '{die.Id}'.", nameof(dice));
            }
        }
    }

    private void CreateWalls()
    {
        var halfWidth = _traySize.Width / 2f;
        var halfHeight = _traySize.Height / 2f;
        var thickness = _settings.WallThickness;
        var spanX = _traySize.Width + (thickness * 2f);
        var spanY = _traySize.Height + (thickness * 2f);

        CreateWall(spanX, thickness, 0f, halfHeight + (thickness / 2f));
        CreateWall(spanX, thickness, 0f, -halfHeight - (thickness / 2f));
        CreateWall(thickness, spanY, -halfWidth - (thickness / 2f), 0f);
        CreateWall(thickness, spanY, halfWidth + (thickness / 2f), 0f);
    }

    private void CreateWall(float width, float height, float x, float y)
    {
        var wall = _world!.CreateRectangle(width, height, 1f, new Vector2(x, y), 0f, BodyType.Static);
        ApplyMaterial(wall);
    }

    /// <summary>
    /// Builds the collision outline for a die, scaled from the shared unit outline to the
    /// configured die size.
    /// </summary>
    /// <param name="sides">The number of faces on the die.</param>
    /// <returns>The vertices describing the die's convex body.</returns>
    private Vertices BuildVertices(int sides)
    {
        var outline = DieGeometry.GetOutline(sides);
        var vertices = new Vertices(outline.Count);

        foreach (var point in outline)
        {
            // DieGeometry works in System.Numerics, while Aether has its own vector type.
            vertices.Add(new Vector2(point.X * _settings.DieSize, point.Y * _settings.DieSize));
        }

        return vertices;
    }

    private Body CreateBody(VisualDie die, int index, int count)
    {
        // The body is built from the same outline the renderer traces, so a die can never
        // collide as one shape while appearing as another.
        var body = _world!.CreatePolygon(
            BuildVertices(die.Sides),
            _settings.DieDensity,
            GetStartPosition(index, count),
            NextFloat(-MathF.PI, MathF.PI),
            BodyType.Dynamic);

        ApplyMaterial(body);
        body.LinearDamping = _settings.LinearDamping;
        body.AngularDamping = _settings.AngularDamping;
        body.SleepingAllowed = true;

        body.LinearVelocity = new Vector2(
            NextFloat(-_settings.MaxLaunchSpeed, _settings.MaxLaunchSpeed),
            NextFloat(-_settings.MaxLaunchSpeed, _settings.MaxLaunchSpeed));
        body.AngularVelocity = NextFloat(-_settings.MaxLaunchSpin, _settings.MaxLaunchSpin);

        return body;
    }

    private void ApplyMaterial(Body body)
    {
        foreach (var fixture in body.FixtureList)
        {
            fixture.Friction = _settings.Friction;
            fixture.Restitution = _settings.Restitution;
        }
    }

    private Vector2 GetStartPosition(int index, int count)
    {
        // Lay the dice out on a grid inside the tray so they do not start overlapped,
        // which would otherwise resolve into extreme separation velocities.
        var columns = Math.Max(1, (int)MathF.Ceiling(MathF.Sqrt(count)));
        var rows = (int)MathF.Ceiling(count / (float)columns);
        var spacingX = _traySize.Width / (columns + 1);
        var spacingY = _traySize.Height / (rows + 1);

        return new Vector2(
            (-_traySize.Width / 2f) + (spacingX * ((index % columns) + 1)),
            (-_traySize.Height / 2f) + (spacingY * ((index / columns) + 1)));
    }

    private bool AllDiceAtRest() =>
        _dice.TrueForAll(d =>
            d.Body.LinearVelocity.Length() <= _settings.LinearSleepThreshold &&
            Math.Abs(d.Body.AngularVelocity) <= _settings.AngularSleepThreshold);

    private void FreezeDice()
    {
        foreach (var die in _dice)
        {
            die.Body.LinearVelocity = Vector2.Zero;
            die.Body.AngularVelocity = 0f;
        }
    }

    /// <summary>
    /// Normalizes a body's linear and angular speed into a single 0-to-1 presentation hint.
    /// The faster of the two contributions wins, so a die that is spinning in place still
    /// reads as being in motion.
    /// </summary>
    private float CalculateEnergy(Body body)
    {
        var linear = _settings.MaxLaunchSpeed > 0f
            ? body.LinearVelocity.Length() / _settings.MaxLaunchSpeed
            : 0f;

        var angular = _settings.MaxLaunchSpin > 0f
            ? Math.Abs(body.AngularVelocity) / _settings.MaxLaunchSpin
            : 0f;

        var energy = Math.Max(linear, angular);

        return float.IsFinite(energy) ? Math.Clamp(energy, 0f, 1f) : 0f;
    }

    private float NextFloat(float min, float max) => min + ((float)_random.NextDouble() * (max - min));

    private sealed record SimulatedDie(VisualDie Die, Body Body);
}
