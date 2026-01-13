namespace DiceCli.Models;

internal sealed class FavoriteRoll
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Expression { get; private set; } = string.Empty;

    public FavoriteRoll(string id, string name, string expression)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(id);
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(expression);

        Id = id;
        Name = name;
        Expression = expression;
    }

    public void Update(string name, string expression)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name);
        ArgumentNullException.ThrowIfNullOrEmpty(expression);
        Name = name;
        Expression = expression;
    }
}
