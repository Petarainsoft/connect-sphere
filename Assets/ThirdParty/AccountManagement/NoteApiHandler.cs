using System.Collections.Generic;
using AccountManagement;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ConnectSphere
{
    public class NoteApiHandler : ServerHandler
    {
        [SerializeField] private string _getNote = "note/get_last_note";
        [SerializeField] private string _updateNote = "note/update_note_using_client_id";
        [SerializeField] private string _createNote = "note/create_note";
        
#if ANH_LOCAL
        [Sirenix.OdinInspector.Button]
#endif
        public async UniTask<Note> CreateNote(int clientNoteId, string newContent)
        {
            if ( string.IsNullOrEmpty(_accessToken) )
            {
                return null;
            }
            
            var parameters = new Dictionary<string, object>();

            if ( clientNoteId > 0 ) parameters.Add("client_note_id", clientNoteId);
            if ( newContent != null ) parameters.Add("content", newContent);
            parameters.Add("title", $"Note {clientNoteId}");
            var apiResponse =
                await CreatePostRequestInternal<NoteResult>($"{_serverUrl}{_createNote}", parameters, true);

            if ( apiResponse == null ) return null;
            if ( !apiResponse.IsSuccess )
            {
                Debug.LogWarning($"{apiResponse.ResponseCode} | {apiResponse.ResponseMessage}");
            }

            if ( apiResponse.ReturnData == null ) return null;
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogWarning(apiResponse.ReturnData.error.details);
                return null;
            }

            return apiResponse.ReturnData.data;
        }

#if ANH_LOCAL
        [Sirenix.OdinInspector.Button]
#endif
        public async UniTask<NoteVersion> GetNote(int clientNoteId)
        {
            if ( string.IsNullOrEmpty(_accessToken) )
            {
                return null;
            }

            var apiUrl = $"{_serverUrl}{_getNote}";
            Debug.Log(apiUrl);

            var apiResponse = await CreateGetRequestInternal<GetNoteResult>(apiUrl, new Dictionary<string, object>()
            {
                { "client_note_id", clientNoteId }
            });

            if ( apiResponse == null ) return null;
            if ( !apiResponse.IsSuccess )
            {
                Debug.LogWarning($"{apiResponse.ResponseCode} | {apiResponse.ResponseMessage}");
            }

            if ( apiResponse.ReturnData == null ) return null;
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogWarning(apiResponse.ReturnData.error.details);
                return null;
            }

            return apiResponse.ReturnData.data;
        }

#if ANH_LOCAL
        [Sirenix.OdinInspector.Button]
#endif
        public async UniTask<Note> UpdateNote(
            int clientNoteId, string newContent)
        {
            if ( string.IsNullOrEmpty(_accessToken) )
            {
                return null;
            }
            
            var parameters = new Dictionary<string, object>();

            if ( clientNoteId > 0 ) parameters.Add("client_note_id", clientNoteId);
            if ( newContent != null ) parameters.Add("newContent", newContent);

            var apiResponse =
                await CreatePostRequestInternal<NoteResult>($"{_serverUrl}{_updateNote}", parameters, true);

            if ( apiResponse == null ) return null;
            if ( !apiResponse.IsSuccess )
            {
                Debug.LogWarning($"{apiResponse.ResponseCode} | {apiResponse.ResponseMessage}");
            }

            if ( apiResponse.ReturnData == null ) return null;
            if ( apiResponse.ReturnData.error != null )
            {
                Debug.LogWarning(apiResponse.ReturnData.error.details);
                return null;
            }

            return apiResponse.ReturnData.data;
        }
    }
}