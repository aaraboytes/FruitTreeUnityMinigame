using UnityEngine;
using UnityEngine.Events;
public class Apple : MonoBehaviour
{
    public UnityAction<Apple> AppleGrabbedEvent;
    public UnityAction<Apple> AppleReleasedEvent;
    public UnityAction<Apple> AppleUnattachedEvent;
    public bool AttachedToTree => attachedToTree;
    [SerializeField] private bool attachedToTree;
    [SerializeField] private bool grabbed;

    [Header("Apple parameters")]
    [SerializeField] private float attachRadius;
    [SerializeField] private float resistanceTime;
    [SerializeField] private float resistanceMagnitude;

    [Header("Physics")]
    [SerializeField] SimpleRigidbody simpleRigidbody;
    [SerializeField] Spring spring;
    [SerializeField] float angularAcceleration;

    [Header("Goal")]
    [SerializeField] private Transform goal;
    [SerializeField] private float goalRadius;

    private int previousPitch = 0;
    private float maxPullRadius = 1;
    private Vector2 originPosition;
    private Vector2 lastFramePosition;
    private Animator animator;
    private AppleFace face;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(originPosition, attachRadius);
    }
    /// <summary>
    /// When user clics on apple.
    /// Disables the rigidbody gravity and current velocity
    /// Play sfx
    /// </summary>
    private void OnMouseDown()
    {
        grabbed = true;
        spring.enabled = true;
        if (simpleRigidbody.isUsingGravity)
        {
            simpleRigidbody.SetGravity(false);
            simpleRigidbody.SetVelocity(Vector2.zero);
        }
        lastFramePosition = transform.position;
        AudioManager.instance.PlaySFX(SFXNames.AppleGrab);
        AppleGrabbedEvent?.Invoke(this);
    }

    /// <summary>
    /// Reenables the physics just if the apple is not attached to the tree
    /// Checks if the apple is overlapping the goal
    /// Play SFX
    /// </summary>
    private void OnMouseUp()
    {
        grabbed = false;
        if (!attachedToTree)
        {
            simpleRigidbody.SetGravity(true);
            spring.enabled = false;
        }
        else
        {
            previousPitch = 0;
        }

        if (Vector2.Distance(transform.position, goal.transform.position) <= goalRadius)
        {
            if (attachedToTree)
            {
                attachedToTree = false;
                AppleUnattachedEvent?.Invoke(this);
            }
            enabled = false;
            simpleRigidbody.SetGravity(false);
            simpleRigidbody.SetVelocity(Vector2.zero); ;
            spring.SetTarget(goal.position);
            DissapearApple();
            AudioManager.instance.PlaySFX(SFXNames.AppleGoal);
        }
        else
        {
            AudioManager.instance.PlaySFX(SFXNames.AppleRelease);
        }

        AppleReleasedEvent?.Invoke(this);
    }
    private void Awake()
    {
        face = GetComponent<AppleFace>();
    }
    private void Start()
    {
        originPosition = transform.position;
        simpleRigidbody.SetGravity(false);
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Vector2 mousePosition = CursorLocator.instance.MousePosition;
        Vector2 target = originPosition;
        if (grabbed)
        {
            if (attachedToTree)
            {
                float distance = Vector2.Distance(mousePosition, originPosition);
                int pitch = Mathf.FloorToInt((distance / maxPullRadius) * 10f);
                if (pitch != previousPitch)
                {
                    previousPitch = pitch;
                    AudioManager.instance.PlayPitchedSfx(SFXNames.TreeResistance, 1f + (float)pitch * 0.1f);
                }
                //If the user is pulling the apple more than the attach radius...
                if (distance > attachRadius)
                {
                    attachedToTree = false;
                    AppleUnattachedEvent?.Invoke(this);
                }
            }
            else
            {
                //Adds angular velocity depending the cursor direction
                Vector2 dir = lastFramePosition - (Vector2)transform.position;
                float angularVelocity = dir.x > 0 ? 1 : -1;
                angularVelocity *= dir.magnitude * angularAcceleration;
                simpleRigidbody.AddAngularVelocity(angularVelocity);
            }
            //Apple spring follows the mouse position
            target = mousePosition;
        }
        //Sets the spring target
        spring.SetTarget(target);
        lastFramePosition = transform.position;

        if (face)
        {
            face.SetMousePosition(mousePosition);
        }
    }
    /// <summary>
    /// Set the apple goal to reach
    /// </summary>
    /// <param name="goal"></param>
    public void SetGoal(Transform goal)
    {
        this.goal = goal;
    }
    /// <summary>
    /// Plays the disappear animation
    /// </summary>
    public void DissapearApple()
    {
        animator.SetTrigger("Dissapear");
    }

    /// <summary>
    /// Sets new apple origin
    /// </summary>
    /// <param name="newOrigin"></param>
    public void SetOriginPoint(Vector2 newOrigin)
    {
        this.originPosition = newOrigin;
    }
}
