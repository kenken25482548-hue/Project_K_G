using UnityEngine;

/// <summary>Keeps the four mission levels unlocked in order.</summary>
public static class GameProgress
{
    private const string RankPrefix = "MissionRank_";
    private const string EvidencePrefix = "Evidence_";
    private const string ChallengeModeKey = "ChallengeMode";

    public static bool IsChallengeMode => PlayerPrefs.GetInt(ChallengeModeKey, 0) == 1;
    public static void UnlockLevel(int levelIndex)
    {
        int current = PlayerPrefs.GetInt("UnlockedLevel", 0);
        if (levelIndex + 1 > current)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    public static bool IsUnlocked(int levelIndex)
    {
        return levelIndex <= PlayerPrefs.GetInt("UnlockedLevel", 0);
    }

    public static void MarkLevelCompleted(int levelIndex)
    {
        int completed = PlayerPrefs.GetInt("CompletedLevel", -1);
        if (levelIndex > completed)
        {
            PlayerPrefs.SetInt("CompletedLevel", levelIndex);
            PlayerPrefs.Save();
        }
    }

    public static bool IsCompleted(int levelIndex)
    {
        return levelIndex <= PlayerPrefs.GetInt("CompletedLevel", -1);
    }

    public static int CompletedCount(int totalLevels)
    {
        int count = 0;
        for (int index = 0; index < totalLevels; index++)
            if (IsCompleted(index)) count++;
        return count;
    }

    public static int GetBestRank(int levelIndex)
    {
        return PlayerPrefs.GetInt(RankPrefix + levelIndex, 0);
    }

    public static int RecordRank(int levelIndex, int rank)
    {
        int best = Mathf.Max(GetBestRank(levelIndex), Mathf.Clamp(rank, 1, 3));
        PlayerPrefs.SetInt(RankPrefix + levelIndex, best);
        PlayerPrefs.Save();
        return best;
    }

    public static void UnlockEvidence(int levelIndex)
    {
        PlayerPrefs.SetInt(EvidencePrefix + levelIndex, 1);
        PlayerPrefs.Save();
    }

    public static bool HasEvidence(int levelIndex)
    {
        return PlayerPrefs.GetInt(EvidencePrefix + levelIndex, 0) == 1;
    }

    public static bool CanUseChallengeMode()
    {
        return CompletedCount(4) >= 4;
    }

    public static void SetChallengeMode(bool enabled)
    {
        if (!CanUseChallengeMode()) return;
        PlayerPrefs.SetInt(ChallengeModeKey, enabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.DeleteKey("CompletedLevel");
        PlayerPrefs.DeleteKey("PlayerXP"); // Cleanup from the retired XP prototype.
        PlayerPrefs.DeleteKey(ChallengeModeKey);
        for (int index = 0; index < 4; index++)
        {
            PlayerPrefs.DeleteKey(RankPrefix + index);
            PlayerPrefs.DeleteKey(EvidencePrefix + index);
        }
        PlayerPrefs.Save();
    }
}
