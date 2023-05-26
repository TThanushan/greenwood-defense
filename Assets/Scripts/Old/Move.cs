using UnityEngine;

public class Move : MonoBehaviour
{
    public float moveSpeedX = 1;
    public float moveSpeedY = 0;

    public int wayX = 1;
    public int wayY = 1;

    void Update()
    {
        transform.Translate(new Vector2(moveSpeedX * wayX * Time.deltaTime, moveSpeedY * wayY * Time.deltaTime));
    }
}
