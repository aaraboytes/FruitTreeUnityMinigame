using UnityEngine;
[RequireComponent(typeof(Apple))]
public class AppleFace : MonoBehaviour
{
    [SerializeField] private Transform face;
    [SerializeField] private float faceRadius;
    private Vector2 origin;
    private Apple apple;
    private Vector2 mousePosition;
    private bool lookToCursor = true;
    private void OnDrawGizmosSelected()
    {
        if (face)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(face.position, faceRadius);
        }
    }
    private void Awake()
    {
        apple = GetComponent<Apple>();
    }

    private void Start()
    {
        origin = face.transform.localPosition;
        apple.AppleGrabbedEvent += OnAppleGrabbed;
        apple.AppleReleasedEvent += OnAppleReleased;
    }
    /// <summary>
    /// Moves the face in the cursor direction
    /// </summary>
    private void Update()
    {
        if (lookToCursor)
        {
            Vector2 dir = mousePosition - (Vector2)transform.position;
            face.transform.localPosition = origin + dir.normalized * faceRadius;
        }
    }
    /// <summary>
    /// Starts looking at the cursor when the user releases it
    /// </summary>
    private void OnAppleReleased(Apple apple)
    {
        lookToCursor = true;
    }
    /// <summary>
    /// The apple stops looking at the cursor when is grabbed
    /// </summary>
    private void OnAppleGrabbed(Apple apple)
    {
        lookToCursor = false;
        face.transform.localPosition = origin;
    }
    /// <summary>
    /// Receives the current mouse position from the apple
    /// </summary>
    /// <param name="mousePos"></param>
    public void SetMousePosition(Vector2 mousePos)
    {
        mousePosition = mousePos;
    }
}
