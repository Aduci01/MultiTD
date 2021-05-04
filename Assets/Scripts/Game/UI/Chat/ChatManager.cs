using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour {
    public static ChatManager _instance;

    public InputField inputField;

    [SerializeField] Text messagePrefab;
    [SerializeField] Transform chatWindowField;

    List<Text> messages;

    ProfanityFilter.ProfanityFilter filter = new ProfanityFilter.ProfanityFilter();

    private void Awake() {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        messages = new List<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.Return)) {
            if (inputField.text.Length > 0) {
                SendMessage();
            }

            inputField.ActivateInputField();
        }

    }

    public void SendMessage() {
        if (inputField.text.Length <= 0) return;

        ClientSend.SendChatMessage(inputField.text);
        inputField.text = "";
    }

    public void ReceiveMessage(short fromId, string msg) {
        PlayerManager pm = GameManager.players[fromId];

        Text newMsg = Instantiate(messagePrefab);

        msg = filter.CensorString(msg);
        newMsg.text = "<b><color=#" + ColorUtility.ToHtmlStringRGBA(pm.playerColor) + ">" + pm.playerName + ": " + "</color></b>" + msg;

        newMsg.gameObject.SetActive(true);
        newMsg.transform.SetParent(chatWindowField);
        newMsg.transform.localScale = Vector3.one;

        messages.Add(newMsg);

        if (messages.Count > 15) {
            Destroy(messages[0].gameObject);
            messages.RemoveAt(0);
        }
    }
}
