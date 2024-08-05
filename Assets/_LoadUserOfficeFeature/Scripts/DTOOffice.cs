using System;

namespace ConnectSphere
{
    [Serializable]
    public class DTOOffice
    {
        public int id;
        public string name;
        public int userId;
        public DateTime lastAccess = DateTime.Now;
        public bool isRemove;

        public int GetDateDiff()
        {
            return (int)(DateTime.Now - lastAccess).TotalSeconds;
        }
    }
}