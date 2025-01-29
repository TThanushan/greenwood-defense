using UnityEngine;

public class DynamicSegmentedHealthBar : MonoBehaviour
{
    public int maxHealth = 100;
    public GameObject healthSegmentPrefab;
    public float segmentWidth = 5f;  // Adjust this width based on your segment image's width
    [SerializeField] private float offset;
    void Update()
    {
        CreateHealthBar();
    }

    void CreateHealthBar()
    {
        // Clear the existing health segments
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        RectTransform containerRect = GetComponent<RectTransform>();

        // Calculate the starting position
        float startX = (-containerRect.rect.width / 2) + (segmentWidth / 2) + offset;

        for (int i = 0; i < maxHealth / 10; i++)
        {
            GameObject segment = Instantiate(healthSegmentPrefab, transform);
            RectTransform rt = segment.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + (i * segmentWidth), 0); // Position each segment
            //rt.localScale = Vector3.one;  // Ensure the scale is set to 1
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        int segmentCount = transform.childCount;
        for (int i = 0; i < segmentCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i < currentHealth / 10);
        }
    }
}

