using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using TMPro;
using UnityEngine;

namespace UI.Chat
{
    /// <summary>
    /// Simple example of one way a chat scroller could look
    /// </summary>
    public class MessageScroller : MonoBehaviour, IEnhancedScrollerDelegate
    {
        /// <summary>
        /// Internal representation of our data. Note that the scroller will never see
        /// this, so it separates the data from the layout using MVC principles.
        /// </summary>
        [SerializeField] private List<Data> _data;

        /// <summary>
        /// This stores the total size of all the cells,
        /// plus the scroller's top and bottom padding.
        /// This will be used to calculate the spacer required
        /// </summary>
        private float _totalCellSize = 0;

        /// <summary>
        /// Stores the scroller's position before jumping to the new chat cell
        /// </summary>
        private float _oldScrollPosition = 0;

        /// <summary>
        /// This is our scroller we will be a delegate for
        /// </summary>
        public EnhancedScroller scroller;

        /// <summary>
        /// The input field for texts from us
        /// </summary>
        public TMP_InputField myInputField;

        /// <summary>
        /// This will be the prefab of our chat cell
        /// </summary>
        public EnhancedScrollerCellView myTextCellViewPrefab;

        /// <summary>
        /// This will be the prefab of another person's chat cell
        /// </summary>
        public EnhancedScrollerCellView otherTextCellViewPrefab;

        /// <summary>
        /// This will be the prefab of our first cell to push the other cells to the bottom
        /// </summary>
        public EnhancedScrollerCellView spacerCellViewPrefab;

        /// <summary>
        /// The estimated width of each character. Note that this is just an estimate
        /// since most fonts are not mono-spaced.
        /// </summary>
        public int characterWidth = 8;

        /// <summary>
        /// The height of each character.
        /// </summary>
        public int characterHeight = 26;

        private int currentChatId = -1;

        void Start()
        {
            // set the application frame rate.
            // this improves smoothness on some devices
            Application.targetFrameRate = 60;

            // tell the scroller that this script will be its delegate
            scroller.Delegate = this;

            // set up a single data item containing the spacer
            // this pushes the cells down to the bottom
            _data = new List<Data>();
            _data.Add(new Data() { cellType = Data.CellType.Spacer });

            // call resize scroller to calculate and set up the scroll
            ResizeScroller();

            // focus on the chat input field
            myInputField.ActivateInputField();
        }

        /// <summary>
        /// Called every frame to check for return key presses.
        /// A return key press will send the chat
        /// </summary>
        void Update()
        {
            if ( Input.GetKeyDown(KeyCode.Return) && myInputField.isFocused )
            {
                SendButton_Click();
            }
        }

        /// <summary>
        /// This function adds a new record, resizing the scroller and calculating the sizes of all cells
        /// </summary>
        public void AddNewRow(Data.CellType cellType, string text)
        {
            // first, clear out the cells in the scroller so the new text transforms will be reset
            scroller.ClearAll();

            _oldScrollPosition = scroller.ScrollPosition;

            // reset the scroller's position so that it is not outside of the new bounds
            scroller.ScrollPosition = 0;


            // calculate the space needed for the text in the cell

            // get the estimated total width of the text (estimated because the font is assumed to be mono-spaced)
            float totalTextWidth = (float)text.Length*(float)characterWidth;

            // get the number of rows the text will take up by dividing the total width by the widht of the cell
            int numRows = Mathf.CeilToInt(totalTextWidth/scroller.GetComponent<RectTransform>().sizeDelta.x) + 1;

            // get the cell size by multiplying the rows times the character height
            var cellSize = numRows*(float)characterHeight;

            // now we can add the data row
            _data.Add(new Data()
            {
                cellType = cellType,
                cellSize = cellSize,
                someText = text
            });

            ResizeScroller();

            // jump to the end of the scroller to see the new content.
            // once the jump is completed, reset the spacer's size
            scroller.JumpToDataIndex(_data.Count - 1, 1f, 1f, tweenType: EnhancedScroller.TweenType.easeInOutSine,
                tweenTime: 0.5f, jumpComplete: ResetSpacer);
        }

        /// <summary>
        /// This function will expand the scroller to accommodate the cells, reload the data to calculate the cell sizes,
        /// reset the scroller's size back, then reload the data once more to display the cells.
        /// </summary>
        private void ResizeScroller()
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
                _totalCellSize += _data[i].cellSize + (i < _data.Count - 1 ? scroller.spacing : 0);
            }

            // set the spacer to the entire scroller size.
            // this is necessary because we need some space to actually do a jump
            _data[0].cellSize = scrollRectSize;

            // reset the scroller size back to what it was originally
            rectTransform.sizeDelta = size;

            // reload the data with the newly set cell view sizes and scroller content size.
            //_calculateLayout = false;
            scroller.ReloadData();

