using System.Collections.Generic;
using AccountManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    public class OfficeApiHandler : ServerHandler
    {
        public string test = "abv";
        private static string OFFICE_ROUTE = "/office";

        private static string CREATE_OFFICE = "/new_office";
        private static string LOAD_ALL_OFFICE = "/old_offices";

        public async UniTask<CreateOfficeResult> CreateNewOffice(
            string officeName,
            string resourceUrl
        )
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString("token")))
            {
                PlayerPrefs.SetString("token", test);
            }
            var apiResponse = await CreatePostRequestInternal<CreateOfficeResult>(
                $"{_serverUrl}{OFFICE_ROUTE}{CREATE_OFFICE}",
                new Dictionary<string, object>
                {
                    { "name", officeName },
                    { "resource_url", resourceUrl }
                }
            );

            if (!apiResponse.IsSuccess)
                Debug.LogError(apiResponse.ResponseMessage);
            if (apiResponse.ReturnData.error != null)
                Debug.LogError(apiResponse.ReturnData.error.code);

            return apiResponse.ReturnData;
        }

        public async UniTask<LoadAllOfficeResult> LoadAllOffices()
        {
            PlayerPrefs.SetString("token", test);
            var apiResponse = await CreateGetRequestInternal<LoadAllOfficeResult>(
                $"{_serverUrl}{OFFICE_ROUTE}{LOAD_ALL_OFFICE}",
                new Dictionary<string, object> { }
            );

            if (!apiResponse.IsSuccess)
                Debug.LogError(apiResponse.ResponseMessage);
            if (apiResponse.ReturnData.error != null)
                Debug.LogError(apiResponse.ReturnData.error.code);

            return apiResponse.ReturnData;
        }
    }
}
