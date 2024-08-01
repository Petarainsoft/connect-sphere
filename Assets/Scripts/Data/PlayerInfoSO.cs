using AccountManagement;
using UnityEngine;

namespace ConnectSphere
{
    [CreateAssetMenu(fileName = "PlayerInfoSO", menuName = "Data/Player Info SO")]
    public class PlayerInfoSO : ScriptableObject
    {
        public string PlayerName;
        public int AvatarIndex;
        public NetworkRole Role;
        public string RoomName;
        public string Email;
        public int DatabaseId;
        public string Title;
        public Gender Gender;
        public string Occupation;
        public string Biography;

        public void MapData(Profile profile)
        {
            PlayerName = profile.display_name;
            AvatarIndex = profile.avatar_id;
            Title = profile.title;
            Gender = profile.gender == Gender.Female.ToString() ? Gender.Female : Gender.Male;
            Occupation = profile.occupation;
            Biography = profile.biography;
        }
    }

    public enum NetworkRole
    {
        Server = 0,
        Client = 1,
        Host = 2
    }

    public enum Gender
    {
        Female = 0,
        Male = 1,
        Other = 2
    }
}
