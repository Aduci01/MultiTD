using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class Tools {
    public static bool IsPointerOverUIObject() {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        return results.Count > 0;
    }

    public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition) {
        var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                                           screenPosition.y / Screen.height,
                                           0);
        return canvas.ViewportToCanvasPosition(viewportPosition);
    }

    public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition) {
        var canvasTransform = (RectTransform)canvas.transform;

        //Viewport positions ranges from 0 to 1
        //Being (0, 0) the bottom left corner
        //and (1, 1) the top right corner of the viewport.
        //
        //The sizeDelta of a canvas is the same as the screen resolution it is currently on.
        //In a screen of 800x600, the canvas would have a sizeDelta of (x:800, y:600) too!
        //
        //With that information, we can "cast" a value from a viewport to its relative position on an (Unscaled) canvas
        //Vector2.Scale or Vector3.Scale does just that (by simple multiplying each axis from A with the same axis from B)
        var scaled = Vector2.Scale(viewportPosition, canvasTransform.sizeDelta);

        //If a CanvasScaler is being used, by just doing the above is not enough.
        //By scaling the result from above (again) with the scale of the canvas we can fix the position to account for canvas scaling
        return Vector2.Scale(scaled, canvasTransform.localScale);
    }
}
