using UnityEngine;

public class SharedMethodsToolBox : MonoBehaviour
{
    public GameObject saveNotificationPopup;

    public static SharedMethodsToolBox instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void ShowSaveNotificationPopup()
    {
        saveNotificationPopup.SetActive(true);
    }
}
