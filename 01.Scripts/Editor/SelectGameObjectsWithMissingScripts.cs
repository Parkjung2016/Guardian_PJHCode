using UnityEditor;
using UnityEngine;

public class SelectGameObjectsWithMissingScripts : Editor
{
    [MenuItem("Utility/Remove Missing Script")]
    private static void RemoveAllMissingScriptComponents()
    {
        var selectedGameObjects = Selection.gameObjects;
        int totalComponentCount = 0;
        int totalGameObjectCount = 0;

        foreach (var gameObject in selectedGameObjects)
        {
            int missingScriptCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(gameObject);

            if (missingScriptCount > 0)
            {
                Undo.RegisterCompleteObjectUndo(gameObject, "Remove Missing Scripts");
                GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

                totalComponentCount += missingScriptCount;
                totalGameObjectCount++;
            }
        }

        Debug.Log($"Removed {totalComponentCount} missing script component(s) from {totalGameObjectCount} game object(s).");
    }
}