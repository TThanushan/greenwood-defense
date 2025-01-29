using UnityEngine;

public class SwitchNextBackgroundDEBUGONLY : MonoBehaviour
{
    public bool onRKeyPress = false;
    // Make a method that will disable the current child enabled and enable the next child.
    public void SwitchNextBackground()
    {
        // Get the current child index
        int currentChildIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                currentChildIndex = i;
                break;
            }
        }

        // Disable the current child
        transform.GetChild(currentChildIndex).gameObject.SetActive(false);

        // Enable the next child
        int nextChildIndex = (currentChildIndex + 1) % transform.childCount;
        transform.GetChild(nextChildIndex).gameObject.SetActive(true);
    }

    // Make a method that will disable the current child enabled and enable the previous child.
    public void SwitchPreviousBackground()
    {
        // Get the current child index
        int currentChildIndex = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                currentChildIndex = i;
                break;
            }
        }

        // Disable the current child
        transform.GetChild(currentChildIndex).gameObject.SetActive(false);

        // Enable the previous child
        int previousChildIndex = (currentChildIndex - 1 + transform.childCount) % transform.childCount;
        transform.GetChild(previousChildIndex).gameObject.SetActive(true);
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!onRKeyPress)
        {

            // Call the method on key press
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                SwitchNextBackground();
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SwitchPreviousBackground();
            }

        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchNextBackground();
        }
    }
#endif
}
