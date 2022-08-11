using UnityEngine;

public class MoveTowardTarget : MonoBehaviour
{
    public GameObject target;
    public bool moveX = true;
    public bool moveY = true;
    public float xOffSet;
    public float yOffSet;

    private void OnEnable()
    {
        MoveToward();
    }

    private void Update()
    {
        if (!target.activeSelf || target.GetComponent<Unit>() && target.GetComponent<Unit>().Disabled)
            gameObject.SetActive(false);
        MoveToward();

    }

    void MoveToward()
    {
        if (!target) return;
        //Vector2 dir = target.transform.position - transform.position;
        Vector2 dir = new Vector2(target.transform.position.x - xOffSet, target.transform.position.y - yOffSet);
        if (!moveX)
            dir.x = transform.position.x;
        if (!moveY)
            dir.y = transform.position.y;

        //dir.x += xOffSet;
        //dir.y += yOffSet;

        //transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);
        transform.position = dir;
    }
}
