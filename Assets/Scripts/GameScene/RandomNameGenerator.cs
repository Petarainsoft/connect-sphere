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
        private readonly string[] maleNames = { "Alex", "Bob", "Charlie", "Frank", "Hank", "John", "Farkle", "Long", "Harold", "Gary", 
            "Matty", "Robert", "Eddy", "Jack", "Jacob", "Elijah", "Mason", "William", "Oliver", "Lucas", "Daniel", "Levi", "Owen",
            "Luca", "Ethan", "David", "Jackson", "Luke", "Gabriel", "Isaac", "Miles", "Anthony", "Jayden", "Aiden", "Cris", "Ian"};

        private readonly string[] femaleNames = { "Alice", "Diana", "Eve", "Grace", "Stephanie", "Taylor", "Janet", "Rose", "Jenny",
            "Maya", "Riley", "Sol", "Lisa", "Gweny", "Emily", "Felicia", "Lizzie", "Winnie", "Wendy", "Asha", "Lilac", "Eloise",
            "Daphne", "Clara", "Lucy", "Aurora", "Hazel", "Freya", "Snow", "Nixi", "Lulu", "Cassandra", "Isabelle", "Bella", "Kelly"};

        private void OnEnable()
        {
            MenuManager.OnAvatarImageClicked += GenerateName;
        }

        private void OnDisable()
        {
            MenuManager.OnAvatarImageClicked -= GenerateName;
        }

        private void GenerateName(int index)
        {
            if (index == 1 || index == 5)
            {
                _inputName.text = GetRandomName(true);
            }
            else
            {
                _inputName.text = GetRandomName(false);
            }
        }    
        
        // Method to get a random name
        public string GetRandomName(bool isFemale)
        {
            // Check if the array is not empty
            if (maleNames.Length == 0)
            {
                Debug.LogWarning("Names array is empty!");
                return string.Empty;
            }

            // Generate a random index
            int randomIndex = isFemale ? Random.Range(0, femaleNames.Length) : Random.Range(0, maleNames.Length);

            // Return the name at the random index
            return isFemale ? femaleNames[randomIndex] : maleNames[randomIndex];
        }

        // Example usage
        void Start()
        {
            GenerateName(0);
        }
    }
}
