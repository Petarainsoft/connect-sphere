using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace ConnectSphere
{
    public class RandomNameGenerator : MonoBehaviour
    {
        // Array of predefined names
        private static readonly string[] names = { "Alex", "Bob", "Charlie", "Frank", "Hank", "John", "Farkle", "Long", "Harold", "Gary", 
            "Matty", "Robert", "Eddy", "Jack", "Jacob", "Elijah", "Mason", "William", "Oliver", "Lucas", "Daniel", "Levi", "Owen",
            "Luca", "Ethan", "David", "Jackson", "Luke", "Gabriel", "Isaac", "Miles", "Anthony", "Jayden", "Aiden", "Cris", "Ian",
            "Alice", "Diana", "Eve", "Grace", "Stephanie", "Taylor", "Janet", "Rose", "Jenny",
            "Maya", "Riley", "Sol", "Lisa", "Gweny", "Emily", "Felicia", "Lizzie", "Winnie", "Wendy", "Asha", "Lilac", "Eloise",
            "Daphne", "Clara", "Lucy", "Aurora", "Hazel", "Freya", "Snow", "Nixi", "Lulu", "Cassandra", "Isabelle", "Bella", "Kelly" };
        
        // Method to get a random name
        public static string GetRandomName(bool isFemale)
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
    }
}
