using System;
using System.Collections;
using System.Collections.Generic;
using AccountManagement;
using UnityEngine;

namespace ConnectSphere
{
    [Serializable]
    public class Chat
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public UserData chat_admin { get; set; }
        public List<UserData> users { get; set; }
        public Message last_message { get; set; }
    }
}
