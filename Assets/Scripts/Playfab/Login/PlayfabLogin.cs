using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System.Text;
using System.Collections;
using TMPro;

/// <summary>
/// Handle login scene events
/// </summary>
public class PlayfabLogin : MonoBehaviour {
    public LoginUi loginUi;
    public RegisterUi registerUi;

    public GameObject loginInProgress;

    public static GetPlayerCombinedInfoRequestParams loginInfoParams =
        new GetPlayerCombinedInfoRequestParams {
            GetUserAccountInfo = true,
            GetUserData = true,
            GetUserInventory = true,
            GetUserVirtualCurrency = true,
            GetUserReadOnlyData = true
        };

    public void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 20;

        //Filling login credentials based on local saves
        loginUi.username.text = PlayerPrefs.GetString("USERNAME", "");
        loginUi.password.text = PlayerPrefs.GetString("PW", "");

        //If already logged in -> go to Menu scene without any new login
        if (PlayFabClientAPI.IsClientLoggedIn()) {
            LoadingScreen._instance.StartLoading(5f, 1);

            PlayFabScript._instance.firstPlayfabCallRequested = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");

            return;
        } else {
            //Getting country and continent infos for matchmaking purposes
            StartCoroutine(LatencyHandler.SetCountry());
        }

    }

    public void Register() {
        if (!ValidateRegisterData()) return;


        var request = new RegisterPlayFabUserRequest {
            TitleId = "986BB",
            Email = registerUi.email.text,
            Password = registerUi.password.text,
            Username = registerUi.username.text,
            InfoRequestParameters = loginInfoParams,
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnLoginFailure);

        loginInProgress.SetActive(true);
    }

    bool ValidateRegisterData() {
        //Validating data
        if (!registerUi.email.text.Contains("@")) {
            MessageHandler._instance.ShowMessage("E-mail is not valid", 2f, Color.red);
            return false;
        }

        if (registerUi.email.text.Length < 5) {
            MessageHandler._instance.ShowMessage("E-mail is not valid", 2f, Color.red);
            return false;
        }

        if (registerUi.username.text.Length < 5) {
            MessageHandler._instance.ShowMessage("Username must be at least 5 characters", 2f, Color.red);
            return false;
        }

        if (registerUi.password.text.Length < 8) {
            MessageHandler._instance.ShowMessage("Password must be at least 8 characters", 2f, Color.red);
            return false;
        }

        if (!registerUi.password.text.Equals(registerUi.verifyPassword.text)) {
            MessageHandler._instance.ShowMessage("Password doesn't match Repeat password", 2f, Color.red);
            return false;
        }



        return true;
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result) {
        Debug.Log("Register Succes!");

        PlayerPrefs.SetString("USERNAME", registerUi.username.text);
        PlayerPrefs.SetString("PW", registerUi.password.text);

        PlayerData.playFabId = result.PlayFabId;
        PlayerData.playfabEntityID = result.EntityToken.Entity.Id;
        PlayerData.playfabEntityType = result.EntityToken.Entity.Type;

        LoadingScreen._instance.doneProgress++;

        StartCoroutine(DelayedLoading());
    }

    public void Login() {
        var request = new LoginWithPlayFabRequest {
            TitleId = "986BB",
            Password = loginUi.password.text,
            Username = loginUi.username.text,
            InfoRequestParameters = loginInfoParams,
        };

        PlayFabClientAPI.LoginWithPlayFab(request, OnLoginSuccess, OnLoginFailure);

        loginInProgress.SetActive(true);
    }

    private void OnLoginSuccess(LoginResult result) {
        Debug.Log("Login Succes!");

        PlayerPrefs.SetString("USERNAME", loginUi.username.text);
        PlayerPrefs.SetString("PW", loginUi.password.text);

        PlayerData.playFabId = result.PlayFabId;
        PlayerData.playfabEntityID = result.EntityToken.Entity.Id;
        PlayerData.playfabEntityType = result.EntityToken.Entity.Type;

        StartCoroutine(DelayedLoading());
    }

    IEnumerator DelayedLoading() {
        yield return new WaitForSeconds(2.5f);

        loginInProgress.SetActive(false);
        LoadingScreen._instance.StartLoading(4, 2);

        UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
    }

    private void OnLoginFailure(PlayFabError error) {
        Debug.LogError("Login failure: " + error.Error + "  " + error.ErrorDetails + error + "  " + error.ApiEndpoint + "  " + error.ErrorMessage);

        MessageHandler._instance.ShowMessage(error.ErrorMessage, 2f, Color.red);
        loginInProgress.SetActive(false);
    }

    void DeviceIDLogin() {
        string guestCustomID = SystemInfo.deviceUniqueIdentifier;

        /*string guestID;
        if (!PlayerPrefs.HasKey("GUEST_ID")) {
            StringBuilder sb = new StringBuilder("guest://");
            System.Random rnd = new System.Random();
            guestID = rnd.Next(100000, 1000000).ToString();
            sb.Append(guestID);
            sb.Append("/");
            sb.Append(DateTime.UtcNow.ToString());
            guestCustomID = sb.ToString();

            PlayerPrefs.SetString("GUEST_ID", guestCustomID);
        } else guestCustomID = PlayerPrefs.GetString("GUEST_ID");*/

        var request = new LoginWithCustomIDRequest {
            CustomId = guestCustomID,
            CreateAccount = true,
            InfoRequestParameters = loginInfoParams,
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    public void Exit() {
        Application.Quit();
    }

#if UNITY_ANDROID
    void AndroidLogin() {
        // The following grants profile access to the Google Play Games SDK.
        // Note: If you also want to capture the player's Google email, be sure to add
        // .RequestEmail() to the PlayGamesClientConfiguration
        /* PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
         .AddOauthScope("profile")
         .RequestServerAuthCode(false)
         .Build();
         PlayGamesPlatform.InitializeInstance(config);

         // recommended for debugging:
         PlayGamesPlatform.DebugLogEnabled = true;

         // Activate the Google Play Games platform
         PlayGamesPlatform.Activate();


         Social.localUser.Authenticate((bool success) => {

             if (success) {

                 var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                 Debug.Log("Server Auth Code: " + serverAuthCode);

                 PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest() {
                     TitleId = PlayFabSettings.TitleId,
                     ServerAuthCode = serverAuthCode,
                     CreateAccount = true
                 }, OnLoginSuccess, OnLoginFailure);
             } else {
                 Debug.Log("Failed to log in");
             }

         });*/
    }
#endif

#if UNITY_IOS
    void IOSLogin() {
        Social.localUser.Authenticate((bool success) => {

            if (success) {

                PlayFabClientAPI.LoginWithGameCenter(new LoginWithGameCenterRequest() {
                    TitleId = PlayFabSettings.TitleId,
                    CreateAccount = true,
                    PlayerId = Social.localUser.id
                }, OnLoginSuccess, OnLoginFailure);
            } else {
                Debug.Log("Failed to log in");
            }

        });
    }
#endif
}
