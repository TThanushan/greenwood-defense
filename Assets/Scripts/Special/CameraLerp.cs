using UnityEngine;

public class CameraLerp : MonoBehaviour
{
    private Vector3 offset = new Vector3(0f, 0f, -10f);
    private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    PoolObject poolObject;
    // Use this for initialization
    void Start()
    {
        poolObject = PoolObject.instance;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTarget();
        if (!target)
            return;
        Vector3 targetPosition = target.position + offset;
        Vector3 newPos = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
    }

    void UpdateTarget()
    {

        GameObject newTarget = GetFurthestAlly();
        if (newTarget)
            target = newTarget.transform;
    }


    GameObject GetFurthestAlly()
    {
        float minX = -2f;
        float furthestX = Mathf.NegativeInfinity;
        GameObject newTarget = null;
        GameObject[] allies = poolObject.Allies;
        if (allies == null || allies.Length <= 0)
            return null;
        foreach (GameObject ally in allies)
        {
            if (ally.name.Equals("PlayerCaptain"))
                continue;
            float posX = ally.transform.position.x;
            if (posX > furthestX && posX > minX)
            {
                furthestX = posX;
                newTarget = ally;
            }
        }

        return newTarget;
    }
}
