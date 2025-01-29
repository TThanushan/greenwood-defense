using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DynamicSortingOrder : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // This calculation ensures that objects lower on the screen are drawn in front.
        // You might need to adjust the multiplier (-100) based on the size of your game world.
        spriteRenderer.sortingOrder = (int)(transform.position.y * -100);
    }
}
