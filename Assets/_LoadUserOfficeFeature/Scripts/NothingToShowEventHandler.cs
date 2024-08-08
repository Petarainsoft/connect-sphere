using UnityEngine;

namespace ConnectSphere
{
    public class NothingToShowEventHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] private OfficeDataLoader _officeDataLoader;
        [SerializeField] private GameObject _nothingToShowObject;

        // Update is called once per frame
        void Update()
        {
            if(_officeDataLoader != null)
            {
                _nothingToShowObject.SetActive(_officeDataLoader.ActiveCount == 0);
            }
        }
    }
}
