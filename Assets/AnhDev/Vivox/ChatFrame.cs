using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Sirenix.OdinInspector;
using TMPro;
using UI;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class ChatFrame : MonoBehaviour, IEnhancedScrollerDelegate
{
    private IList<KeyValuePair<string, MessageObjectUI>> m_MessageObjPool = new List<KeyValuePair<string, MessageObjectUI>>();


    #region Enhanced scroller
    [TableList]
    [SerializeField] private LinkedList<Data> _data;
    private float _totalCellSize = 0;
    private float _oldScrollPosition = 0;
    public EnhancedScroller scroller;
    public EnhancedScrollerCellView myTextCellViewPrefab;
    public EnhancedScrollerCellView otherTextCellViewPrefab;

    #endregion
    public Button EnterButton;
    public TMP_InputField MessageInputField;

    public GameObject ChannelEffectPanel;
    public Dropdown ChannelEffectDropdown;

    private Task FetchMessages = null;
    private DateTime? oldestMessage = null;

    private string _roomName;

    public void SetRoomName(string roomName)
    {
        _roomName = roomName;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() => VivoxService.Instance != null);
        
        scroller.Delegate = this;
        _data = new LinkedList<Data>();
        // ResizeScroller();

        MessageInputField.ActivateInputField();
        VivoxService.Instance.ChannelJoined += OnChannelJoined;
        VivoxService.Instance.DirectedMessageReceived += OnDirectedMessageReceived;
        VivoxService.Instance.ChannelMessageReceived += OnChannelMessageReceived;
        

#if !(UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID)
        MessageInputField.gameObject.SetActive(false);
        EnterButton.gameObject.SetActive(false);
#else
        EnterButton.onClick.AddListener(SendMessage);
        MessageInputField.onEndEdit.AddListener((string text) => { EnterKeyOnTextField(); });
        ChannelEffectDropdown.onValueChanged.AddListener(ChannelEffectValueChanged);
        AudioTapsManager.Instance.OnTapsFeatureChanged += OnAudioTapsManagerFeatureChanged;

        ChannelEffectPanel.gameObject.SetActive(AudioTapsManager.Instance.IsFeatureEnabled);
#endif
        // m_TextChatScrollRect.onValueChanged.AddListener(ScrollRectChange);
    }

    private void OnEnable()
    {
        ClearTextField();
    }

    private void OnDisable()
    {
        if (m_MessageObjPool.Count > 0)
        {
            ClearMessageObjectPool();
        }

        oldestMessage = null;
    }

    // private void ScrollRectChange(Vector2 vector)
    // {
    //     // Scrolled near end and check if we are fetching history already
    //     if (m_TextChatScrollRect.verticalNormalizedPosition >= 0.95f && FetchMessages != null && (FetchMessages.IsCompleted || FetchMessages.IsFaulted || FetchMessages.IsCanceled))
    //     {
    //         m_TextChatScrollRect.normalizedPosition = new Vector2(0, 0.8f);
    //         FetchMessages = FetchHistory(false);
    //     }
    // }

    private async Task FetchHistory(bool scrollToBottom = false)
    {
        try
        {
            var chatHistoryOptions = new ChatHistoryQueryOptions()
            {
                TimeEnd = oldestMessage,
            };
            var historyMessages =
                await VivoxService.Instance.GetChannelTextMessageHistoryAsync(_roomName, 10, chatHistoryOptions);
            var reversedMessages = historyMessages.Reverse();
            foreach (var historyMessage in reversedMessages)
            {
                // AddMessageToChat(historyMessage, true, scrollToBottom);
                _data.AddFirst(new Data()
                {
                    cellType = historyMessage.FromSelf ? Data.CellType.MyText : Data.CellType.OtherText,
                    timeStamp = historyMessage.ReceivedTime,
                    someText = historyMessage.MessageText
                });
            }

            // Update the oldest message ReceivedTime if it exists to help the next fetch get the next batch of history
            oldestMessage = historyMessages.FirstOrDefault()?.ReceivedTime;
            ResizeScroller(); 
        }
        catch (TaskCanceledException e)
        {
            Debug.Log($"Chat history request was canceled, likely because of a logout or the data is no longer needed: {e.Message}");
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

#if UNITY_STANDALONE || UNITY_IOS || UNITY_ANDROID
        EnterButton.onClick.RemoveAllListeners();
        MessageInputField.onEndEdit.RemoveAllListeners();
      
#endif
        // m_TextChatScrollRect.onValueChanged.RemoveAllListeners();
    }

    private void OnAudioTapsManagerFeatureChanged(bool enabled)
    {
        ChannelEffectPanel.gameObject.SetActive(enabled);
    }

    private void ChannelEffectValueChanged(int value)
    {
        AudioTapsManager.Instance.AddChannelAudioEffect((AudioTapsManager.Effects)value);
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
        if (!Input.GetKeyDown(KeyCode.Return))
        {
            return;
        }
        SendMessage();
    }

    void SendMessage()
    {
        if (string.IsNullOrEmpty(MessageInputField.text))
        {
            return;
        }

        VivoxService.Instance.SendChannelTextMessageAsync(_roomName, MessageInputField.text);
        ClearTextField();
    }
    
    IEnumerator SendScrollRectToBottom()
    {
        yield return new WaitForEndOfFrame();

        // We need to wait for the end of the frame for this to be updated, otherwise it happens too quickly.
        // m_TextChatScrollRect.normalizedPosition = new Vector2(0, 0);

        yield return null;
    }

    void OnDirectedMessageReceived(VivoxMessage message)
    {
        // AddMessageToChat(message, false, true);
    }

    void OnChannelJoined(string channelName)
    {
        Debug.Log($"[ChatFrame] Joined channel {channelName}");
        _roomName = channelName;
        FetchMessages = FetchHistory(true);
    }

    void OnChannelMessageReceived(VivoxMessage message)
    {
        Debug.Log($"[ChatFrame] Received Message {message.ChannelName} : {message.MessageText}");
        // AddMessageToChat(message, false, true);
        var cellType = message.FromSelf ? Data.CellType.MyText : Data.CellType.OtherText;

        AddNewRow(cellType, message.MessageText, message.ReceivedTime);
    }

    private void ResizeScroller(bool reloadData = true)
    {
        // capture the scroll rect size.
        // this will be used at the end of this method to determine the final scroll position
        var scrollRectSize = scroller.ScrollRectSize;

        // capture the scroller's position so we can smoothly scroll from it to the new cell
        var offset = _oldScrollPosition - scroller.ScrollSize;

        // capture the scroller dimensions so that we can reset them when we are done
        var rectTransform = scroller.GetComponent<RectTransform>();
        var size = rectTransform.sizeDelta;

        // set the dimensions to the largest size possible to acommodate all the cells
        rectTransform.sizeDelta = new Vector2(size.x, float.MaxValue);

        // calculate the total size required by all cells. This will be used when we determine
        // where to end up at after we reload the data on the second pass.
        _totalCellSize = scroller.padding.top + scroller.padding.bottom;
        for (var i = 1; i < _data.Count; i++)
        {
            _totalCellSize += _data.ElementAt(i).cellSize + (i < _data.Count - 1 ? scroller.spacing : 0);
        }
        
        // set the spacer to the entire scroller size.
        // this is necessary because we need some space to actually do a jump
        _data.ElementAt(0).cellSize = scrollRectSize;

        // reset the scroller size back to what it was originally
        rectTransform.sizeDelta = size;

        // reload the data with the newly set cell view sizes and scroller content size.
        //_calculateLayout = false;
        if (reloadData)
        {
            scroller.ReloadData();
        }

        // set the scroll position to the previous cell (plus the offset of where the scroller currently is) so that we can jump to the new cell.
        scroller.ScrollPosition = (_totalCellSize - _data.ElementAt(_data.Count - 1).cellSize) + offset;
    }
    
    
    public void AddNewRow(Data.CellType cellType, string text, DateTime timeStamp)
    {
        // first, clear out the cells in the scroller so the new text transforms will be reset
        scroller.ClearAll();

        _oldScrollPosition = scroller.ScrollPosition;

        // reset the scroller's position so that it is not outside of the new bounds
        scroller.ScrollPosition = 0;
        
        // now we can add the data row
        _data.AddLast(new Data()
        {
            cellType = cellType,
            someText = text,
            timeStamp = timeStamp
        });

        ResizeScroller();

        // jump to the end of the scroller to see the new content.
        // once the jump is completed, reset the spacer's size
        scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f, tweenType: EnhancedScroller.TweenType.easeInOutSine,
            tweenTime: 0.5f, jumpComplete:null);
    }
    
    
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _data.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return  _data.ElementAt(dataIndex).cellSize;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        ChatCellView cellView;


        if ( _data.ElementAt(dataIndex).cellType == Data.CellType.MyText )
        {
            cellView = scroller.GetCellView(myTextCellViewPrefab) as ChatCellView;
        }
        else
        {
            cellView = scroller.GetCellView(otherTextCellViewPrefab) as ChatCellView;
        }

        cellView.name = "Cell " + dataIndex.ToString();

        // initialize the cell's data so that it can configure its view.
        cellView.SetData(_data.ElementAt(dataIndex));
        

        return cellView;
    }
}
