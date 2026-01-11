namespace DiceCli.Models;

internal class FavoriteRollsDocument
{
    public string Version { get; set; } = "1.0";

    public HashSet<FavoriteRoll> Rolls { get; init; } = [];
}
