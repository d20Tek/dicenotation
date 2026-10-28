using d20Tek.DiceNotation;
using d20Tek.DiceNotation.Blazor.Components;
using d20Tek.DiceNotation.Blazor.Models;
using d20Tek.DiceNotation.Results;
using Microsoft.AspNetCore.Components;

namespace BlazorDice.Pages;

/// <summary>
/// Hosts the sample's dice roller, driving the public <see cref="VisualDiceRoller"/>
/// component with results the application rolls for itself.
/// </summary>
public partial class Home : ComponentBase
{
    private const int MaxHistoryEntries = 25;

    private const string DiceTermType = "DiceTerm";

    private static readonly DiceTheme[] Themes =
    [
        DiceTheme.Default,
        DiceTheme.DarkFantasy,
        DiceTheme.Parchment,
    ];

    private static readonly (string Notation, string Description)[] Samples =
    [
        ("4d6k3+2", "Ability score roll keeping the highest three dice"),
        ("1d20+5", "Single attack roll with a modifier"),
        ("2d6+1d8+1d4", "Mixed die types in one roll"),
        ("1d2", "Coin flip"),
        ("3d100", "Percentile dice"),
        ("2d6++", "Invalid notation, shows the error path"),
    ];

    private readonly List<HistoryEntry> _history = [];

    private VisualDiceRoller? _roller;
    private DiceResult? _result;
    private DiceResult? _pendingResult;
    private DiceRollSummary? _summary;
    private string _notation = "4d6k3+2";
    private string? _error;
    private bool _reducedMotion;
    private bool _isRolling;
    private string _themeName = DiceTheme.Default.Name;
    private OutputTab _activeTab = OutputTab.Dice;
    private bool _resizeOnNextRender;
    private int _rollCount;

    [Inject]
    private IDice Dice { get; set; } = default!;

    private DiceTheme SelectedTheme =>
        Themes.FirstOrDefault(t => t.Name == _themeName) ?? DiceTheme.Default;

    /// <inheritdoc />
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_resizeOnNextRender || _roller is null) return;

        _resizeOnNextRender = false;
        await _roller.ResizeAsync();
    }

    private void SelectTab(OutputTab tab)
    {
        if (_activeTab == tab) return;

        _activeTab = tab;

        // A hidden canvas has no bounding box, so it measures as zero while the history
        // tab is shown. Resize once it is visible again to restore the backing buffer.
        _resizeOnNextRender = tab == OutputTab.Dice;

        if (tab != OutputTab.Dice || _pendingResult is null) return;

        // Only the newest roll made while the tab was hidden is animated; the earlier
        // ones were already recorded in the history and are skipped.
        _result = _pendingResult;
        _pendingResult = null;
        _isRolling = true;
    }

    private void ClearHistory()
    {
        _history.Clear();
        _rollCount = 0;
    }

    private void AddHistoryEntry(DiceResult result)
    {
        var dice = result.Results
            .Where(r => r.Type.Contains(DiceTermType))
            .Select(r => $"{r.Type[(r.Type.LastIndexOf('.') + 1)..]}:{r.Value}");

        // Newest first, capped so a long session does not grow without bound.
        _history.Insert(
            0,
            new HistoryEntry(
                ++_rollCount,
                DateTime.Now,
                result.DiceExpression,
                string.Join(", ", dice),
                result.Value));

        if (_history.Count > MaxHistoryEntries)
        {
            _history.RemoveAt(_history.Count - 1);
        }
    }

    private void UseSample(string notation)
    {
        _notation = notation;
        RollAsync();
    }

    private void RollAsync()
    {
        _error = null;
        _summary = null;
        _isRolling = true;

        var result = Dice.Roll(_notation);
        if (result.HasError)
        {
            _error = result.Error;
            _isRolling = false;
            return;
        }

        // The history is driven by the roll itself, never by the animation, so results
        // show up immediately even when the dice tab is hidden.
        AddHistoryEntry(result);

        if (_activeTab != OutputTab.Dice)
        {
            // Nothing is visible, so skip the animation and keep only the latest roll
            // to animate when the user returns to the dice tab.
            _pendingResult = result;
            _isRolling = false;
            return;
        }

        _result = result;
    }

    private async Task ReplayAsync()
    {
        if (_roller is null) return;

        _isRolling = true;
        await _roller.ReplayAsync();
    }

    private void OnCompleted(DiceRollSummary summary)
    {
        _summary = summary;
        _isRolling = false;
    }

    private void OnFailed(Exception exception)
    {
        _error = $"The animation failed: {exception.Message}";
        _isRolling = false;
    }

    private enum OutputTab
    {
        Dice,
        History,
    }

    private sealed record HistoryEntry(
        int Number,
        DateTime RolledAt,
        string Expression,
        string Dice,
        int Total);
}
