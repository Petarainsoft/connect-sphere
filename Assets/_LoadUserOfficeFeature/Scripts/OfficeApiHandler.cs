using AccountManagement;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

namespace ConnectSphere
{
    public class OfficeApiHandler : ServerHandler
    {
        private static string OFFICE_ROUTE = "office";
        private static string CREATE_OFFICE = "/create_office";
        private static string LOAD_ALL_OFFICE = "/get_old_offices";
        private static string EXIT_OFFICE = "/update_last_access";

        public async UniTask<CreateOfficeResult> CreateNewOffice(
            string officeName,
            string resourceUrl
        )
        {
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

        public async UniTask<ExitOfficeResult> UpdateLastAccess(string officename, string username)
        {
            var apiResponse = await CreatePostRequestInternal<ExitOfficeResult>(
                $"{_serverUrl}{OFFICE_ROUTE}{EXIT_OFFICE}",
                new Dictionary<string, object> {
                    { "office_name", officename},
                    { "username", username},

                }
            );

            if (!apiResponse.IsSuccess)
                Debug.LogError(apiResponse.ResponseMessage);
            if (apiResponse.ReturnData.error != null)
                Debug.LogError(apiResponse.ReturnData.error.code);

            return apiResponse.ReturnData;
        }
    }
}
