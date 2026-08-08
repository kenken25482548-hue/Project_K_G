public struct MissionLevelData
{
    public int number;
    public string difficulty;
    public string challenge;
    public int stainTarget;
    public int decoyItemCount;
    public int maxWrongUses;

    public MissionLevelData(int number, string difficulty, string challenge, int stainTarget, int decoyItemCount, int maxWrongUses)
    {
        this.number = number;
        this.difficulty = difficulty;
        this.challenge = challenge;
        this.stainTarget = stainTarget;
        this.decoyItemCount = decoyItemCount;
        this.maxWrongUses = maxWrongUses;
    }
}

/// <summary>Defines the intended difficulty curve for the four playable missions.</summary>
public static class MissionLevelCatalog
{
    public static MissionLevelData GetByIndex(int levelIndex)
    {
        switch (levelIndex)
        {
            case 0: return Get("1bathroom1");
            case 1: return Get("2Kitchen2");
            case 2: return Get("3iving room3");
            case 3: return Get("4bedroom4");
            default: return Get("1bathroom1");
        }
    }

    public static MissionLevelData Get(string sceneName)
    {
        switch (sceneName)
        {
            case "1bathroom1": return new MissionLevelData(1, "EASY", "GUIDED SEARCH", 3, 0, 3);
            case "2Kitchen2": return new MissionLevelData(2, "NORMAL", "MIXED TOOLS", 5, 1, 2);
            case "3iving room3": return new MissionLevelData(3, "HARD", "LIMITED MISTAKES", 6, 2, 2);
            case "4bedroom4": return new MissionLevelData(4, "EXPERT", "FINAL CLEANUP", 7, 99, 1);
            default: return new MissionLevelData(1, "EASY", "GUIDED SEARCH", 3, 0, 3);
        }
    }
}
