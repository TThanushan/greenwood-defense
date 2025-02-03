using UnityEditor;
using UnityEngine;

public class PasteAsChild : EditorWindow
{
    private static GameObject copiedObject;

    [MenuItem("GameObject/Copy as Child", false, 10)]
    private static void CopyAsChild()
    {
        if (Selection.activeGameObject != null)
        {
            copiedObject = Selection.activeGameObject;
            Debug.Log($"Copied: {copiedObject.name}");
        }
        else
        {
            Debug.LogWarning("No GameObject selected to copy.");
        }
    }

    [MenuItem("GameObject/Paste as Child", false, 11)]
    private static void PasteAsChildObjects()
    {
        if (copiedObject == null)
        {
            Debug.LogWarning("No object copied. Use 'Copy as Child' first.");
            return;
        }

        GameObject[] selectedObjects = Selection.gameObjects;
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("No parent objects selected.");
            return;
        }

        foreach (GameObject parent in selectedObjects)
        {
            GameObject newChild = Instantiate(copiedObject, parent.transform);
            newChild.name = copiedObject.name;  // Maintain the original name
        }

        Debug.Log($"Pasted {copiedObject.name} as child of {selectedObjects.Length} objects.");
    }
}
