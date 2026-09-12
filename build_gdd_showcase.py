from pathlib import Path
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_CELL_VERTICAL_ALIGNMENT
from docx.oxml import OxmlElement
from docx.oxml.ns import qn

OUT = Path("Documentation/Clean_and_Learn_GDD_Showcase_TH.docx")
NAVY, CYAN, PALE, GRID = "10364F", "3FAFD0", "EAF5F9", "D9D9D9"

def font(run, size=10.5, bold=False, color="000000"):
    run.font.name = "Aptos"
    run._element.rPr.rFonts.set(qn("w:ascii"), "Aptos")
    run._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos")
    run._element.rPr.rFonts.set(qn("w:eastAsia"), "Noto Sans Thai")
    run.font.size, run.bold, run.font.color.rgb = Pt(size), bold, RGBColor.from_string(color)

def shade(cell, color):
    el = OxmlElement("w:shd"); el.set(qn("w:fill"), color)
    cell._tc.get_or_add_tcPr().append(el)

def border(cell):
    props = cell._tc.get_or_add_tcPr(); b = OxmlElement("w:tcBorders")
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        e = OxmlElement(f"w:{edge}"); e.set(qn("w:val"), "single"); e.set(qn("w:sz"), "5"); e.set(qn("w:color"), GRID); b.append(e)
    props.append(b)

def cell_pad(cell):
    props = cell._tc.get_or_add_tcPr(); m = OxmlElement("w:tcMar")
    for side in ("top", "start", "bottom", "end"):
        e = OxmlElement(f"w:{side}"); e.set(qn("w:w"), "115"); e.set(qn("w:type"), "dxa"); m.append(e)
    props.append(m)

def p(doc, text="", size=10.5, bold=False, color="000000", align=None, before=0, after=6):
    para = doc.add_paragraph()
    para.paragraph_format.space_before, para.paragraph_format.space_after = Pt(before), Pt(after)
    para.paragraph_format.line_spacing = 1.16
    if align is not None: para.alignment = align
    font(para.add_run(text), size, bold, color)
    return para

def heading(doc, title, level=1):
    para = doc.add_paragraph()
    para.paragraph_format.space_before, para.paragraph_format.space_after = Pt(14 if level == 1 else 8), Pt(5)
    font(para.add_run(title), 16 if level == 1 else 12, True, NAVY)
    return para

def bullets(doc, items):
    for item in items:
        para = doc.add_paragraph(style="List Bullet")
        para.paragraph_format.space_after = Pt(3); para.paragraph_format.line_spacing = 1.1
        font(para.add_run(item), 10.5)

def table(doc, headers, rows, widths):
    t = doc.add_table(rows=1, cols=len(headers)); t.style = "Table Grid"; t.alignment = WD_TABLE_ALIGNMENT.CENTER
    for i, name in enumerate(headers):
        c = t.rows[0].cells[i]; c.text = ""; c.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.CENTER
        font(c.paragraphs[0].add_run(name), 9.5, True, "FFFFFF"); shade(c, NAVY); border(c); cell_pad(c); c.width = Inches(widths[i]); c.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    for r, values in enumerate(rows):
        cells = t.add_row().cells
        for i, value in enumerate(values):
            c = cells[i]; c.text = ""; c.paragraphs[0].alignment = WD_ALIGN_PARAGRAPH.LEFT
            font(c.paragraphs[0].add_run(str(value)), 9.2); shade(c, "FFFFFF" if r % 2 == 0 else PALE); border(c); cell_pad(c); c.width = Inches(widths[i]); c.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
    doc.add_paragraph().paragraph_format.space_after = Pt(1)

def new_page(doc): doc.add_page_break()

