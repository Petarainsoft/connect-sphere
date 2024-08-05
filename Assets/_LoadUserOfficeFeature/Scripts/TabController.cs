using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class TabController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textMeshProUGUI;
        [SerializeField] private Lean.Gui.LeanBox backGroundButton;

        public void OnChoosingTab()
        {
            ChangeColor(chooseColor);
            backGroundButton.enabled = true;
        }
        private static string unchooseColor = "#A6A6A0";
        private static string chooseColor = "#FFFFFF";

        private void ChangeColor(string color)
        {
            Color newColor;
            if (ColorUtility.TryParseHtmlString(color, out newColor))
            {
                textMeshProUGUI.color = newColor;
            }
            else
            {
                Debug.LogError("Invalid color code");
            }
        }

        public void OnUnChoosingTab()
        {
            ChangeColor(unchooseColor);
            backGroundButton.enabled = false;
        }
    }
}
