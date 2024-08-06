using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "OfficeSO", menuName = "ScriptableObjects/OfficeSO")]
    public class OfficeSO : ScriptableObject
    {
        [SerializeField]
        private List<DTOOffice> _officeData;

        public int Count => _officeData.Count;

        public DTOOffice this[int i]
        {
            set
            {
                if (i >= Count)
                {
                    return;
                }
                _officeData[i].name = value.name;
                _officeData[i].last_access = value.last_access;
                _officeData[i].resource_url = value.resource_url;
            }
            get
            {
                if (i >= Count)
                {
                    return null;
                }
                return _officeData[i];
            }
        }

        public void AddOffice(DTOOffice office)
        {
            if (_officeData == null)
            {
                _officeData = new();
            }
            _officeData.Add(office);
        }

        public void ClearAllOffice()
        {
            _officeData.Clear();
        }
    }
}
