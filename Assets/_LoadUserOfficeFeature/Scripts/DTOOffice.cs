using AccountManagement;
using System;
using System.Collections.Generic;

namespace ConnectSphere
{
    [Serializable]
    public class DTOOffice
    {
        public string name;
        public DateTime lastAccess = DateTime.Now;
        public string resource_url;

        public int GetDateDiff()
        {
            return (int)(DateTime.Now - lastAccess).TotalSeconds;
        }
    }

    [Serializable]
    public class DTOBaseOffice
    {
        public string name { get; set; }
    }

    [Serializable]
    public class CreateOfficeResult : BaseResult
    {
        public DTOBaseOffice office { get; set; }
    }
    [Serializable]
    public class LoadAllOfficeResult : BaseResult
    {
        public List<DTOOffice> offices { get; set; }
    }
}
}
