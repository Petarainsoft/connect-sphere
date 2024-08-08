using AccountManagement;
using Fusion;
using UnityEngine;
using UnityEngine.UI;

namespace ConnectSphere
{
    public class BackButtonHandler : NetworkBehaviour
    {
        

        private Button _turnBackButton;

        private void OnEnable()
        {
            _turnBackButton = GetComponent<Button>();
            _turnBackButton.onClick.AddListener(() =>
            {
                Debug.Log("Back to office");
                Back();
                Runner.Shutdown();

            });
        }

        private void OnDisable()
        {
            _turnBackButton.onClick.RemoveAllListeners();
        }

        public void Back()
        {
           var officeApiHandler = ApiManager.Instance.GetComponent<OfficeApiHandler>();
            Debug.Log(PlayerPrefs.GetString("office") + " " + PlayerPrefs.GetString("username"));
            _ = officeApiHandler.UpdateLastAccess(PlayerPrefs.GetString("office"), PlayerPrefs.GetString("username"));
        }
    }
}
