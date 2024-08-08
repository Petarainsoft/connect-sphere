using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class TextChatUI : MonoBehaviour
{
    private IList<KeyValuePair<string, MessageObjectUI>> m_MessageObjPool =
        new List<KeyValuePair<string, MessageObjectUI>>();

    public static Action<bool> OnTyping;

    ScrollRect m_TextChatScrollRect;

    public GameObject ChatContentObj;
    public GameObject MessageObject;
    public Button EnterButton;
    public InputField MessageInputField;
    private Task FetchMessages = null;
    private DateTime? oldestMessage = null;

    public TMP_Text _chatFrameTitle;

    public void FireOntypingFalse()
    {
        OnTyping?.Invoke(false);
    }
    
    private string _lastMessageDisplayName = string.Empty;

    IEnumerator Start()
    {
        yield return new WaitUntil(() => VivoxService.Instance != null);
        VivoxService.Instance.ChannelJoined += OnChannelJoined;
        VivoxService.Instance.DirectedMessageReceived += OnDirectedMessageReceived;
        VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
        VivoxService.Instance.ChannelMessageEdited += OnChannelMessageEdited;
        VivoxService.Instance.ChannelMessageDeleted += OnChannelMessageDeleted;

        m_TextChatScrollRect = GetComponent<ScrollRect>();

#if !(UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
        MessageInputField.gameObject.SetActive(false);
        EnterButton.gameObject.SetActive(false);
#else
        EnterButton.onClick.AddListener(SendMessage);
        MessageInputField.onValueChanged.AddListener(OnvalueChange);
        MessageInputField.onEndEdit.AddListener((string text) => { EnterKeyOnTextField(); });
#endif
        m_TextChatScrollRect.onValueChanged.AddListener(ScrollRectChange);
    }

    private void OnvalueChange(string arg0)
    {
        OnTyping?.Invoke(true);
    }

    private void OnEnable()
    {
        ClearTextField();
    }

    private void OnDisable()
    {
        if ( m_MessageObjPool.Count > 0 )
        {
            ClearMessageObjectPool();
        }
        OnTyping?.Invoke(false);
        oldestMessage = null;
    }

    private void ScrollRectChange(Vector2 vector)
    {
        // Scrolled near end and check if we are fetching history already
        if ( m_TextChatScrollRect.verticalNormalizedPosition >= 0.95f && FetchMessages != null &&
             (FetchMessages.IsCompleted || FetchMessages.IsFaulted || FetchMessages.IsCanceled) )
        {
            m_TextChatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
            FetchMessages = FetchHistory(false);
        }
    }

    private async Task FetchHistory(bool scrollToBottom = false)
    {
        try
        {
            if ( string.IsNullOrEmpty(currentChannelName) )
            {
                Debug.LogError("Current channel name is empty, cannot get chat history");
                return;
            }

            var chatHistoryOptions = new ChatHistoryQueryOptions()
            {
                TimeEnd = oldestMessage
            };
            var historyMessages =
                await VivoxService.Instance.GetChannelTextMessageHistoryAsync(currentChannelName, 10,
                    chatHistoryOptions);
            var reversedMessages = historyMessages.Reverse().ToList();

            for (int i = 0; i < reversedMessages.Count; i++)
            {
                var historyMessage = reversedMessages[i];
                var prevDisplayName = i > 0 ? reversedMessages[i - 1].SenderDisplayName : string.Empty;
                AddMessageToChat(historyMessage, true, scrollToBottom, prevDisplayName:prevDisplayName);
                await UniTask.WaitForEndOfFrame();
            }

            MessageInputField.enabled = true;

            // Update the oldest message ReceivedTime if it exists to help the next fetch get the next batch of history
            oldestMessage = historyMessages.FirstOrDefault()?.ReceivedTime;
        }
        catch (TaskCanceledException e)
        {
            Debug.Log(
                $"Chat history request was canceled, likely because of a logout or the data is no longer needed: {e.Message}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Tried to fetch chat history and failed with error: {e.Message}");
        }
    }

    void OnDestroy()
    {
        VivoxService.Instance.ChannelJoined -= OnChannelJoined;
        VivoxService.Instance.DirectedMessageReceived -= OnDirectedMessageReceived;
        VivoxService.Instance.ChannelMessageReceived -= OnChannelMessageReceived;
        VivoxService.Instance.ChannelMessageEdited -= OnChannelMessageEdited;
        VivoxService.Instance.ChannelMessageDeleted -= OnChannelMessageDeleted;

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
        EnterButton.onClick.RemoveAllListeners();
        MessageInputField.onValueChanged.RemoveAllListeners();
        MessageInputField.onEndEdit.RemoveAllListeners();
#endif
        m_TextChatScrollRect.onValueChanged.RemoveAllListeners();
    }

    void ClearMessageObjectPool()
    {
        foreach (KeyValuePair<string, MessageObjectUI> keyValuePair in m_MessageObjPool)
        {
            Destroy(keyValuePair.Value.gameObject);
        }

        m_MessageObjPool.Clear();
    }

    void ClearTextField()
    {
        MessageInputField.text = string.Empty;
        MessageInputField.Select();
        MessageInputField.ActivateInputField();
    }

    void EnterKeyOnTextField()
    {
        OnTyping?.Invoke(false);
        if ( !Input.GetKeyDown(KeyCode.Return) )
        {
            return;
        }

        
        SendMessage();
    }

    void SendMessage()
    {
        if ( string.IsNullOrEmpty(MessageInputField.text) )
        {
            return;
        }

        if ( string.IsNullOrEmpty(currentChannelName) )
        {
            Debug.LogError("CurrentChanel is empty");
            return;
        }

        VivoxService.Instance.SendChannelTextMessageAsync(currentChannelName, MessageInputField.text.Trim());
        ClearTextField();
    }

    private void ClearOldMessage()
    {
        foreach (Transform tfChild in ChatContentObj.transform)
        {
            Destroy(tfChild.gameObject);
        }
    }

    void SubmitTTSMessageToVivox()
    {
        if ( string.IsNullOrEmpty(MessageInputField.text) )
        {
            return;
        }

        VivoxService.Instance.TextToSpeechSendMessage(MessageInputField.text,
            TextToSpeechMessageType.RemoteTransmissionWithLocalPlayback);
        ClearTextField();
    }

    IEnumerator SendScrollRectToBottom()
    {
        yield return new WaitForEndOfFrame();

        // We need to wait for the end of the frame for this to be updated, otherwise it happens too quickly.
        m_TextChatScrollRect.normalizedPosition = new Vector2(0, 0);
        yield return null;
    }

    void OnDirectedMessageReceived(VivoxMessage message)
    {
        AddMessageToChat(message, false, true, prevDisplayName:_lastMessageDisplayName);
    }

    private string currentChannelName = string.Empty;
    public void ResetChannelName() =>  currentChannelName = string.Empty;

    public void OnChannelJoined(string channelName)
    {
        if ( channelName.Contains("audio") ) return;
        if ( channelName.Trim() != currentChannelName )
        {
            currentChannelName = channelName.Trim();
      
        }
        ClearMessageObjectPool();
        oldestMessage = null;
        _chatFrameTitle.text = GetDisplayName(currentChannelName);
        MessageInputField.enabled = false;
        FetchMessages = FetchHistory(true);
    }

    private string GetDisplayName(string chatRoomId)
    {
        var splits = chatRoomId.Split("_");
        if ( splits.Length == 2 )
        {
            return $"{splits[1]} Office";
        }

        if ( splits.Length == 3 )
        {
            return  $"Area 0{int.Parse(splits[2]) + 1}";
        }

        return splits.Last();
    }

    void OnChannelMessageReceived(VivoxMessage message)
    {
        AddMessageToChat(message, false, true, prevDisplayName:_lastMessageDisplayName);
    }

    private void OnChannelMessageEdited(VivoxMessage message)
    {
        var editedMessage = m_MessageObjPool?.FirstOrDefault(x => x.Key == message.MessageId).Value;
        // If we have the message that's been edited we will update if not we do nothing.
        editedMessage?.SetTextMessage(message);
    }

    private void OnChannelMessageDeleted(VivoxMessage message)
    {
        var editedMessage = m_MessageObjPool?.FirstOrDefault(x => x.Key == message.MessageId).Value;
        // If we have the message that's been deleted we will destroy it if not we do nothing.
        editedMessage?.SetTextMessage(message, true);
    }

    void AddMessageToChat(VivoxMessage message, bool isHistory = false, bool scrollToBottom = false, bool isLastMessage = false, string prevDisplayName = "")
    {
        var newMessageObj = Instantiate(MessageObject, ChatContentObj.transform);
        var newMessageTextObject = newMessageObj.GetComponent<MessageObjectUI>();
        if ( isHistory )
        {
            m_MessageObjPool.Insert(0,
                new KeyValuePair<string, MessageObjectUI>(message.MessageId, newMessageTextObject));
            newMessageObj.transform.SetSiblingIndex(0);
        }
        else
        {
            m_MessageObjPool.Add(new KeyValuePair<string, MessageObjectUI>(message.MessageId, newMessageTextObject));
        }

        newMessageTextObject.SetTextMessage(message, showName:message.SenderDisplayName != prevDisplayName);
        if ( scrollToBottom )
        {
            StartCoroutine(SendScrollRectToBottom());
        }

        _lastMessageDisplayName = message.SenderDisplayName;
    }
}