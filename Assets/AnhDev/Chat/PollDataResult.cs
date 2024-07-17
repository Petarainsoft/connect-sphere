using System;
using System.Collections;
using System.Collections.Generic;
using AccountManagement;
using UnityEngine;

namespace ConnectSphere
{
    // Poll Data response
    [Serializable]
    public class PollDataResult : BaseResult
    {
        public List<Chat> data { get; set; }
    }
}
