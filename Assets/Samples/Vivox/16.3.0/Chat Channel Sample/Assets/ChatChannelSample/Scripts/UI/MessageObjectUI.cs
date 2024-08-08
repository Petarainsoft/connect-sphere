using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ConnectSphere;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class MessageObjectUI : MonoBehaviour
{
    public TMP_Text MessageText;
    public TMP_Text Name;
    public VerticalLayoutGroup LayoutGroup;
    public PlayerInfoSO _playerInfo;
    public string email;

    public RectTransform rectTransform;

    private Color m_defaultTextColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
    private Color m_dangerTextColor = new Color(0.6705883f, 0, 0, 1f);

    private VivoxMessage m_vivoxMessage;
    
    private string ToUsername(string displayName)
    {
        var split = displayName.Split("_");
        if (split.Length == 3)
        {
            return $"{split[0]}@{split[1]}.{split[2]}";
        }
        
        return string.Empty;
    }

    public void SetTextMessage(VivoxMessage message, bool deleted = false, bool showName = true)
    {
        var updatedStatusMessage = deleted ? string.Format($"(Deleted) ") : string.Format($"(Edited) ");
        var editedText = m_vivoxMessage != null ? updatedStatusMessage : null;

        m_vivoxMessage = message;

        if (deleted)
        {
            MessageText.text = string.Format($"<color=#8C8B><size=8>{editedText}{message.ReceivedTime}</size></color>");
            return;
        }

        var username = ToUsername(message.SenderDisplayName);
        email = username;
        if (showName)
        {
            if ( message.FromSelf || username == _playerInfo.Email)
            {
                MessageText.alignment = TextAlignmentOptions.MidlineRight;
                LayoutGroup.padding.left = 80;
                LayoutGroup.padding.right = 0;
                if ( Name != null )
                {
                    Name.gameObject.SetActive(true);
                    Name.alignment = TextAlignmentOptions.MidlineRight;
                    Name.text = string.Format(
                        $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>");
                    MessageText.text =
                        string.Format(
                            $"<margin-right=5%>{message.MessageText}\n<color=#8C8B><size=15>{editedText}<margin-right=5%>{message.ReceivedTime}</size></color>");
                }
                else
                {
                    MessageText.text =
                        string.Format(
                            $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>\n<margin-right=5%>{message.MessageText}\n<color=#8C8B><size=15>{editedText}<margin-right=5%>{message.ReceivedTime}</size></color>");
                }
                

                ChangePivot(1);
            }
            else
            {
                if ( Name != null )
                {
                    Name.gameObject.SetActive(true);
                    Name.alignment = TextAlignmentOptions.MidlineLeft;
                    Name.text = string.Format(
                        $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>");
                }

                MessageText.text = string.Format(
                    $"<margin-left=5%>{message.MessageText}</indent>\n<color=#8C8B><size=15>{editedText}<margin-left=5%>{message.ReceivedTime}</size></color>"); // Channel Message
                MessageText.alignment = TextAlignmentOptions.MidlineLeft;
                LayoutGroup.padding.right = 80;
                LayoutGroup.padding.left = 0;
                

                ChangePivot(0);
            }
        }
        else
        {
            if ( Name != null )
            {
                Name.gameObject.SetActive(false);
            }
            if ( message.FromSelf || username == _playerInfo.Email )
            {
                LayoutGroup.padding.left = 80;
                LayoutGroup.padding.right = 0;
                MessageText.alignment = TextAlignmentOptions.MidlineRight;
                MessageText.text =
                    string.Format(
                        $"<margin-right=5%>{message.MessageText}\n<color=#8C8B><size=15><margin-right=5%>{editedText}{message.ReceivedTime}</size></color>");
                

                ChangePivot(1);
            }
            else
            {
                LayoutGroup.padding.right = 80;
                LayoutGroup.padding.left = 0;
                MessageText.text = string.Format($"<margin-left=5%>{message.MessageText}\n<color=#8C8B><size=15><margin-left=5%>{editedText}{message.ReceivedTime}</size></color>"); // Channel Message
                MessageText.alignment = TextAlignmentOptions.MidlineLeft;
                

                ChangePivot(0);
            }
        }
    }

    [Button]
    public void ChangePivot(int newPivotX)
    {
        // Calculate the difference in pivot
        Vector2 deltaPivot = rectTransform.pivot - new Vector2(newPivotX, rectTransform.pivot.y);

        // Calculate the offset
        Vector3 deltaPosition = new Vector3(deltaPivot.x * rectTransform.rect.width, deltaPivot.y * rectTransform.rect.height);

        // Adjust the position before changing the pivot
        rectTransform.localPosition -= deltaPosition;

        // Change the pivot
        rectTransform.pivot = new Vector2(newPivotX, rectTransform.pivot.y);
    }
}
