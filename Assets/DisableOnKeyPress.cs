using UnityEngine;

public class DisableOnKeyPress : MonoBehaviour
{
    // Disable the game object on key press
    public KeyCode key = KeyCode.Space;
    public bool onKeyPress = true;
    public bool onKeyUp = true;
    public bool onKeyDown = true;

    void Update()
    {
        if ((onKeyPress && Input.GetKeyDown(key)) ||
                       (onKeyUp && Input.GetKeyUp(key)) ||
                                  (onKeyDown && Input.GetKeyDown(key)))
        {
            gameObject.SetActive(false);
        }
    }
}
