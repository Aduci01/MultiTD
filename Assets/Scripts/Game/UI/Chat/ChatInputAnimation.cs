using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ChatInputAnimation : MonoBehaviour {

    public bool showChat = false;

    [Header("InputField")]
    public RectTransform inputRectTransform;
    public InputField inputField;
    public Image inputBackground;
    public Text inputPlaceholder;
    public Vector2 inputBigSize;
    public Vector2 inputSmallSize;

    private Vector2 inputTargetSize;
    private float inputTargetImageOpacity;
    private float inputTargetTextOpacity;
    private float scaleTimer;

    [Header("Messages")]
    public Image messagesBackground;
    public Scrollbar scrollbar;

    private float messagesTargetImageOpacity;

    public bool inGame = false;

    private bool lastState;
    private EventSystem es;

    void Start() {
        es = FindObjectOfType<EventSystem>();

        lastState = false;

        //Input Defaults
        inputTargetSize = inputSmallSize;
        inputTargetImageOpacity = 0.25f;
        inputTargetTextOpacity = 0.5f;
        //Messages Defaults
        messagesTargetImageOpacity = 0f;

        scaleTimer = 0;

        //When the scrollview window is set to transparent it still blocks the raycasts
        messagesBackground.raycastTarget = false;
        messagesBackground.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
    }

    void Update() {
        if (Input.GetKey(KeyCode.Return) && !inputField.isFocused) {
            es.SetSelectedGameObject(inputField.gameObject);
        }

        bool state = inputField.isFocused || inputField.text != "";

        if (!showChat && (Input.anyKey && !Input.GetKey(KeyCode.Return)) && inGame) {
            state = false;
            es.SetSelectedGameObject(null);
        }

        if (lastState != state) {
            if (state) {
                showChat = true;

                //input
                inputTargetSize = inputBigSize;
                inputTargetImageOpacity = 1;
                inputTargetTextOpacity = 0;
                
                //messages
                messagesTargetImageOpacity = 1f;

                scaleTimer = 0;

                messagesBackground.raycastTarget = true;
                messagesBackground.transform.GetChild(0).GetComponent<Image>().raycastTarget = true;
            } else {
                showChat = false;

                //input
                inputTargetSize = inputSmallSize;
                inputTargetImageOpacity = 0.25f;
                inputTargetTextOpacity = 0.5f;

                //messages
                messagesTargetImageOpacity = 0f;

                scaleTimer = 0;

                messagesBackground.raycastTarget = false;
                messagesBackground.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
            }
        }

        scaleTimer += Time.deltaTime;
        Color targetColor;

        //changing input field to target variables
        inputRectTransform.sizeDelta = Vector2.Lerp(
            inputRectTransform.sizeDelta, inputTargetSize, smoothFunction(scaleTimer * 2)
        );

        float xPosition = ((inputRectTransform.sizeDelta.x+5) / 2 - 475);
        inputRectTransform.localPosition = new Vector3(
            xPosition,
            inputRectTransform.localPosition.y,
            inputRectTransform.localPosition.z
        );

        targetColor = inputPlaceholder.color;
        targetColor.a = inputTargetTextOpacity;
        inputPlaceholder.color = Color.Lerp(
            inputPlaceholder.color, targetColor, smoothFunction(scaleTimer * 2)
        );

        targetColor = inputBackground.color;
        targetColor.a = inputTargetImageOpacity;
        inputBackground.color = Color.Lerp(
            inputBackground.color, targetColor, smoothFunction(scaleTimer * 2)
        );

        //changing messages to target variables
        targetColor = messagesBackground.color;
        targetColor.a = messagesTargetImageOpacity;
        messagesBackground.color = Color.Lerp(
            messagesBackground.color, targetColor, smoothFunction(scaleTimer * 2)
        );

        if (messagesBackground.color.a == 1) {
            scrollbar.interactable = true;
        } else {
            scrollbar.interactable = false;
        }


        //saving state
        lastState = inputField.isFocused || inputField.text != "";
    }

    /// <summary>
    /// Helper function to smooth out movement
    /// </summary>
    private float smoothFunction(float t) {
        t = t * t * t * (t * (6f * t - 15f) + 10f);
        return t;
    }
}
