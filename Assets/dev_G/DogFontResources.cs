using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>Provides dynamic TextMesh Pro fonts using the bundled DOG font files.</summary>
public static class DogFontResources
{
    private static readonly Dictionary<string, TMP_FontAsset> Fonts = new Dictionary<string, TMP_FontAsset>();

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

        Fonts[weight] = font;
        return font;
    }
}
