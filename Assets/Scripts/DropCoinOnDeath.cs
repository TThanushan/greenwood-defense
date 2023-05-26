using UnityEngine;

public class DropCoinOnDeath : MonoBehaviour
{
    public GameObject coinPrefab;
    public int numberOfCoins = 5;
    public float dropRadius = 2f;
    public Transform playerTransform; // reference to the player
    public float forceStrength = 10f; // strength of the force applied to the coins


    private void Start()
    {
        GetComponent<Unit>().OnDeath += Die;
        playerTransform = PoolObject.instance.playerCaptain.transform;
    }
    public void Die()
    {
        for (int i = 0; i < numberOfCoins; i++)
        {
            Vector2 dropPosition = transform.position + (Vector3)(Random.insideUnitCircle * dropRadius);
            GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            Rigidbody2D coinRb = coin.GetComponent<Rigidbody2D>();
            Vector2 forceDirection = (playerTransform.position - transform.position).normalized;
            coinRb.AddForce(forceDirection * forceStrength, ForceMode2D.Impulse);
        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, dropRadius);
    }
}
