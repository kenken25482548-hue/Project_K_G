using UnityEngine;

public struct MissionStoryData
{
    public string sceneName;
    public string missionNumber;
    public string chapter;
    public string englishRoom;
    public string thaiRoom;
    public string briefing;
    public string objective;
    public string recoveredMessage;
}

/// <summary>Single source of truth for the mission order, story, and completion state.</summary>
public static class MissionStoryCatalog
{
    private static readonly MissionStoryData[] Missions =
    {
        new MissionStoryData
        {
            sceneName = "1bathroom1", missionNumber = "01", chapter = "THE FIRST TRACE",
            englishRoom = "BATHROOM", thaiRoom = "ห้องน้ำ",
            briefing = "22:47 — ระบบบ้านแจ้งเตือนความชื้นผิดปกติ ทั้งที่ไม่มีใครใช้งานห้องนี้มาหลายชั่วโมง\n\nคราบที่พบไม่ได้เกิดจากอุบัติเหตุธรรมดา เหมือนมีใครพยายามลบร่องรอยบางอย่างออกไป.",
            objective = "สำรวจให้ครบ เลือกไอเทมให้ถูก และกู้บันทึกแรกของบ้าน.",
            recoveredMessage = "บันทึกที่กู้ได้: “อย่าใช้สิ่งที่แรงที่สุด ถ้ายังไม่รู้ว่ากำลังล้างอะไร”"
        },
        new MissionStoryData
        {
            sceneName = "2Kitchen2", missionNumber = "02", chapter = "AFTER HOURS",
            englishRoom = "KITCHEN", thaiRoom = "ห้องครัว",
            briefing = "ประตูครัวปลดล็อกหลังจากกู้บันทึกแรกสำเร็จ แต่เซนเซอร์ยังพบร่องรอยอาหารและสารตกค้างที่ไม่ควรอยู่รวมกัน.\n\nระบบเริ่มสงสัยว่าเจ้าของบ้านกำลังเรียนรู้เรื่องเดียวกับคุณ — ด้วยวิธีที่เสี่ยงกว่า.",
            objective = "ตามหาคราบทุกจุดและหยุดการปนเปื้อนก่อนที่ข้อมูลจะหายไป.",
            recoveredMessage = "บันทึกที่กู้ได้: “ความสะอาดไม่ได้วัดจากกลิ่นแรง แต่วัดจากการเลือกที่เหมาะสม”"
        },
        new MissionStoryData
        {
            sceneName = "3iving room3", missionNumber = "03", chapter = "THE QUIET ROOM",
            englishRoom = "LIVING ROOM", thaiRoom = "ห้องนั่งเล่น",
            briefing = "เมื่อครัวสะอาด ไฟในห้องนั่งเล่นกลับติดขึ้นเอง ภาพจากกล้องบันทึกเสียงการสนทนาที่ถูกตัดหายไปเกือบทั้งหมด.\n\nเหลือเพียงประโยคสุดท้าย: “ถ้าเลือกผิดอีกครั้ง ทุกอย่างจะกลับมาเหมือนเดิม”",
            objective = "เคลียร์คราบให้หมด เพื่อกู้ข้อความที่ขาดหายจากศูนย์กลางของบ้าน.",
            recoveredMessage = "บันทึกที่กู้ได้: “ทุกคราบมีวิธีจัดการของมัน เหมือนทุกปัญหามีคำตอบที่ไม่ใช่ทางลัด”"
        },
        new MissionStoryData
        {
            sceneName = "4bedroom4", missionNumber = "04", chapter = "THE FINAL ROOM",
            englishRoom = "BEDROOM", thaiRoom = "ห้องนอน",
            briefing = "ห้องสุดท้ายเก็บข้อมูลทั้งหมดไว้ เบาะแสยืนยันว่าคราบทุกจุดคือแบบฝึกของระบบบ้าน เพื่อทดสอบว่าคนที่เข้ามาจะเลือกแก้ปัญหาด้วยความเข้าใจหรือความรีบร้อน.\n\nครั้งนี้ การตัดสินใจของคุณคือคำตอบสุดท้าย.",
            objective = "จัดการทุกคราบอย่างถูกวิธี แล้วปิดแฟ้มคดี CLEAN & LEARN.",
            recoveredMessage = "แฟ้มคดีสมบูรณ์: คุณพิสูจน์แล้วว่า ‘เลือกให้ถูก’ สำคัญพอ ๆ กับ ‘ทำให้สะอาด’"
        }
    };

    public static bool TryGet(string sceneName, out MissionStoryData mission)
    {
        for (int i = 0; i < Missions.Length; i++)
        {
            if (Missions[i].sceneName == sceneName)
            {
                mission = Missions[i];
                return true;
            }
        }

        mission = default;
        return false;
    }

    public static int GetIndex(string sceneName)
    {
        for (int i = 0; i < Missions.Length; i++)
            if (Missions[i].sceneName == sceneName) return i;
        return -1;
    }
}
