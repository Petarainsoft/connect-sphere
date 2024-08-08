using AccountManagement;
using System;
using System.Collections.Generic;

namespace ConnectSphere
{
    [Serializable]
    public class DTOOffice
    {
        public string name { get; set; }
        public DateTime last_access { get; set; }
        public string resource_url { get; set; }
        public string type_user { get; set; }

        public int GetDateDiff()
        {
            if (last_access == null)
                last_access = DateTime.Now;
            return (int)(DateTime.Now - last_access).TotalSeconds;
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
        public DTOBaseOffice data { get; set; }
    }

    [Serializable]
    public class LoadAllOfficeResult : BaseResult
    {
        public List<DTOOffice> data { get; set; }
    }

    [Serializable]
    public class ExitOfficeResult : BaseResult { 
        public DTOOffice data { get; set; }
    }
}
