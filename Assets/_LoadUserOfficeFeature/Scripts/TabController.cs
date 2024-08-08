using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class TabController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI textMeshProUGUI;

        [SerializeField]
        private Lean.Gui.LeanBox backGroundButton;

        public void ChooseTab()
        {
            ChangeColor(CHOOSE_COLOR);
            backGroundButton.enabled = true;
        }

        private static string UNCHOOSE_COLOR = "#A6A6A0";
        private static string CHOOSE_COLOR = "#FFFFFF";

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

        public void UnChooseTab()
        {
            ChangeColor(UNCHOOSE_COLOR);
            backGroundButton.enabled = false;
        }
    }
}
