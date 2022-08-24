using UnityEngine;

public class ShowHideMiniMap : MonoBehaviour
{
    public GameObject minimap;
    public GameObject minimapOverlay;
    public GameObject minimapBackground;

    Camera minimapOverlayCamera;
    Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        minimapOverlayCamera = minimapOverlay.GetComponent<Camera>();
    }
    private void Update()
    {
        if (Camera.main.transform.position.x >= 2.657f && minimap.activeSelf)
        {
            EnableMinimap(false);
        }
        else if (!minimap.activeSelf && Camera.main.transform.position.x < 2.657f)
            EnableMinimap(true);
    }

    void EnableMinimap(bool val)
    {
        minimap.SetActive(val);
        minimapBackground.SetActive(val);
        minimapOverlayCamera.enabled = val;
    }
}
