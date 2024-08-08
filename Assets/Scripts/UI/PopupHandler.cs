using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ConnectSphere
{
    public class PopupHandler : MonoBehaviour
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _content;
        [SerializeField] private Button _positiveButton;
        [SerializeField] private Button _negativeButton;
        [SerializeField] private TMP_Text _textPositiveButton;
        [SerializeField] private TMP_Text _textNegativeButton;

        private void Start()
        {
            _positiveButton.onClick.AddListener(() => { Destroy(gameObject, 0.1f); });
            _negativeButton.onClick.AddListener(() => { Destroy(gameObject, 0.1f); });
        }

        /// <summary>
        /// Setup popup texts, it has 2 button in default, if negative param is empty the popup will have 1 button
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content"></param>
        /// <param name="positive"></param>
        /// <param name="negative"></param>
        public void Setup(string title, string content, string positive, string negative = "", UnityAction positiveCallback = null, UnityAction negativeCallback = null)
        {
            _title.text = title;
            _content.text = content;
            _textPositiveButton.text = positive;
            _textNegativeButton.text = negative;
            _negativeButton.gameObject.SetActive(negative.Equals(string.Empty));
            _positiveButton.onClick.AddListener(positiveCallback);
            _negativeButton.onClick.AddListener(negativeCallback);
        }
    }
}
