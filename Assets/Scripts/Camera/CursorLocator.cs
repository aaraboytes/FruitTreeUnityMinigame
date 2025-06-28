using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLocator : MonoBehaviour
{
    public static CursorLocator instance;
    /// <summary>
    /// Returns the mouse position on the screen
    /// </summary>
    public Vector2 MousePosition
    {
        get
        {
            Vector3 mouseScreen = Input.mousePosition;

            if (float.IsInfinity(mouseScreen.x) || float.IsNaN(mouseScreen.x) ||
                float.IsInfinity(mouseScreen.y) || float.IsNaN(mouseScreen.y))
            {
                return Vector2.zero;
            }

            mouseScreen.z = 10f;

            return camera.ScreenToWorldPoint(mouseScreen);
        }
    }
    private Camera camera;

    private void Awake()
    {
        instance = this;
        camera = Camera.main;
    }
}
