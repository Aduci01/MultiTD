﻿using UnityEngine;

[RequireComponent(typeof(Camera))]

public class RtsCamera : MonoBehaviour {
    public static RtsCamera _instance;

    private void Awake() {
        _instance = this;
    }

    public float ScreenEdgeBorderThickness = 5.0f; // distance from screen edge. Used for mouse movement

    [Header("Camera Mode")]
    [Space]
    public bool RTSMode = true;
    public bool FlyCameraMode = false;

    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")]
    [Space]
    public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 lenghtLimit;
    public Vector2 widthLimit;
    private Vector2 zoomLimit;
    public Vector2 rotXLimit;

    private float panSpeed;
    private Vector3 initialPos;
    private Vector3 panMovement;
    private Vector3 pos;
    private Quaternion rot;
    private bool rotationActive = false;
    private Vector3 lastMousePosition;
    private Quaternion initialRot;
    private float panIncrease = 0.0f;

    [Header("Rotation")]
    [Space]
    public bool rotationEnabled;
    public float rotateSpeed;

    [HideInInspector]
    public float offsetZ;
    public Vector2 vecZ;



    // Use this for initialization
    void Start() {
        initialPos = transform.position;
        initialRot = transform.rotation;

        zoomLimit.x = 9;
        zoomLimit.y = 30;
    }


    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (GameManager._instance.localPlayer != null)
                SetPositionToPlayer(GameManager._instance.localPlayer.id);
        }

        # region Camera Mode

        //check that ony one mode is choosen
        if (RTSMode == true) FlyCameraMode = false;
        if (FlyCameraMode == true) RTSMode = false;

        #endregion

        #region Movement

        panMovement = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness) {
            panMovement += Vector3.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y <= ScreenEdgeBorderThickness) {
            panMovement -= Vector3.forward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x <= ScreenEdgeBorderThickness) {
            panMovement += Vector3.left * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness) {
            panMovement += Vector3.right * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Q)) {
            panMovement += Vector3.up * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.E)) {
            panMovement += Vector3.down * panSpeed * Time.deltaTime;
        }

        if (RTSMode) transform.Translate(panMovement, Space.World);
        else if (FlyCameraMode) transform.Translate(panMovement, Space.Self);


        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q)
            || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness
            || Input.mousePosition.y <= ScreenEdgeBorderThickness
            || Input.mousePosition.x <= ScreenEdgeBorderThickness
            || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness) {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        } else {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }

        #endregion

        #region Zoom

        float posY = transform.position.y;
        posY -= Input.mouseScrollDelta.y * zoomSpeed;
        posY = Mathf.Clamp(posY, zoomLimit.x, zoomLimit.y);

        transform.position = new Vector3(transform.position.x, posY, transform.position.z);

        float lerpValue = (posY - zoomLimit.x) / (zoomLimit.y - zoomLimit.x);

        float angleX = Mathf.Lerp(rotXLimit.x, rotXLimit.y, lerpValue);
        transform.localEulerAngles = new Vector3(angleX, transform.localEulerAngles.y, transform.localEulerAngles.z);

        offsetZ = Mathf.Lerp(vecZ.x, vecZ.y, lerpValue);

        #endregion

        #region mouse rotation

        if (rotationEnabled) {
            // Mouse Rotation
            if (Input.GetMouseButton(0)) {
                rotationActive = true;
                Vector3 mouseDelta;
                if (lastMousePosition.x >= 0 &&
                    lastMousePosition.y >= 0 &&
                    lastMousePosition.x <= Screen.width &&
                    lastMousePosition.y <= Screen.height)
                    mouseDelta = Input.mousePosition - lastMousePosition;
                else {
                    mouseDelta = Vector3.zero;
                }
                var rotation = Vector3.up * Time.deltaTime * rotateSpeed * mouseDelta.x;

                transform.Rotate(rotation, Space.World);

                // Make sure z rotation stays locked
                rotation = transform.rotation.eulerAngles;
                rotation.z = 0;
                transform.rotation = Quaternion.Euler(rotation);
            }

            if (Input.GetMouseButtonUp(0)) {
                rotationActive = false;
                //if (RTSMode) transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, 0.5f * Time.time);
            }

            lastMousePosition = Input.mousePosition;

        }


        #endregion


        #region boundaries

        if (enableMovementLimits == true) {
            //movement limits
            pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, heightLimit.x, heightLimit.y);
            pos.z = Mathf.Clamp(pos.z, lenghtLimit.x, lenghtLimit.y);
            pos.x = Mathf.Clamp(pos.x, widthLimit.x, widthLimit.y);
            transform.position = pos;
        }



        #endregion

    }

    public void SetPositionToPlayer(int id) {
        Vector3 pos = GameManager._instance.playerStartTransforms[id].position;
        transform.position = new Vector3(pos.x, transform.position.y, pos.z - 7f);
    }

    public bool IsVisible(Vector3 pos) {
        bool vertical = pos.z > transform.position.z + 1 && pos.z < transform.position.z + 20;
        bool horizontal = pos.x > transform.position.x - 21 && pos.x < transform.position.x + 21;

        return vertical && horizontal;
    }

}