using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class FriendObject : MonoBehaviour {
    public TMP_Text nameText;
    public Image activityIndicator;

    string pfId;

    public void Init(string name, DateTime? lastLogin, List<string> tags, string id) {
        Debug.Log(lastLogin.Value.ToLongTimeString());

        pfId = id;
        nameText.text = name;
        activityIndicator.gameObject.SetActive(false);

        if (tags[0] == "requester") {

        } else if (tags[0] == "requestee") {

        } else {
            activityIndicator.gameObject.SetActive(true);
            if (lastLogin.Value.Hour < 3) {
                activityIndicator.color = new Color(activityIndicator.color.r, activityIndicator.color.g, activityIndicator.color.b, 1);
            } else activityIndicator.color = new Color(activityIndicator.color.r, activityIndicator.color.g, activityIndicator.color.b, 0.3f);
        }

        gameObject.SetActive(true);
    }

    public void AcceptRequest() {
        FriendlistManager._instance.AcceptFriendRequest(pfId);
    }

    public void DeclineRequest() {
        FriendlistManager._instance.DeclineFriendRequest(pfId);
    }
}
