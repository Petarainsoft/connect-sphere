using UnityEngine;

public class ToggleFullscreenUI : MonoBehaviour
{
    [Tooltip("The UI panel to toggle between fullscreen and rectangle.")]
    public RectTransform uiPanel;

    [Tooltip("The size of the rectangle when not in fullscreen.")]
    public Vector2 rectangleSize = new Vector2(150, 200);

    private Vector2 originalSize;
    private Vector2 originalPosition;

    void Start()
    {
        if (uiPanel != null)
        {
            originalSize = uiPanel.sizeDelta;
            originalPosition = uiPanel.anchoredPosition;
        }
    }    
    
    public void SetFullScreen(bool fullscreen)
    {
        // if (uiPanel == null) return;
        //
        // if (fullscreen)
        // {
        //     uiPanel.sizeDelta = originalSize;
        //     uiPanel.anchorMin = new Vector2(0f, 0f);
        //     uiPanel.anchorMax = new Vector2(1f, 1f);
        //     uiPanel.pivot = new Vector2(0.5f, 0.5f);
        //     uiPanel.anchoredPosition = originalPosition;
        // }
        // else
        // {
        //     uiPanel.sizeDelta = rectangleSize;
        //     uiPanel.anchorMin = new Vector2(1, 1);
        //     uiPanel.anchorMax = new Vector2(1, 1);
        //     uiPanel.pivot = new Vector2(1, 1);
        //     uiPanel.anchoredPosition = new Vector2(-10, -10); // 10 pixels from top-right corner
        // }
    }
}