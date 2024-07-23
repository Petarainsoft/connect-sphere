using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class RandomNameGenerator : MonoBehaviour
    {
        public TMP_InputField _inputName;

        // Array of predefined names
        private string[] names = { "Alice", "Bob", "Charlie", "Diana", "Eve", "Frank", "Grace", "Hank", "John",
            "Stephanie", "Taylor", "Janet", "Rose", "Jenny", "Maya", "Riley", "Farkle", "Long", "Sol", "Lisa",
            "Gweny", "Harold", "Emily", "Felicity", "Lizzie", "Gary", "Matty", "Robert", "Winnie", "Wendy", "Eddy"};

        // Method to get a random name
        public string GetRandomName()
        {
            // Check if the array is not empty
            if (names.Length == 0)
            {
                Debug.LogWarning("Names array is empty!");
                return string.Empty;
            }

            // Generate a random index
            int randomIndex = Random.Range(0, names.Length);

            // Return the name at the random index
            return names[randomIndex];
        }

        // Example usage
        void Start()
        {
            _inputName.text = GetRandomName();
        }
    }
}
