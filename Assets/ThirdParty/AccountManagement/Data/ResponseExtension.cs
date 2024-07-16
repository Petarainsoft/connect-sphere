using System.Collections.Generic;
using Best.HTTP;
using Newtonsoft.Json;
using UnityEngine;

namespace AccountManagement
{
    public static class ResponseExtension
    {
        public static ApiResponse<T> ToApiResponse<T>(this HTTPResponse serverRp) where T : class
        {
            var rp = new ApiResponse<T>
            {
                Url = serverRp.Request.Uri.ToString(),
                Method = serverRp.Request.MethodType.ToString(),

                IsSuccess = serverRp.IsSuccess,
                ResponseCode = serverRp.StatusCode,
                ResponseMessage = serverRp.Message,
                ReturnData = null
            };

            if ( string.IsNullOrEmpty(serverRp.DataAsText) ) return rp;

            try
            {
                rp.ReturnData = JsonConvert.DeserializeObject<T>(serverRp.DataAsText);
            }
            catch (JsonException ex)
            {
                Debug.LogError(ex);
            }

            return rp;
        }
    }
}