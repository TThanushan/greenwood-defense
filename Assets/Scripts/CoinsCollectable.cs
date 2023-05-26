using UnityEngine;

public class CoinsCollectable : MonoBehaviour
{

    public float pickupDistance = 1f;
    public Transform playerTransform;
    Vector2 startPos;

    private void Start()
    {
        playerTransform = PoolObject.instance.playerCaptain.transform;
        startPos = transform.position;
    }
    private void Update()
    {
        if (Vector2.Distance(transform.position, playerTransform.position) <= pickupDistance)
        {
            PickUp();
        }
        if (transform.position.y < startPos.y)
            transform.GetComponent<Rigidbody2D>().simulated = false;
    }

    public void PickUp()
    {
        // Add coin to player's inventory
        // Destroy the coin
        gameObject.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, pickupDistance);
    }
}

