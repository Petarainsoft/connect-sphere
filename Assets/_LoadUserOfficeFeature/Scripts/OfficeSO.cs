using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "OfficeSO", menuName = "ScriptableObjects/OfficeSO")]
    public class OfficeSO : ScriptableObject
    {
        [SerializeField]
        private List<DTOOffice> data;

        public int Count => data.Count;
        public DTOOffice this[int i]
        {
            get
            {
                if (i >= Count)
                {
                    return null;
                }
                return data[i];
            }

        }
    }
}
