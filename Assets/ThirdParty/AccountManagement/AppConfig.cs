using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AccountManagement
{
    public class AppConfig
    {
        public const int UserNameMinLength = 6;
        public const string UserNameRegex = @"^[a-zA-Z0-9_]+$";
        public const int PasswordMinLength = 6;

        public const string EmailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        public const int ResetCodeLength = 6;
    }
}
