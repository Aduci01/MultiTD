using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the appearance of the chat messages
/// </summary>
public class ChatMessage : MonoBehaviour {
    public Text msg;

    public string room;
    public string player;
    public string text;
    public string type;
    public string playFabId;
    public bool isSent;

    public string tab;

    public ChatInputAnimation chatInput;

    private bool lastState;
    private float targetOpacity;
    private float scaleTimer;
    public float age;

    /// <summary>
    /// Text color and tags of messages
    /// </summary>
    public void Make() {
        msg.text = "";
        if (!player.Equals("")) {
            if (type == "private" && isSent) msg.text += "To ";
            else if (type == "private") msg.text += "From ";
            msg.text += player;
        }

        if (!room.Equals("")) msg.text += "[" + room + "]: ";
        else msg.text += ":";

        if (!text.Equals("")) msg.text += text;

        if (type == "server") msg.color = Color.yellow;
        if (type == "private") msg.color = Color.cyan;
        if (type == "guild") msg.color = Color.green;
    }


}
