using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AccountManagement
{
    [Serializable]
    public class ApiResponse<T> where T : class
    {
        public string Url { get; set; }
        public string Method { get; set; }

        public bool IsSuccess { get; set; }
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public T ReturnData { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    // some general data model class
    [Serializable]
    public class UserData
    {
        public string access_token { get; set; }
        public int id { get; set; }
        public string username { get; set; }
    }

    [Serializable]
    public class BaseResult
    {
        public string message { get; set; }
        public string status { get; set; }
        public Error error { get; set; }
    }

    [Serializable]
    public class Error
    {
        public string code { get; set; }
        public string details { get; set; }
    }

    // VerifyToken API response
    [Serializable]
    public class VerifyTokenResult
    {
        public string msg { get; set; }
    }

    // Login API response

    [Serializable]
    public class LoginResult : BaseResult
    {
        public UserData data { get; set; }
    }

    // Register API response
    [Serializable]
    public class RegisterResult : BaseResult
    {
        public UserData data { get; set; }
    }

    // Create Chat Api response
    [Serializable]
    public class Chat
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public UserData chat_admin { get; set; }
        public List<UserData> users { get; set; }
        public Message last_message { get; set; }
    }
    
    [Serializable]
    public class MetaChat
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool is_private { get; set; }
        public bool is_group { get; set; }
        public int chat_admin { get; set; }
    }

    [Serializable]
    public class ChatWithUsers : MetaChat
    {
        List<UserData> users { get; set; }
    }

    // "data": {
    //     "chat_admin": 4,
    //     "id": 25,
    //     "is_group": false,
    //     "is_private": false,
    //     "name": "hoanganh3 Created with hoanganh2",
    //     "users": [
    //     {
    //         "id": 3,
    //         "username": "hoanganh2@gmail.com"
    //     },
    //     {
    //         "id": 4,
    //         "username": "hoanganh3@gmail.com"
    //     }
    //     ]
    // },
    // "message": "Chat meta retrieved successfully",
    // "status": "success"
    

    [Serializable]
    public class CreateChatResult : BaseResult
    {
        public MetaChat data { get; set; }
    }

    // Poll Data response
    [Serializable]
    public class PollDataResult : BaseResult
    {
        public List<Chat> data { get; set; }
    }

    // Delete Chat Response
    [Serializable]
    public class DeleteChatResult : BaseResult
    {
    }

    // GetChatHistory Response
    [Serializable]
    public class Message
    {
        public int id { get; set; }
        public int chat_id { get; set; }
        public int sender_id { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }
    }


// # Lấy thông tin người tạo chat
//         chat_admin = User.query.get(chat.chat_admin)
//     chat_admin_data = {
//         'id': chat_admin.id,
//         'username': chat_admin.username
//     } if chat_admin else None
//
// # Lấy thông tin người tham gia
//         users = [{'id': user.id, 'username': user.username} for user in chat.users]
//
//     chat_list.append({
//         'id': chat.id,
//         'name': chat.name,
//         'is_private': chat.is_private,
//         'is_group': chat.is_group,
//         'chat_admin': chat_admin_data,
//         'users': users,
//         'last_message': last_message_data
//     })
//     
//     return jsonify({
//         "status": "success",
//         "data": chat_list,
//         "message": "Chats retrieved successfully"
//     }), 200

    [Serializable]
    public class GetChatHistoryResult : BaseResult
    {
        public List<Message> data { get; set; }
    }

    // Add user to chat response
    [Serializable]
    public class AddUserToChatResult : BaseResult
    {
        public MetaChat data { get; set; }
    }

    // Get chat meta data response
    [Serializable]
    public class GetChatMetaResult : BaseResult
    {
        public ChatWithUsers data { get; set; }
    }

    // request code
    [Serializable]
    public class RequestCodeResult : BaseResult
    {
    }

    // update password result
    [Serializable]
    public class UpdatePasswordResult : BaseResult
    {
    }

    [Serializable]
    public class Profile
    {
        public int avatar_id { get; set; }
        public string biography { get; set; }
        public string display_name { get; set; }
        public string gender { get; set; }
        public int id { get; set; }
        public string occupation { get; set; }
        public string title { get; set; }
    }
    
    [Serializable]
    public class ProfileResult : BaseResult
    {
        public Profile data { get; set; }
    }
    
    [Serializable]
    public class NoteResult : BaseResult
    {
        public Note data { get; set; }
    }
    
    [Serializable]
    public class Note
    {
        public int id { get; set; }
        public int client_note_id { get; set; }
        
        public int user_id { get; set; }
        public string title { get; set; }
        
        public NoteVersion last_version { get; set; }
    }
    
    [Serializable]
    public class GetNoteResult : BaseResult
    {
        public NoteVersion data { get; set; }
    }

    [Serializable]
    public class NoteVersion
    {
        public int note_id { get; set; }
        public int client_note_id { get; set; }
        
        public int id { get; set; } // version
        
        public int user_id { get; set; }
        public string content { get; set; }
        public DateTime timestamp { get; set; }
    }
}