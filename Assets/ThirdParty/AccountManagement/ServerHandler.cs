using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Best.HTTP;
using Best.HTTP.Request.Settings;
using Best.HTTP.Request.Upload;
using UnityEngine;

namespace AccountManagement
{
    public class ServerHandler : MonoBehaviour
    {
        [SerializeField] protected string _serverUrl = "http://35.213.148.125:5000/";
        protected static string _bearerAccessToken;
        protected static string _accessToken;

        protected virtual void Awake()
        {
            _accessToken = PlayerPrefs.GetString("token");
            _bearerAccessToken = $"Bearer {_accessToken}";
        }

        public void SetBaseUrl(string url)
        {
            _serverUrl = url;
        }

        protected async Task<ApiResponse<T>> CreateGetRequestInternal<T>(string apiUrl,
            Dictionary<string, object> jsonData = null, bool logResponse = true)
            where T : BaseResult
        {
            if ( jsonData != null && jsonData.Count > 0 )
            {
                var uriBuilder = new UriBuilder(apiUrl);
                
                var queryParams = new StringBuilder();
                foreach (var param in jsonData)
                {
                    queryParams.Append($"{param.Key}={param.Value}&");
                }
                
                uriBuilder.Query = queryParams.ToString();
                
                apiUrl = uriBuilder.Uri.AbsoluteUri;
            }
            
            var request = HTTPRequest.CreateGet(apiUrl);
            if ( request == null )
            {
                return null;
            }
    
            request.AddHeader("Authorization", _bearerAccessToken);
            

            Debug.Log(request.CurrentUri);

            try
            {
                var response = await request.GetHTTPResponseAsync();
                var apiResponse = response.ToApiResponse<T>();
                if ( logResponse ) Debug.Log(apiResponse);
                return apiResponse;
            }
            catch (AsyncHTTPException e)
            {
                Debug.LogError($"Request finished with error! Error: {e.Message}");
            }

            return null;
        }

        protected async Task<ApiResponse<T>> CreatePostRequestInternal<T>(string apiUrl,
            Dictionary<string, object> jsonData, bool logResponse = true) where T : BaseResult
        {
            var request = HTTPRequest.CreatePost(apiUrl);
            if ( request == null ) return null;
            
            request.AddHeader("Authorization", _bearerAccessToken);

            var requestStream = new JSonDataStream<Dictionary<string, object>>(jsonData);

            request.UploadSettings = new UploadSettings()
            {
                UploadStream = requestStream
            };

            try
            {
                var response = await request.GetHTTPResponseAsync();
                var apiResponse = response.ToApiResponse<T>();
                if ( logResponse ) Debug.Log(apiResponse);

                return apiResponse;
            }
            catch (AsyncHTTPException e)
            {
                Debug.LogError($"Request finished with error! Error: {e.Message}");
            }

            return null;
        }
    }
}