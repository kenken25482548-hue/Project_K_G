using UnityEngine;

public static class GameProgress
{
    // บันทึกว่าผ่านด่านไหนแล้ว
    public static void UnlockLevel(int levelIndex)
    {
        int current = PlayerPrefs.GetInt("UnlockedLevel", 1);
        if (levelIndex + 1 > current)
        {
            PlayerPrefs.SetInt("UnlockedLevel", levelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    // เช็คว่าด่านนี้เล่นได้ไหม
    public static bool IsUnlocked(int levelIndex)
    {
        int unlocked = PlayerPrefs.GetInt("UnlockedLevel", 1);
        return levelIndex <= unlocked;
    }

    // รีเซ็ตทั้งหมด
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey("UnlockedLevel");
        PlayerPrefs.Save();
    }
}