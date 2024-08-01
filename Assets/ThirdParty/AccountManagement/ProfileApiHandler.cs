using System.Collections.Generic;
using AccountManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    public class ProfileApiHandler : ServerHandler
    {
        [SerializeField] private string _getUserProfile = "chat/get_user_by_id";
        [SerializeField] private string _updateUserProfile = "auth/update_profile";

#if ANH_LOCAL
        [Sirenix.OdinInspector.Button]
#endif
        public async UniTask<Profile> GetUserProfile(int userId)
        {
            if ( string.IsNullOrEmpty(_accessToken) )
            {
                return null;
            }

            var apiUrl = $"{_serverUrl}{_getUserProfile}";
            Debug.Log(apiUrl);

            var apiResponse = await CreateGetRequestInternal<ProfileResult>(apiUrl, new Dictionary<string, object>()
            {
                { "id", userId }
            });

            if ( apiResponse == null ) return null;
            if ( !apiResponse.IsSuccess )
            {
                Debug.LogError($"{apiResponse.ResponseCode} | {apiResponse.ResponseMessage}");
            }

            if ( apiResponse.ReturnData == null ) return null;
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogError(apiResponse.ReturnData.error.details);
                return null;
            }

            return apiResponse.ReturnData.data;
        }

#if ANH_LOCAL
        [Sirenix.OdinInspector.Button]
#endif
        public async UniTask<Profile> UpdateUserProfile(
            int avatarId = -1,
            string displayName = "",
            string title = "",
            string gender = "",
            string occupation = "",
            string biography = "")
        {
            var parameters = new Dictionary<string, object>();

            if ( avatarId > -1 ) parameters.Add("avatar_id", avatarId);
            if ( !string.IsNullOrEmpty(displayName) ) parameters.Add("display_name", displayName);
            
            if ( !string.IsNullOrEmpty(title) ) parameters.Add("title", title);
            if ( !string.IsNullOrEmpty(gender) ) parameters.Add("gender", gender);
            if ( !string.IsNullOrEmpty(occupation) ) parameters.Add("occupation", occupation);
            if ( !string.IsNullOrEmpty(biography) ) parameters.Add("biography", biography);

            var apiResponse =
                await CreatePostRequestInternal<ProfileResult>($"{_serverUrl}{_updateUserProfile}", parameters, true);

            if ( apiResponse == null ) return null;
            if ( !apiResponse.IsSuccess )
            {
                Debug.LogError($"{apiResponse.ResponseCode} | {apiResponse.ResponseMessage}");
            }
            
            if ( apiResponse.ReturnData == null ) return null;
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogError(apiResponse.ReturnData.error.details);
                return null;
            }

            return apiResponse.ReturnData.data;
        }
    }
}