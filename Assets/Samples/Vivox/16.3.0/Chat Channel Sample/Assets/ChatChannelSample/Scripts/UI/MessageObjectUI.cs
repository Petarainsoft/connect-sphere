using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.UI;

public class MessageObjectUI : MonoBehaviour
{
    public TMP_Text MessageText;
    public VerticalLayoutGroup LayoutGroup;


    private Color m_defaultTextColor = new Color(0.1960784f, 0.1960784f, 0.1960784f, 1f);
    private Color m_dangerTextColor = new Color(0.6705883f, 0, 0, 1f);
    private VivoxMessage m_vivoxMessage;

    public void SetTextMessage(VivoxMessage message, bool deleted = false, bool showName = true)
    {
        var updatedStatusMessage = deleted ? string.Format($"(Deleted) ") : string.Format($"(Edited) ");
        var editedText = m_vivoxMessage != null ? updatedStatusMessage : null;

        m_vivoxMessage = message;

        if (deleted)
        {
            MessageText.text = string.Format($"<color=#5A5A5A><size=8>{editedText}{message.ReceivedTime}</size></color>");
            return;
        }

        if (showName)
        {
            if ( message.FromSelf )
            {
                MessageText.alignment = TextAlignmentOptions.MidlineRight;
                LayoutGroup.padding.left = 80;
                LayoutGroup.padding.right = 0;
                MessageText.text =
                    string.Format(
                        $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>\n<margin-right=5%>{message.MessageText}\n<color=#5A5A5A><size=15>{editedText}<margin-right=5%>{message.ReceivedTime}</size></color>");
            }
            else
            {
                MessageText.text = string.IsNullOrEmpty(message.ChannelName)
                    ? string.Format(
                        $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>\n<margin-left=5%>{message.MessageText}</indent>\n<color=#5A5A5A><size=15>{editedText}<margin-left=5%>{message.ReceivedTime}</size></color>") // DM
                    : string.Format(
                        $"<size=35><color=#BFEFFF>{message.SenderDisplayName.Split("_").FirstOrDefault()}</color></size>\n<margin-left=5%>{message.MessageText}</indent>\n<color=#5A5A5A><size=15>{editedText}<margin-left=5%>{message.ReceivedTime}</size></color>"); // Channel Message
                MessageText.alignment = TextAlignmentOptions.MidlineLeft;
                LayoutGroup.padding.right = 80;
                LayoutGroup.padding.left = 0;
            }
        }
        else
        {
            if ( message.FromSelf )
            {
                LayoutGroup.padding.left = 80;
                LayoutGroup.padding.right = 0;
                MessageText.alignment = TextAlignmentOptions.MidlineRight;
                MessageText.text =
                    string.Format(
                        $"<margin-right=5%>{message.MessageText}\n<color=#5A5A5A><size=15><margin-right=5%>{editedText}{message.ReceivedTime}</size></color>");
            }
            else
            {
                LayoutGroup.padding.right = 80;
                LayoutGroup.padding.left = 0;
                MessageText.text = string.Format($"<margin-left=5%>{message.MessageText}\n<color=#5A5A5A><size=15><margin-left=5%>{editedText}{message.ReceivedTime}</size></color>"); // Channel Message
                MessageText.alignment = TextAlignmentOptions.MidlineLeft;
            }
        }
    }
}
