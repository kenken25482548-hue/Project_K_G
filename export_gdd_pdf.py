from pathlib import Path
import re
from docx import Document
from docx.table import Table as DocxTable
from docx.text.paragraph import Paragraph
from docx.oxml.ns import qn
from reportlab.lib import colors
from reportlab.lib.enums import TA_CENTER, TA_LEFT
from reportlab.lib.pagesizes import A4
from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
from reportlab.lib.units import mm
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import SimpleDocTemplate, Paragraph as RLParagraph, Spacer, Table, TableStyle, PageBreak, KeepTogether

ROOT = Path(__file__).resolve().parent
INPUT = ROOT / "Documentation" / "Clean_and_Learn_GDD_Showcase_TH.docx"
OUTPUT = ROOT / "Documentation" / "Clean_and_Learn_GDD_Showcase_TH.pdf"
FONT = r"C:\Windows\Fonts\LeelawUI.ttf"
FONT_BOLD = r"C:\Windows\Fonts\LEELAWDB.TTF"
NAVY, CYAN, PALE, GRID = "#10364F", "#3FAFD0", "#EAF5F9", "#D9D9D9"

def ordered_blocks(parent):
    body = parent.element.body
    for child in body.iterchildren():
        if child.tag == qn("w:p"):
            yield Paragraph(child, parent)
        elif child.tag == qn("w:tbl"):
            yield DocxTable(child, parent)

def esc(text):
    return (text or "").replace("→", " - ").replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")

def add_page(canvas, doc):
    canvas.saveState()
    if doc.page > 1:
        canvas.setFont("Leelaw", 8)
        canvas.setFillColor(colors.HexColor("#666666"))
        canvas.drawRightString(A4[0] - 18 * mm, A4[1] - 13 * mm, "CLEAN AND LEARN  |  GAME DESIGN DOCUMENT")
    canvas.setFont("Leelaw", 8)
    canvas.setFillColor(colors.HexColor("#777777"))
    canvas.drawCentredString(A4[0] / 2, 11 * mm, f"Clean and Learn  |  Unity 6  |  {doc.page}")
    canvas.restoreState()

def main():
    pdfmetrics.registerFont(TTFont("Leelaw", FONT))
    pdfmetrics.registerFont(TTFont("Leelaw-Bold", FONT_BOLD))
    d = Document(INPUT)
    styles = getSampleStyleSheet()
    body = ParagraphStyle("body", parent=styles["BodyText"], fontName="Leelaw", fontSize=10.4, leading=15, spaceAfter=5, textColor=colors.black)
    title = ParagraphStyle("title", parent=body, fontName="Leelaw-Bold", fontSize=30, leading=36, alignment=TA_CENTER, textColor=colors.HexColor(NAVY), spaceAfter=8)
    subtitle = ParagraphStyle("subtitle", parent=body, fontName="Leelaw-Bold", fontSize=15, leading=21, alignment=TA_CENTER, textColor=colors.HexColor(CYAN), spaceAfter=14)
    center = ParagraphStyle("center", parent=body, alignment=TA_CENTER, textColor=colors.HexColor("#333333"))
    heading = ParagraphStyle("heading", parent=body, fontName="Leelaw-Bold", fontSize=16, leading=22, textColor=colors.HexColor(NAVY), spaceBefore=12, spaceAfter=5, keepWithNext=True)
    label = ParagraphStyle("label", parent=body, fontName="Leelaw-Bold", fontSize=11, leading=15, textColor=colors.HexColor(CYAN), spaceBefore=5, spaceAfter=4)
    small = ParagraphStyle("small", parent=body, fontName="Leelaw", fontSize=8.7, leading=11.5)
    cover_count = 0
    story = []
    skip_break_after_stain_table = False
    for block in ordered_blocks(d):
        if isinstance(block, Paragraph):
            text = block.text.strip()
            if 'w:type="page"' in block._p.xml:
                if skip_break_after_stain_table:
                    skip_break_after_stain_table = False
                else:
                    story.append(PageBreak())
                continue
            if not text:
                continue
            if text == "Clean and Learn":
                story.append(Spacer(1, 63 * mm)); story.append(RLParagraph(esc(text), title)); cover_count += 1; continue
            if text == "GAME DESIGN DOCUMENT":
                story.append(RLParagraph(esc(text), subtitle)); cover_count += 1; continue
            if re.match(r"^\d+\. ", text):
                story.append(RLParagraph(esc(text), heading)); continue
            if text in ("DESIGN FOCUS",):
                story.append(RLParagraph(esc(text), label)); continue
            if text.startswith("Core Concept:") or text.startswith("Current Build Summary:"):
                strong = ParagraphStyle("strong", parent=body, fontName="Leelaw-Bold", textColor=colors.HexColor(NAVY), spaceBefore=7)
                story.append(RLParagraph(esc(text), strong)); continue
            if text.startswith("•") or block.style.name == "List Bullet":
                story.append(RLParagraph("- " + esc(text[1:].strip() if text.startswith("•") else text), body)); continue
            story.append(RLParagraph(esc(text), center if cover_count and len(story) < 6 else body))
        else:
            data = []
            for row in block.rows:
                data.append([RLParagraph(esc(c.text), small) for c in row.cells])
            if not data: continue
            avail = A4[0] - 36 * mm
            cols = len(data[0]); widths = [avail / cols] * cols
            if cols == 2: widths = [avail * .27, avail * .73]
            if cols == 3: widths = [avail * .18, avail * .54, avail * .28]
            if cols == 5: widths = [avail * .09, avail * .17, avail * .46, avail * .14, avail * .14]
            t = Table(data, colWidths=widths, repeatRows=1, hAlign="CENTER")
            style = [("GRID", (0,0), (-1,-1), .35, colors.HexColor(GRID)), ("VALIGN", (0,0), (-1,-1), "MIDDLE"), ("LEFTPADDING", (0,0), (-1,-1), 6), ("RIGHTPADDING", (0,0), (-1,-1), 6), ("TOPPADDING", (0,0), (-1,-1), 5), ("BOTTOMPADDING", (0,0), (-1,-1), 5), ("BACKGROUND", (0,0), (-1,0), colors.HexColor(NAVY)), ("TEXTCOLOR", (0,0), (-1,0), colors.white)]
            for r in range(1, len(data)):
                if r % 2 == 0: style.append(("BACKGROUND", (0,r), (-1,r), colors.HexColor(PALE)))
            t.setStyle(TableStyle(style)); story.append(t); story.append(Spacer(1, 5 * mm))
            if "แมลงวันบินในห้องครัว" in " ".join(c.text for row in block.rows for c in row.cells):
                skip_break_after_stain_table = True
    doc = SimpleDocTemplate(str(OUTPUT), pagesize=A4, leftMargin=18*mm, rightMargin=18*mm, topMargin=21*mm, bottomMargin=18*mm, title="Clean and Learn Game Design Document", author="Clean and Learn Team")
    doc.build(story, onFirstPage=add_page, onLaterPages=add_page)

if __name__ == "__main__": main()