            // set the scroll position to the previous cell (plus the offset of where the scroller currently is) so that we can jump to the new cell.
            scroller.ScrollPosition = (_totalCellSize - _data[_data.Count - 1].cellSize) + offset;
        }

        /// <summary>
        /// This method is called when the new cell has been jumpped to.
        /// It will reset the spacer's cell size to the remainder of the scroller's size minus the
        /// total cell size calculated in ResizeScroller. Finally, it will reload the
        /// scroller to set the new cell sizes.
        /// </summary>
        private void ResetSpacer()
        {
            // reset the spacer's cell size to the scroller's size minus the rest of the cell sizes
            // (or zero if the spacer is no longer needed)
            _data[0].cellSize = Mathf.Max(scroller.ScrollRectSize - _totalCellSize, 0);

            // reload the data to set the new cell size
            scroller.ReloadData(1.0f);
        }

        #region UI Handlers

        /// <summary>
        /// Button handler sending message
        /// </summary>
        public void SendButton_Click()
        {
            // add a chat row from us
            // AddNewRow(Data.CellType.MyText, myInputField.text);
            // ApiManager.Inst.SocketIOApi.SendMessage(currentChatId, myInputField.text.Trim());

            // clear the input field and focus on it
            myInputField.text = "";
            myInputField.ActivateInputField();
        }

        #endregion

        #region EnhancedScroller Handlers

        /// <summary>
        /// This tells the scroller the number of cells that should have room allocated. This should be the length of your data array.
        /// </summary>
        /// <param name="scroller">The scroller that is requesting the data size</param>
        /// <returns>The number of cells</returns>
        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            // in this example, we just pass the number of our data elements
            return _data.Count;
        }

        /// <summary>
        /// Gets the cell view size for each cell
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <returns></returns>
        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            // return the cell size for each cell
            return _data[dataIndex].cellSize;
        }

        /// <summary>
        /// Reuse the appropriate cell
        /// </summary>
        /// <param name="scroller"></param>
        /// <param name="dataIndex"></param>
        /// <param name="cellIndex"></param>
        /// <returns></returns>
        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            ChatCellView cellView;

            if ( dataIndex == 0 )
            {
                cellView = scroller.GetCellView(spacerCellViewPrefab) as ChatCellView;
                cellView.name = "Spacer";
            }
            else
            {
                if ( _data[dataIndex].cellType == Data.CellType.MyText )
                {
                    cellView = scroller.GetCellView(myTextCellViewPrefab) as ChatCellView;
                }
                else
                {
                    cellView = scroller.GetCellView(otherTextCellViewPrefab) as ChatCellView;
                }
                
                cellView.name = "Cell " + dataIndex.ToString();

                // initialize the cell's data so that it can configure its view.
                cellView.SetData(_data[dataIndex]);
            }

            return cellView;
        }

        #endregion

        public async UniTaskVoid SetChatMessage(int chatId, List<AccountManagement.Message> chatAllMessage)
        {
            if ( chatAllMessage == null ) chatAllMessage = new List<AccountManagement.Message>();
            currentChatId = chatId;
            Debug.Log($"Set all messline {chatAllMessage.Count}");
            _data.RemoveRange(1, _data.Count - 1);

           
            
            for (int i = 0; i < chatAllMessage.Count; i++)
            {
                var ms = chatAllMessage[i];
                // _data.Add(new Data(){cellType = Data.CellType.Spacer});
                // var messType = ms.sender_id == ApiManager.Inst.UserId ? Data.CellType.MyText : Data.CellType.OtherText;
                var previousCellIsMyTextOrSpace = true;
                if (i > 0)
                {
                    // previousCellIsMyTextOrSpace = (chatAllMessage[i - 1].sender_id == ApiManager.Inst.UserId);
                }
                _data.Add(new Data()
                {
                    // cellType = messType,
                    someText = ms.content,
                    // ShowIcon = messType == Data.CellType.OtherText && previousCellIsMyTextOrSpace,
                    timeStamp = ms.timestamp,
                    cellSize = 130f
                });
            }


            ResizeScroller();
            await UniTask.WaitForSeconds(0.1f);
            scroller.ReloadData(1);
        }

        public void UpdateNewMessageLine(AccountManagement.Message messageLine)
        {
        }

        public async UniTaskVoid AddOtherMessage(int dataSenderID, AccountManagement.Message ms)
        {
            if ( _data.Count < 1 ) return;
            var lastData = _data.Last();
            // Debug.Log($"<color=red>sender {dataSenderID} | me is {ApiManager.Inst.UserId} content {ms.content}</color>");
            // var cellType = dataSenderID == ApiManager.Inst.UserId ? Data.CellType.MyText : Data.CellType.OtherText;
            _data.Add(new Data()
            {
                // cellType = cellType,
                someText = ms.content,
                cellSize = 130f,
                timeStamp = ms.timestamp,
                // ShowIcon = cellType == Data.CellType.OtherText && lastData.cellType == Data.CellType.MyText
            });

            try
            {
                // ResizeScroller();
                await UniTask.WaitForSeconds(0.1f);
                scroller.ReloadData(1);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}