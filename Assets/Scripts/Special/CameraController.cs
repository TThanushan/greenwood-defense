using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    // CameraLerp variables
    public Vector3 offset = new Vector3(0.5f, 0f, -10f);
    public float smoothTime = 0.8f;
    public float slowSmoothTime = 1.25f;
    float initialSmoothTime;
    Vector3 velocity = Vector3.zero;
    [SerializeField] private Transform target;
    PoolObject poolObject;

    // ShakeCamera variables
    public static CameraController instance;
    public float intensity;
    public float duration;
    public float fadeSpeed;
    public float enemyDistanceThreshold;


    float startTime;
    float t;
    public float durationRemaining;
    bool isShaking;
    bool fading = false;
    Vector3 initialPosition;
    Vector3 lastNonShakingPosition;
    PlayerCaptainUnit PlayerCaptainUnit;

    void Awake()
    {
        if (instance == null)
            instance = this;
    }

    void Start()
    {
        poolObject = PoolObject.instance;
        PlayerCaptainUnit = poolObject.playerCaptain.gameObject.GetComponent<PlayerCaptainUnit>();
        initialSmoothTime = smoothTime;
    }

    void Update()
    {
        if (!isShaking)
        {
            UpdateCameraPosition();
            initialPosition = transform.position;
        }

        if (isShaking)
        {
            durationRemaining -= Time.deltaTime;
            Vector3 shakeOffset = new Vector3(Random.Range(-intensity, intensity), Random.Range(-intensity, intensity), 0);

            Vector3 newPosition = initialPosition + shakeOffset;
            newPosition.x = Mathf.Clamp(newPosition.x, 0, 3.0687f); // Clamp the x-axis position after shaking

            // Apply the clamped shake offset
            shakeOffset.x = newPosition.x - initialPosition.x;
            transform.position = initialPosition + shakeOffset;

            SlowDownShaking();
        }
    }

    private void LateUpdate()
    {
        if (!isShaking) // Add this condition to prevent clamping during shaking
        {
            if (transform.position.x <= 0)
                transform.position = new Vector3(0, transform.position.y, transform.position.z);
            else if (transform.position.x > 3.0687f)
                transform.position = new Vector3(3.0687f, transform.position.y, transform.position.z);
        }
    }


    bool IsEnemyTooCloseToPlayer()
    {
        GameObject newTarget = PlayerCaptainUnit.GetClosestEnemy();
        float threshold = enemyDistanceThreshold;
        if (!newTarget)
            return false;
        return Vector2.Distance(PlayerCaptainUnit.transform.position, newTarget.transform.position) < threshold;
    }
    void UpdateCameraPosition()
    {
        if (!target)
            return;
        if (IsEnemyTooCloseToPlayer())
            smoothTime = slowSmoothTime;
        else
            smoothTime = initialSmoothTime;
        Vector3 targetPosition = target.position + offset;
        Vector3 newPos = new Vector3(targetPosition.x, transform.position.y, transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, newPos, ref velocity, smoothTime);
        initialPosition = transform.position;
        lastNonShakingPosition = initialPosition; // Store the last non-shaking position
    }

    public void Shake(float _intensity = 1, float _duration = 1)
    {
        durationRemaining = _duration;
        intensity = _intensity;
        duration = _duration;
        isShaking = true;
        fading = false;
        CancelInvoke();
        Invoke("StopShaking", duration);
    }

    public void ShakeWithDelay(float delay, float _intensity = 1, float _duration = 1)
    {
        print("before2" + Time.time.ToString());

        StartCoroutine(ShakeWithDelayIE(delay, _intensity, _duration));
    }

    IEnumerator ShakeWithDelayIE(float delay, float _intensity = 1, float _duration = 1)
    {
        yield return new WaitForSeconds(delay);

        Shake(_intensity, _duration);
        print("after" + Time.time.ToString());

    }

    void SlowDownShaking()
    {
        if (durationRemaining > 1)
            return;
        if (fading == false)
        {
            fading = true;
            startTime = Time.time;
        }

        t = (Time.time - startTime) * fadeSpeed;
        intensity = Mathf.Lerp(intensity, 0, t);
    }

    public void StopShaking()
    {
        isShaking = false;
        initialPosition = lastNonShakingPosition; // Restore the last non-shaking position after shaking
        UpdateCameraPosition();
    }

    // Add remaining methods from the previous CameraLerp script.
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


    //void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.green;
    //    if (IsEnemyTooCloseToPlayer())
    //        Gizmos.color = Color.blue;
    //    Gizmos.DrawWireSphere(PlayerCaptainUnit.gameObject.transform.position, tmpThreshold);
    //}
}
