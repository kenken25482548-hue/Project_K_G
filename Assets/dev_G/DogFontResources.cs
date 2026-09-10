using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>Provides dynamic TextMesh Pro fonts using the bundled DOG font files.</summary>
public static class DogFontResources
{
    private static readonly Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();
    private const string ThaiCharacters =
        "กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮ" +
        "ะัาำิีึืฺุู็่้๊๋์ํๅเแโใไ";
    private const string ThaiConsonants = "กขฃคฅฆงจฉชซฌญฎฏฐฑฒณดตถทธนบปผฝพฟภมยรลวศษสหฬอฮ";
    private const string UpperMarks = "ัิีึื็ํ";
    private const string ToneMarks = "่้๊๋์";

    public static TMP_FontAsset Load(string weight = "Regular")
    {
        if (weight != "Regular" && weight != "Light" && weight != "Bold")
            weight = "Regular";

        if (Fonts.TryGetValue(weight, out TMP_FontAsset cached) && cached != null)
            return cached;

        string path = "UI/Fonts/iannnnn-DOG-" + weight;
        TMP_FontAsset font = Resources.Load<TMP_FontAsset>(path + " SDF");
        if (font == null)
        {
            Font source = Resources.Load<Font>(path);
            if (source == null)
            {
                Debug.LogWarning("DOG font missing: Resources/" + path);
                return null;
            }

            font = TMP_FontAsset.CreateFontAsset(source, 90, 9,
                GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
            if (font == null) return null;
            font.name = "iannnnn-DOG-" + weight + " SDF";
        }

        ApplyThaiMarkAdjustments(font);
        Fonts[weight] = font;
        return font;
    }

    // TMP does not apply the Thai OpenType mark-to-mark positioning in this font.
    // Raise a tone mark after an upper vowel so combinations such as "ิ่" remain legible.
    private static void ApplyThaiMarkAdjustments(TMP_FontAsset font)
    {
        if (font == null || font.fontFeatureTable == null) return;

        font.TryAddCharacters(ThaiCharacters, out _);
        foreach (char consonant in ThaiConsonants)
        {
            if (!TryGetGlyphIndex(font, consonant, out uint consonantGlyph)) continue;

            foreach (char upperMark in UpperMarks)
            {
                if (TryGetGlyphIndex(font, upperMark, out uint upperGlyph))
                    UpsertPairAdjustment(font, consonantGlyph, upperGlyph, 0f, 2f);
            }

            foreach (char toneMark in ToneMarks)
            {
                if (TryGetGlyphIndex(font, toneMark, out uint toneGlyph))
                    UpsertPairAdjustment(font, consonantGlyph, toneGlyph, 0f, 8f);
            }
        }

        foreach (char upperMark in UpperMarks)
        {
            if (!TryGetGlyphIndex(font, upperMark, out uint upperGlyph)) continue;

            foreach (char toneMark in ToneMarks)
            {
                if (!TryGetGlyphIndex(font, toneMark, out uint toneGlyph)) continue;
                UpsertPairAdjustment(font, upperGlyph, toneGlyph, -4f, 28f);
            }
        }
    }

    private static bool TryGetGlyphIndex(TMP_FontAsset font, char character, out uint glyphIndex)
    {
        foreach (TMP_Character entry in font.characterTable)
        {
            if (entry.unicode != character) continue;
            glyphIndex = entry.glyphIndex;
            return true;
        }

        glyphIndex = 0;
        return false;
    }

    private static void UpsertPairAdjustment(TMP_FontAsset font, uint firstGlyph, uint secondGlyph,
        float xPlacement, float yPlacement)
    {
        List<GlyphPairAdjustmentRecord> pairs = font.fontFeatureTable.glyphPairAdjustmentRecords;
        pairs.RemoveAll(pair =>
            pair.firstAdjustmentRecord.glyphIndex == firstGlyph &&
            pair.secondAdjustmentRecord.glyphIndex == secondGlyph);

        GlyphAdjustmentRecord first = new GlyphAdjustmentRecord(firstGlyph, new GlyphValueRecord());
        GlyphAdjustmentRecord second = new GlyphAdjustmentRecord(secondGlyph, new GlyphValueRecord());
        GlyphValueRecord placement = second.glyphValueRecord;
        placement.xPlacement = xPlacement;
        placement.yPlacement = yPlacement;
        second.glyphValueRecord = placement;
        pairs.Add(new GlyphPairAdjustmentRecord(first, second));
    }
}