def main():
    OUT.parent.mkdir(parents=True, exist_ok=True)
    d = Document(); s = d.sections[0]
    s.top_margin, s.bottom_margin, s.left_margin, s.right_margin = Inches(.72), Inches(.66), Inches(.8), Inches(.8)
    d.styles["Normal"].font.size = Pt(10.5)
    h = s.header.paragraphs[0]; h.alignment = WD_ALIGN_PARAGRAPH.RIGHT; font(h.add_run("CLEAN AND LEARN  |  GAME DESIGN DOCUMENT"), 8, True, "666666")
    f = s.footer.paragraphs[0]; f.alignment = WD_ALIGN_PARAGRAPH.CENTER; font(f.add_run("Clean and Learn  |  Unity 6"), 8, False, "777777")

    title = d.add_paragraph(style="Title"); title.alignment = WD_ALIGN_PARAGRAPH.CENTER; title.paragraph_format.space_before, title.paragraph_format.space_after = Pt(112), Pt(11); font(title.add_run("Clean and Learn"), 31, True, NAVY)
    p(d, "GAME DESIGN DOCUMENT", 15, True, CYAN, WD_ALIGN_PARAGRAPH.CENTER, after=18)
    p(d, "เกมพัซเซิลทำความสะอาดมุมมองบุคคลที่หนึ่ง", 13, False, "000000", WD_ALIGN_PARAGRAPH.CENTER, after=8)
    p(d, "Unity 6  |  Windows PC  |  Single Player", 10.5, False, "555555", WD_ALIGN_PARAGRAPH.CENTER, after=28)
    p(d, "เอกสารนี้อธิบายการออกแบบ Build ที่เล่นได้จริง: การค้นหาคราบ การเลือกน้ำยาที่ถูกต้อง ภารกิจ 4 ห้อง และระบบความยาก 2 โหมด", 10.5, False, "333333", WD_ALIGN_PARAGRAPH.CENTER, after=0)
    new_page(d)

    heading(d, "1. GAME OVERVIEW")
    p(d, "Clean and Learn คือเกมพัซเซิลทำความสะอาดที่ให้ผู้เล่นสำรวจห้อง ค้นหาคราบ อ่านข้อมูล เลือกน้ำยาหรืออุปกรณ์ให้ตรงกับคราบ และเคลียร์เป้าหมายทั้งหมดโดยไม่ใช้ไอเทมผิดเกินจำนวนที่กำหนด")
    table(d, ["หมวด", "รายละเอียด"], [
        ("Genre", "First Person Cleaning Puzzle"), ("Platform", "Windows PC"), ("Engine", "Unity 6"), ("ผู้เล่น", "Single Player"), ("เป้าหมาย", "กำจัดคราบ Active ทุกจุดในห้อง"), ("ลำดับด่าน", "Bathroom → Kitchen → Living Room → Bedroom"), ("โหมด", "Normal Mode และ Challenge Mode")], [1.45, 5.2])
    heading(d, "2. DESIGN GOAL")
    p(d, "เป้าหมายคือเปลี่ยนการทำความสะอาดให้เป็นการตัดสินใจ ผู้เล่นต้องสังเกตข้อมูลของคราบ เชื่อมโยงกับไอเทมที่มี และยอมรับผลของการเดา ไม่ใช่เพียงกดโต้ตอบให้วัตถุหายไป")
    p(d, "DESIGN FOCUS", 11, True, CYAN, before=4)
    bullets(d, ["Observation ก่อน Interaction", "ความสัมพันธ์ที่ชัดเจนระหว่างคราบกับไอเทม", "ความยากเพิ่มตามจำนวนคราบ พื้นที่ และความหลากหลาย", "HUD และเสียงตอบกลับสถานะของผู้เล่นทันที"])
    p(d, "Core Concept: Observe the clue. Choose the cleaner. Clear the room.", 10.5, True, NAVY, after=0)
    new_page(d)

    heading(d, "3. CORE GAMEPLAY LOOP")
    p(d, "Explore → Observe → Interact → Experiment → Solve → Progress", 13, True, CYAN, after=9)
    table(d, ["ขั้น", "สิ่งที่ผู้เล่นทำ", "ผลตอบกลับของเกม"], [
        ("Explore", "เดินสำรวจห้องเพื่อหาคราบและไอเทม", "การจัดวางในฉากและ beacon นำสายตา"),
        ("Observe", "ตรวจข้อมูลคราบและคำใบ้", "หน้าต่างข้อมูลแสดงชื่อ คำอธิบาย และสถานะ"),
        ("Interact", "เก็บและเลือกไอเทมจาก Inventory", "HUD แสดงช่องไอเทมและชิ้นที่เลือก"),
        ("Experiment", "ใช้ไอเทมกับคราบที่พบ", "เสียงและผลลัพธ์ต่างกันระหว่างใช้ถูก/ผิด"),
        ("Solve", "กำจัดคราบ Active ทุกจุด", "Stain Files และ Errors Left อัปเดตทันที"),
        ("Progress", "จบด่านและปลดล็อกภารกิจถัดไป", "ผลลัพธ์ด่านบันทึกความก้าวหน้า")], [0.95, 2.8, 2.9])
    heading(d, "4. PLAYER CONTROLS AND INTERACTION")
    table(d, ["Input", "การทำงาน"], [("W A S D", "เคลื่อนที่"), ("Mouse", "หมุนกล้อง"), ("Space", "กระโดด"), ("E", "ดูข้อมูลคราบหรือไอเทม"), ("F", "เก็บไอเทมหรือใช้น้ำยาที่เลือก"), ("1 - 5", "เลือกช่อง Inventory"), ("Esc", "พักเกมหรือปิดหน้าต่าง")], [1.45, 5.2])
    new_page(d)

    heading(d, "5. PUZZLE DESIGN")
    p(d, "พัซเซิลของเกมเริ่มจากคราบที่ผู้เล่นยังไม่รู้วิธีแก้ ผู้เล่นต้องหาข้อมูลและทดลองอย่างมีเหตุผล การใช้ไอเทมผิดจะไม่ทำให้แพ้ทันที แต่จะลดจำนวน Errors Left และสร้างแรงกดดันใน Challenge Mode")
    table(d, ["โครงสร้างพัซเซิล", "หน้าที่"], [
        ("Problem", "พบคราบ แต่ยังไม่รู้ไอเทมที่ถูกต้อง"), ("Observation", "อ่านข้อความและสังเกตตำแหน่งหรือประเภทคราบ"), ("Exploration", "ค้นหาไอเทมที่เหมาะสมในพื้นที่"), ("Experiment", "เลือกและใช้ไอเทมกับคราบ"), ("Consequence", "ใช้ถูกคราบหาย ใช้ผิดเสียโอกาสหนึ่งครั้ง"), ("Solution", "เคลียร์คราบ Active ครบตามเป้าหมาย")], [1.7, 4.95])
    heading(d, "6. STAIN AND ITEM SYSTEM")
    p(d, "Cleaning Target เก็บชื่อคราบ คำอธิบาย ไอเทมที่ต้องใช้ สถานะการพบ และสถานะการเคลียร์ คราบจะถูกเลือกผ่านระยะ Interaction และ Visual จะค่อย ๆ หายเมื่อผู้เล่นใช้อุปกรณ์ที่ถูกต้อง ตารางนี้รวบรวมคราบและน้ำยาของทุกห้อง เพื่อให้การจับคู่ในเกมชัดเจน")
    table(d, ["ห้อง", "คราบหรือเป้าหมาย", "น้ำยาหรืออุปกรณ์ที่ใช้"], [
        ("Bathroom", "คราบเหลืองสะสมในโถส้วม", "น้ำยาล้างห้องน้ำ"),
        ("Bathroom", "คราบสบู่และหินปูนขอบอ่าง", "น้ำยาล้างห้องน้ำ"),
        ("Bathroom", "คราบฝุ่นและเส้นผมตามมุมพื้น", "น้ำยาถูพื้น"),
        ("Bathroom", "รอยเท้าเปื้อนบนพื้น", "น้ำยาถูพื้น"),
        ("Bathroom", "คราบน้ำสกปรกกระเด็นบนตู้", "น้ำ"),
        ("Kitchen", "คราบน้ำมันกระเด็นหลังเตา", "น้ำยาขจัดคราบไขมัน"),
        ("Kitchen", "คราบไหม้และไขมันเกาะเตา", "น้ำยาขจัดคราบไขมัน"),
        ("Kitchen", "คราบจานมันบนเคาน์เตอร์", "น้ำยาล้างจาน"),
        ("Kitchen", "คราบเศษอาหารแห้งในอ่างล้างจาน", "น้ำยาล้างจาน"),
        ("Kitchen", "คราบน้ำสกปรกไหลบนตู้ครัว", "น้ำ"),
        ("Kitchen", "กลุ่มแมลงและร่องรอยตามมุมครัว", "ยาไล่แมลง"),
        ("Kitchen", "แมลงวันบินในห้องครัว (เพิ่ม)", "ยาฆ่าแมลง"),
        ("Living Room", "คราบมือและรอยปาดบนหน้าต่าง", "น้ำยาเช็ดกระจก"),
        ("Living Room", "รอยนิ้วมือมันบนโต๊ะกระจก", "น้ำยาเช็ดกระจก"),
        ("Living Room", "รอยรองเท้าเปื้อนโคลน", "น้ำยาถูพื้น"),
        ("Living Room", "คราบฝุ่นและเส้นผมใต้โซฟา", "น้ำยาถูพื้น"),
        ("Living Room", "คราบกาแฟหกบนโซฟา", "น้ำ"),
        ("Living Room", "คราบน้ำดื่มกระเด็นบนตู้ทีวี", "น้ำ"),
        ("Living Room", "รอยแก้วน้ำบนโต๊ะกลาง", "น้ำ"),
        ("Bedroom", "คราบฝุ่นและเส้นผมใต้เตียง", "น้ำยาถูพื้น"),
        ("Bedroom", "รอยเท้าเปื้อนฝุ่นใกล้ประตู", "น้ำยาถูพื้น"),
        ("Bedroom", "คราบช็อกโกแลตบนผ้าห่ม", "น้ำ"),
        ("Bedroom", "คราบน้ำหวานหกบนเก้าอี้", "น้ำ"),
        ("Bedroom", "รอยแก้วน้ำบนโต๊ะข้างเตียง", "น้ำ"),
        ("Bedroom", "รอยนิ้วมือบนกระจกแต่งตัว", "น้ำยาเช็ดกระจก"),
        ("Bedroom", "คราบเครื่องสำอางบนกระจก", "น้ำยาเช็ดกระจก"),
        ("Bedroom", "คราบเครื่องสำอางเลอะกำแพง", "น้ำยาเช็ดกระจก")], [1.2, 3.55, 1.9])
    new_page(d)

    heading(d, "7. LEVEL DESIGN AND PROGRESSION")
    p(d, "ด่านเพิ่มความยากอย่างค่อยเป็นค่อยไปด้วยจำนวนคราบ Active ที่มากขึ้นและพื้นที่ที่ซับซ้อนขึ้น จำนวนในตารางเป็นคราบที่เกิดจริงขณะเล่นในแต่ละโหมด")
    table(d, ["Mission", "Room", "การเรียนรู้", "Normal", "Challenge"], [
        ("01", "Bathroom", "พื้นฐานการตรวจคราบและจับคู่ไอเทม", "3 คราบ", "4 คราบ"), ("02", "Kitchen", "คราบมัน เศษอาหาร และร่องรอยแมลง", "5 คราบ", "6 คราบ"), ("03", "Living Room", "สำรวจพื้นที่กว้างและพื้นผิวหลายชนิด", "6 คราบ", "7 คราบ"), ("04", "Bedroom", "ความรู้ผสมหลายประเภทคราบ", "7 คราบ", "8 คราบ")], [.62, 1.15, 2.75, .9, .95])
    heading(d, "8. GAME MODES")
    table(d, ["Mode", "ประสบการณ์", "ความแตกต่าง"], [
        ("Normal Mode", "แนะนำตรรกะการทำความสะอาด ให้ผู้เล่นเรียนรู้จากการเล่น", "จำนวนคราบน้อยกว่า และรับความผิดพลาดได้มากกว่า"),
        ("Challenge Mode", "ทดสอบความรู้เดิมภายใต้แรงกดดันที่มากขึ้น", "เพิ่มคราบจริง 1 จุดต่อห้อง และจำกัด Errors Left เข้มงวดขึ้น")], [1.45, 2.9, 2.45])
    new_page(d)

    heading(d, "9. UI AND PLAYER FEEDBACK")
    table(d, ["องค์ประกอบ", "หน้าที่"], [
        ("Mission HUD", "แสดง Stain Files ความคืบหน้า คะแนน และ Errors Left"), ("Inventory HUD", "แสดงไอเทม 5 ช่องและไอเทมที่เลือกอยู่"), ("Stain Information Panel", "แสดงชื่อคราบ คำอธิบาย คำใบ้ และสถานะ"), ("Mission Brief", "แจ้งเป้าหมายของห้องก่อนเริ่มภารกิจ"), ("Level Select", "เลือกห้องและโหมด Normal หรือ Challenge"), ("Pause Guide", "ทบทวนวิธีควบคุมและเป้าหมายระหว่างเล่น")], [2.15, 4.65])
    heading(d, "10. AUDIO FEEDBACK")
    p(d, "เสียงช่วยยืนยันผลของการตัดสินใจ ผู้เล่นได้ยินเสียงเฉพาะเมื่อใช้น้ำยาถูกและเสียงอีกแบบเมื่อใช้น้ำยาผิด เพื่อให้เข้าใจผลลัพธ์โดยไม่ต้องพึ่งข้อความเพียงอย่างเดียว")
    bullets(d, ["UseCorrect.mp3 - เล่นเมื่อใช้น้ำยาถูก", "UseWrong.mp3 - เล่นเมื่อใช้น้ำยาผิด", "เสียง Hover และ Click ของเมนู", "ปรับระดับเสียงเพลงและ SFX แยกกันได้"])
    heading(d, "11. UNITY DEVELOPMENT")
    table(d, ["System", "หน้าที่ในการพัฒนา"], [
        ("CleaningTarget", "จัดการข้อมูลคราบ การตรวจสอบไอเทม และ Visual fade"), ("PlayerItemSystem", "เก็บไอเทม เลือกช่อง Inventory และสั่งใช้"), ("MissionDifficultyController", "เปิดชุดคราบตาม Normal หรือ Challenge"), ("LevelFlowManager", "จัดการข้อมูลด่าน การโหลด และความก้าวหน้า"), ("GameSFXManager", "เรียกใช้เสียงสำหรับการทำความสะอาดถูกและผิด")], [2.1, 4.7])
    p(d, "Current Build Summary: เกมมี 4 ห้อง ภารกิจต่อเนื่อง ระบบคราบและไอเทม HUD การเลือกโหมด และ Feedback เสียง โดยจำนวนใน HUD ถูกออกแบบให้ตรงกับคราบ Active ที่เกิดจริงของแต่ละด่าน", 10.5, True, NAVY, before=8, after=0)
    d.core_properties.title = "Clean and Learn Game Design Document"
    d.core_properties.subject = "Game design document"
    d.core_properties.author = "Clean and Learn Team"
    d.save(OUT)

if __name__ == "__main__": main()
