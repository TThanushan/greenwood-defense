using UnityEngine;

[ExecuteInEditMode]
public class SpaceChildrenOnGrid : MonoBehaviour
{
    public float xSpacing = 1.0f;
    public float ySpacing = 1.0f;

    void OnValidate()
    {
        int numChildren = transform.childCount;

        for (int i = 0; i < numChildren; i++)
        {
            Transform child = transform.GetChild(i);
            float posX = i * xSpacing;
            float posY = i * ySpacing;
            child.localPosition = new Vector3(posX, posY, 0);
        }
    }
}


