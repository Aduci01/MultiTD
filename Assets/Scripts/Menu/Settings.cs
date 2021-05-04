using System.Collections;
using System.Collections.Generic;
using PlayFab;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Settings : MonoBehaviour {
    public static Settings _instance;

    public GameObject settingsWindow;

    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;

    Resolution[] resolutions;

    private void Awake() {
        if (_instance != null) {
            Destroy(gameObject);
            return;
        }

        _instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentOption = 0;

        for (int i = 0; i < resolutions.Length; i++) {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height) {
                currentOption = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentOption;
        resolutionDropdown.RefreshShownValue();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            settingsWindow.SetActive(!settingsWindow.activeSelf);
        }
    }

    public void SetVolume(float v) {
        audioMixer.SetFloat("volume", v);
    }

    public void SetQuality(int index) {
        QualitySettings.SetQualityLevel(index);
    }

    public void SetFullScreen(bool b) {
        Screen.fullScreen = b;
    }

    public void SetResolution(int index) {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void Exit() {
        if (SceneManager.GetActiveScene().buildIndex == 2) {
            Client._instance.Disconnect();

            SceneManager.LoadScene(0);
        }

        if (SceneManager.GetActiveScene().buildIndex == 1)
            Application.Quit();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            Application.Quit();
    }

    public void LogOut() {
        PlayFabClientAPI.ForgetAllCredentials();
        PlayFabScript._instance.firstPlayfabCallRequested = false;

        Client._instance.Disconnect();
        SceneManager.LoadScene(0);

        settingsWindow.SetActive(false);
    }
}
