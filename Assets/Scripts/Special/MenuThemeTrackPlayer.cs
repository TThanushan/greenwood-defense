using UnityEngine;

public class MenuThemeTrackPlayer : MonoBehaviour
{
    public static MenuThemeTrackPlayer instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
}
