using UnityEngine;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// This is the view of our cell which handles how the cell looks.
    /// </summary>
    public class ChatCellView : EnhancedScrollerCellView
    {
        public RectWidthUpdater widthUpdater;

        public TextMeshProUGUI _shortTimeStamp;
        
        public Image Icon;
        /// <summary>
        /// A reference to the UI Text element to display the cell data
        /// </summary>
        public TMP_Text someTextText;

        /// <summary>
        /// A reference to the rect transform which will be
        /// updated by the content size fitter
        /// </summary>
        public RectTransform textRectTransform;

        /// <summary>
        /// The space around the text label so that we
        /// aren't up against the edges of the cell
        /// </summary>
        public RectOffset textBuffer;

        [SerializeField]
        private Data _data;
        

        public void SetData(Data data)
        {
            someTextText.text = data.someText;
            _data = data;
            _data.messageScroller = this;
            if ( Icon != null )
            {
                Icon.gameObject.SetActive(_data.ShowIcon);
            }
            _shortTimeStamp.text = data.timeStamp != null ? data.timeStamp.ToShortTimeString() : "";
            widthUpdater.UpdateRectHeightOfCell();
        }

        public float GetCellViewSize()
        {
            return (transform as RectTransform).rect.height;
        }
    }
}
