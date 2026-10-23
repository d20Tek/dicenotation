using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

namespace D20Tek.DiceNotation.PhysicsSpike;

/// <summary>
/// Minimal Aether.Physics2D prototype: a rectangular tray with static walls and
/// square dynamic dice launched with randomized positions and velocities.
/// Each die carries a fixed, precomputed value that the simulation never changes.
/// </summary>
internal sealed class DiceTraySimulation : IDisposable
{
    private readonly SpikeSettings _settings;
    private readonly Random _random;
    private readonly List<(string Id, int Value, Body Body)> _dice = [];
    private World _world;
    private float _elapsedSeconds;
    private float _restingSeconds;
    private bool _disposed;

    public DiceTraySimulation(SpikeSettings settings, Random random)
    {
        _settings = settings;
        _random = random;
        _world = new World(Vector2.Zero);
    }

    /// <summary>Gets a value indicating whether every die has come to rest.</summary>
    public bool IsSettled => _restingSeconds >= _settings.SettleHoldSeconds;

    /// <summary>Gets a value indicating whether the maximum animation duration elapsed.</summary>
    public bool IsTimedOut => _elapsedSeconds >= _settings.MaxDurationSeconds;

    /// <summary>Gets the total simulated time of the current roll, in seconds.</summary>
    public float ElapsedSeconds => _elapsedSeconds;

    /// <summary>
    /// Resets the world and launches the supplied dice values into the tray.
    /// </summary>
    /// <param name="values">The precomputed die values to animate.</param>
    public void StartRoll(IReadOnlyList<int> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        _world.Clear();
        _dice.Clear();
        _elapsedSeconds = 0f;
        _restingSeconds = 0f;

        CreateWalls();

        for (var i = 0; i < values.Count; i++)
        {
            var body = CreateDie(i, values.Count);
            _dice.Add(($"die-{i}", values[i], body));
        }
    }

    /// <summary>Advances the simulation by one fixed timestep.</summary>
    public void Step()
    {
        var step = _settings.FixedStep;
        _world.Step(step);
        _elapsedSeconds += step;

        _restingSeconds = AllDiceAtRest() ? _restingSeconds + step : 0f;
    }

    /// <summary>Captures the current renderable state of the scene.</summary>
    /// <returns>A snapshot of every die plus settling information.</returns>
    public SceneSnapshot GetSnapshot()
    {
        var dice = new DieSnapshot[_dice.Count];
        for (var i = 0; i < _dice.Count; i++)
        {
            var (id, value, body) = _dice[i];
            dice[i] = new DieSnapshot(id, value, body.Position.X, body.Position.Y, body.Rotation);
        }

        return new SceneSnapshot(dice, _elapsedSeconds, IsSettled);
    }

    /// <summary>Determines whether every die is inside the tray bounds.</summary>
    /// <returns><c>true</c> when no die has escaped the tray.</returns>
    public bool AllDiceInsideTray()
    {
        var halfWidth = (_settings.TrayWidth / 2f) + _settings.DieSize;
        var halfHeight = (_settings.TrayHeight / 2f) + _settings.DieSize;

        return _dice.TrueForAll(d =>
            Math.Abs(d.Body.Position.X) <= halfWidth && Math.Abs(d.Body.Position.Y) <= halfHeight);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _world.Clear();
        _dice.Clear();
        _world = null!;
        _disposed = true;
    }

    private void CreateWalls()
    {
        var halfWidth = _settings.TrayWidth / 2f;
        var halfHeight = _settings.TrayHeight / 2f;
        var thickness = _settings.WallThickness;

        CreateWall(_settings.TrayWidth + (thickness * 2f), thickness, 0f, halfHeight + (thickness / 2f));
        CreateWall(_settings.TrayWidth + (thickness * 2f), thickness, 0f, -halfHeight - (thickness / 2f));
        CreateWall(thickness, _settings.TrayHeight + (thickness * 2f), -halfWidth - (thickness / 2f), 0f);
        CreateWall(thickness, _settings.TrayHeight + (thickness * 2f), halfWidth + (thickness / 2f), 0f);
    }

    private void CreateWall(float width, float height, float x, float y)
    {
        var wall = _world.CreateRectangle(width, height, 1f, new Vector2(x, y), 0f, BodyType.Static);
        ApplyMaterial(wall);
    }

    private Body CreateDie(int index, int count)
    {
        var position = GetStartPosition(index, count);
        var body = _world.CreateRectangle(
            _settings.DieSize,
            _settings.DieSize,
            _settings.DieDensity,
            position,
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

    private Vector2 GetStartPosition(int index, int count)
    {
        // Lay the dice out on a grid inside the tray so they do not start overlapped.
        var columns = Math.Max(1, (int)MathF.Ceiling(MathF.Sqrt(count)));
        var rows = (int)MathF.Ceiling(count / (float)columns);
        var spacingX = _settings.TrayWidth / (columns + 1);
        var spacingY = _settings.TrayHeight / (rows + 1);

        var column = index % columns;
        var row = index / columns;

        return new Vector2(
            (-_settings.TrayWidth / 2f) + (spacingX * (column + 1)),
            (-_settings.TrayHeight / 2f) + (spacingY * (row + 1)));
    }

    private void ApplyMaterial(Body body)
    {
        foreach (var fixture in body.FixtureList)
        {
            fixture.Friction = _settings.Friction;
            fixture.Restitution = _settings.Restitution;
        }
    }

    private bool AllDiceAtRest() =>
        _dice.TrueForAll(d =>
            d.Body.LinearVelocity.Length() <= _settings.LinearSleepThreshold &&
            Math.Abs(d.Body.AngularVelocity) <= _settings.AngularSleepThreshold);

    private float NextFloat(float min, float max) =>
        min + ((float)_random.NextDouble() * (max - min));
}
