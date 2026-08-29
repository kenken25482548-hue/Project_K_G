#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>Editor-only helper that lists room surfaces before stain placement.</summary>
public static class SceneStainPlacementUtility
{
    [MenuItem("Clean & Learn/Replace Selected With Bedroom Wall Makeup Stain")]
    public static void ReplaceSelectedWithBedroomWallMakeupStain()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null || selected.GetComponent<CleaningTarget>() == null)
        {
            Debug.LogError("Select the stain root you want to replace first.");
            return;
        }

        const string prefabPath = "Assets/CleaningSystem/LevelAdditions/Bedroom/ADD_Stain_Bedroom_MakeupSmudge_Wall.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError("Missing wall makeup stain prefab: " + prefabPath);
            return;
        }

        Transform oldTransform = selected.transform;
        Transform parent = oldTransform.parent;
        Vector3 position = oldTransform.position;
        Quaternion rotation = oldTransform.rotation;
        Scene scene = selected.scene;

        Undo.RegisterFullObjectHierarchyUndo(selected, "Replace Makeup Stain");
        GameObject replacement = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        Undo.RegisterCreatedObjectUndo(replacement, "Replace Makeup Stain");
        replacement.transform.SetParent(parent, true);
        replacement.transform.SetPositionAndRotation(position, rotation);
        replacement.name = "Stain_Bedroom_MakeupSmudge_Wall";
        Undo.DestroyObjectImmediate(selected);

        Selection.activeGameObject = replacement;
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Replaced selected stain with wall makeup stain at the same position.");
    }

    [MenuItem("Clean & Learn/Remove Placed Mission Stains")]
    public static void RemovePlacedMissionStains()
    {
        Remove("Assets/Scenes/2Kitchen2.unity", "GameplayStains_Level02");
        Remove("Assets/Scenes/3iving room3.unity", "GameplayStains_Level03");
        Remove("Assets/Scenes/4bedroom4.unity", "GameplayStains_Level04");

        AssetDatabase.SaveAssets();
        Debug.Log("Placed mission stains removed: Kitchen, Living Room, Bedroom.");
    }

    [MenuItem("Clean & Learn/Place Mission Stains")]
    public static void PlaceMissionStains()
    {
        Place("Assets/Scenes/2Kitchen2.unity", "GameplayStains_Level02", new[]
        {
            new StainPlacement("Assets/CleaningSystem/KitchenStains/Stain_Kitchen_InsectTrail_Corner.prefab", new Vector3(-3.52f, 0.52f, 3.70f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/KitchenStains/Stain_Kitchen_GreasyDishResidue_Counter.prefab", new Vector3(-3.52f, 1.40f, 4.25f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/KitchenStains/Stain_Kitchen_GreaseSplash_Backsplash.prefab", new Vector3(1.38f, 1.50f, 4.43f), Quaternion.FromToRotation(Vector3.down, Vector3.forward)),
            new StainPlacement("Assets/CleaningSystem/KitchenStains/Stain_Kitchen_DriedFood_Sink.prefab", new Vector3(4.58f, 1.73f, 4.35f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/KitchenStains/Stain_Kitchen_DirtyWater_Cabinet.prefab", new Vector3(-0.41f, 1.40f, -0.34f), Quaternion.identity)
        });

        Place("Assets/Scenes/3iving room3.unity", "GameplayStains_Level03", new[]
        {
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_CoffeeSpill_Sofa.prefab", new Vector3(-1.47f, 0.99f, 1.92f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_DrinkSplash_TVCabinet.prefab", new Vector3(-0.75f, 0.99f, -0.45f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_DustHair_SofaCorner.prefab", new Vector3(-0.76f, 0.99f, 0.52f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_Fingerprint_GlassTable.prefab", new Vector3(2.23f, 1.59f, -0.49f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_GlassRing_CoffeeTable.prefab", new Vector3(2.38f, 1.59f, -0.08f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/LivingRoomStains/Stain_Living_MuddyFootprints_Floor.prefab", new Vector3(-0.35f, 0.52f, -3.70f), Quaternion.identity)
        });

        Place("Assets/Scenes/4bedroom4.unity", "GameplayStains_Level04", new[]
        {
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_ChocolateSmear_Bed.prefab", new Vector3(-3.40f, 1.22f, 0.02f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_DustHair_UnderBed.prefab", new Vector3(-3.40f, 0.52f, -0.95f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_DustyBareFootprints_Door.prefab", new Vector3(4.24f, 0.52f, 2.76f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_DustyHandprint_Wardrobe.prefab", new Vector3(-0.10f, 1.05f, 4.38f), Quaternion.FromToRotation(Vector3.down, Vector3.forward)),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_Fingerprint_DressingMirror.prefab", new Vector3(3.42f, 1.02f, -4.36f), Quaternion.FromToRotation(Vector3.down, Vector3.back)),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_RedDrinkSpill_Chair.prefab", new Vector3(4.06f, 1.22f, -4.19f), Quaternion.identity),
            new StainPlacement("Assets/CleaningSystem/BedroomStains/Stain_Bedroom_WaterRing_BedsideTable.prefab", new Vector3(-4.99f, 1.28f, -2.05f), Quaternion.identity)
        });

        AssetDatabase.SaveAssets();
        Debug.Log("Mission stains placed: Kitchen 5, Living Room 6, Bedroom 7.");
    }

    [MenuItem("Clean & Learn/Set All Game Fonts To Kanit")]
    public static void SetAllGameFontsToKanit()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Resources/UI/Fonts/Kanit-SemiBold SDF.asset");
        if (font == null)
        {
            Debug.LogError("Kanit SemiBold font asset is missing.");
            return;
        }

        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });
        foreach (string sceneGuid in sceneGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(sceneGuid);
            Scene scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            bool changed = false;
            foreach (TMP_Text text in UnityEngine.Object.FindObjectsByType<TMP_Text>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (text.font == font)
                    continue;

                text.font = font;
                changed = true;
            }

            if (changed)
            {
                EditorSceneManager.MarkSceneDirty(scene);
                EditorSceneManager.SaveScene(scene);
            }
        }

        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" });
        foreach (string prefabGuid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            if (path.StartsWith("Assets/TextMesh Pro/") || path.StartsWith("Assets/StarterAssets/"))
                continue;

            GameObject root = PrefabUtility.LoadPrefabContents(path);
            bool changed = false;
            foreach (TMP_Text text in root.GetComponentsInChildren<TMP_Text>(true))
            {
                if (text.font == font)
                    continue;

                text.font = font;
                changed = true;
            }

            if (changed)
                PrefabUtility.SaveAsPrefabAsset(root, path);

            PrefabUtility.UnloadPrefabContents(root);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Kanit SemiBold applied to every game scene and relevant prefab.");
    }

    [MenuItem("Clean & Learn/Audit Mission Modes")]
    public static void AuditMissionModes()
    {
        string[] scenePaths =
        {
            "Assets/Scenes/1bathroom1.unity",
            "Assets/Scenes/2Kitchen2.unity",
            "Assets/Scenes/3iving room3.unity",
            "Assets/Scenes/4bedroom4.unity"
        };

        const string challengeKey = "ChallengeMode";
        bool hadChallengeSetting = PlayerPrefs.HasKey(challengeKey);
        int originalChallengeSetting = PlayerPrefs.GetInt(challengeKey, 0);

        try
        {
            foreach (bool challenge in new[] { false, true })
            {
                PlayerPrefs.SetInt(challengeKey, challenge ? 1 : 0);
                PlayerPrefs.Save();
                Debug.Log("=== MISSION AUDIT / " + (challenge ? "CHALLENGE" : "NORMAL") + " ===");

                foreach (string scenePath in scenePaths)
                    AuditScene(scenePath);
            }
        }
        finally
        {
            if (hadChallengeSetting)
                PlayerPrefs.SetInt(challengeKey, originalChallengeSetting);
            else
                PlayerPrefs.DeleteKey(challengeKey);
            PlayerPrefs.Save();
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }
    }

    private static void AuditScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        LevelFlowManager flow = UnityEngine.Object.FindFirstObjectByType<LevelFlowManager>();
        if (flow == null)
        {
            Debug.LogError("[Mission Audit] " + scenePath + " : LevelFlowManager is missing.");
            return;
        }

        MissionDifficultyController controller = flow.GetComponent<MissionDifficultyController>();
        bool addedController = controller == null;
        if (addedController)
            controller = flow.gameObject.AddComponent<MissionDifficultyController>();

        controller.Configure(flow);

        CleaningTarget[] stains = UnityEngine.Object.FindObjectsByType<CleaningTarget>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        ItemData[] items = UnityEngine.Object.FindObjectsByType<ItemData>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        List<string> issues = new List<string>();

        foreach (CleaningTarget stain in stains)
        {
            if (string.IsNullOrWhiteSpace(stain.stainName))
                issues.Add(stain.name + " has no stain name");
            if (string.IsNullOrWhiteSpace(stain.requiredItemName))
                issues.Add(stain.name + " has no required item");
            if (stain.dirtObject == null)
                issues.Add(stain.name + " has no dirt visual");
            if (stain.GetComponent<InteractionZone>() == null)
                issues.Add(stain.name + " has no interaction zone");

            bool matchingItemExists = false;
            foreach (ItemData item in items)
            {
                if (item != null && item.itemName == stain.requiredItemName)
                {
                    matchingItemExists = true;
                    break;
                }
            }
            if (!matchingItemExists)
                issues.Add(stain.name + " needs missing item '" + stain.requiredItemName + "'");
        }

        MissionLevelData level = controller.CurrentLevel;
        if (stains.Length != level.stainTarget)
            issues.Add("HUD target " + level.stainTarget + " but active stains " + stains.Length);

        string result = "[Mission Audit] " + SceneManager.GetActiveScene().name + " / " + level.difficulty
            + " : active stains=" + stains.Length + ", target=" + level.stainTarget + ", active items=" + items.Length;
        if (issues.Count == 0)
            Debug.Log(result + " / PASS");
        else
            Debug.LogWarning(result + " / ISSUES: " + string.Join(" | ", issues));

        if (addedController)
            UnityEngine.Object.DestroyImmediate(controller);
    }

    private static void Place(string scenePath, string rootName, StainPlacement[] placements)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject oldRoot = GameObject.Find(rootName);
        if (oldRoot != null)
            UnityEngine.Object.DestroyImmediate(oldRoot);

        GameObject root = new GameObject(rootName);
        foreach (StainPlacement placement in placements)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(placement.prefabPath);
            if (prefab == null)
            {
                Debug.LogError("Missing stain prefab: " + placement.prefabPath);
                continue;
            }

            GameObject stain = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
            stain.transform.SetParent(root.transform, true);
            stain.transform.SetPositionAndRotation(placement.position, placement.rotation);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }

    private static void Remove(string scenePath, string rootName)
    {
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject root = GameObject.Find(rootName);
        if (root != null)
        {
            UnityEngine.Object.DestroyImmediate(root);
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private readonly struct StainPlacement
    {
        public readonly string prefabPath;
        public readonly Vector3 position;
        public readonly Quaternion rotation;

        public StainPlacement(string prefabPath, Vector3 position, Quaternion rotation)
        {
            this.prefabPath = prefabPath;
            this.position = position;
            this.rotation = rotation;
        }
    }

    public static void ListRoomSurfaces()
    {
        string[] scenes =
        {
            "Assets/Scenes/2Kitchen2.unity",
            "Assets/Scenes/3iving room3.unity",
            "Assets/Scenes/4bedroom4.unity"
        };

        foreach (string path in scenes)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Debug.Log("=== " + path + " ===");
            Renderer[] renderers = UnityEngine.Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            foreach (Renderer renderer in renderers)
            {
                Bounds bounds = renderer.bounds;
                if (bounds.size.sqrMagnitude < 0.01f) continue;
                Debug.Log($"SURFACE | {renderer.name} | pos={renderer.transform.position} | size={bounds.size}");
            }
        }
    }

    public static void CountMissionStains()
    {
        string[] scenes =
        {
            "Assets/Scenes/2Kitchen2.unity",
            "Assets/Scenes/3iving room3.unity",
            "Assets/Scenes/4bedroom4.unity"
        };
        foreach (string path in scenes)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            CleaningTarget[] targets = UnityEngine.Object.FindObjectsByType<CleaningTarget>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Debug.Log("TARGET COUNT | " + path + " | " + targets.Length);
            foreach (CleaningTarget target in targets)
                Debug.Log("TARGET | " + target.name + " | " + target.stainName + " | item=" + target.requiredItemName + " | pos=" + target.transform.position);
        }
    }

    public static void ListRoomObjects()
    {
        string[] scenes =
        {
            "Assets/Scenes/2Kitchen2.unity",
            "Assets/Scenes/3iving room3.unity",
            "Assets/Scenes/4bedroom4.unity"
        };

        foreach (string path in scenes)
        {
            EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            Debug.Log("=== OBJECTS " + path + " ===");
            foreach (GameObject obj in UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (obj.transform.parent == null || obj.GetComponent<Renderer>() != null || obj.GetComponent<CleaningTarget>() != null)
                    Debug.Log($"OBJECT | {GetPath(obj.transform)} | pos={obj.transform.position} | scale={obj.transform.lossyScale}");
            }
            foreach (ItemData item in UnityEngine.Object.FindObjectsByType<ItemData>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                Debug.Log($"ITEM | {item.name} | itemName={item.itemName} | pos={item.transform.position}");
        }
    }

    private static string GetPath(Transform transform)
    {
        string path = transform.name;
        while (transform.parent != null)
        {
            transform = transform.parent;
            path = transform.name + "/" + path;
        }
        return path;
    }
}
#endif
