using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedEnabler : MonoBehaviour
{
    [System.Serializable]
    public struct TimedObject
    {
        public GameObject objectToEnable; // The object to enable
        public Component component; // The component to enable
        public float timeToEnable; // The time at which to enable the object
    }

    // List of timed objects
    public List<TimedObject> timedObjects = new List<TimedObject>();

    void Start()
    {
        // Start the coroutine to enable objects at specified times
        _ = StartCoroutine(EnableObjectsAtTimes());
    }

    public void AddTimedObject(GameObject obj, float time)
    {
        TimedObject newTimedObject = new TimedObject { objectToEnable = obj, timeToEnable = time };
        timedObjects.Add(newTimedObject);
    }

    private IEnumerator EnableObjectsAtTimes()
    {
        //// Sort the list by time to enable (optional, for ordered enabling)
        //timedObjects.Sort((a, b) => a.timeToEnable.CompareTo(b.timeToEnable));

        foreach (TimedObject timedObject in timedObjects)
        {
            // Wait for the specified time
            yield return new WaitForSeconds(timedObject.timeToEnable);

            // Enable the object
            if (timedObject.objectToEnable != null)
            {
                timedObject.objectToEnable.SetActive(true);
            }
            else if (timedObject.component != null && timedObject.component is Behaviour)
            {
                (timedObject.component as Behaviour).enabled = true;
            }
        }
    }
}
