using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;

public class FriendlistManager : MonoBehaviour {
    public static FriendlistManager _instance;

    public Transform friendlistParent;
    public FriendObject friendPrefab;
    List<FriendObject> friends;

    private void Awake() {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        friends = new List<FriendObject>();

        StartCoroutine(RefreshFriendList());
    }

    IEnumerator RefreshFriendList() {
        GetFriendList();

        yield return new WaitForSeconds(60f);

        StartCoroutine(RefreshFriendList());
    }

    void GetFriendList() {
        GetFriendsListRequest request = new GetFriendsListRequest();

        PlayerProfileViewConstraints pc = new PlayerProfileViewConstraints();
        pc.ShowLastLogin = true;
        pc.ShowTags = true;
        request.ProfileConstraints = pc;

        PlayFabClientAPI.GetFriendsList(request, (GetFriendsListResult result) => {
            foreach (FriendInfo friend in result.Friends) {
                InitFriend(friend);
            }
        }, (PlayFabError error) => { });
    }

    public void InitFriend(FriendInfo friend) {
        FriendObject fro = Instantiate(friendPrefab, friendlistParent);
        fro.Init(friend.Username, friend.Profile.LastLogin, friend.Tags, friend.Profile.PlayerId);

        friends.Add(fro);
    }

    public void AcceptFriendRequest(string playfabId) {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest {
            FunctionName = "AcceptFriendRequest",
            FunctionParameter = new { FriendPlayFabId = playfabId }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) => {
            RefreshFriendList();
        }, (PlayFabError obj) => { });
    }

    public void DeclineFriendRequest(string playfabId) {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest {
            FunctionName = "DeclineFriendRequest",
            FunctionParameter = new { FriendPlayFabId = playfabId }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) => {
            RefreshFriendList();
        }, (PlayFabError obj) => { });
    }

    public static void SendFriendRequest(string playfabId) {
        ExecuteCloudScriptRequest request = new ExecuteCloudScriptRequest {
            FunctionName = "SendFriendRequest",
            FunctionParameter = new { FriendPlayFabId = playfabId }
        };

        PlayFabClientAPI.ExecuteCloudScript(request, (ExecuteCloudScriptResult result) => {

        }, (PlayFabError obj) => { });
    }
}
