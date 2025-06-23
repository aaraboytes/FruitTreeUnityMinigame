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

    private float treeResistanceTimer;
    private int previousPitch = 0;
    private Camera camera;
    private Vector2 initialPosition;
    private Vector2 lastFramePosition;
    private Animator animator;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(initialPosition, attachRadius);
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

        if (Vector2.Distance(transform.position, goal.transform.position) <= goalRadius)
        {
            if (attachedToTree)
            {
                attachedToTree = false;
                AppleUnattachedEvent?.Invoke(this);
            }
            enabled = false;
            simpleRigidbody.SetGravity(false);
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
    private void Start()
    {
        camera = Camera.main;
        initialPosition = transform.position;
        simpleRigidbody.SetGravity(false);
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Vector2 mousePosition = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 target = initialPosition;
        if (grabbed)
        {
            float distance = Vector2.Distance(mousePosition, initialPosition);

            //If the user is pulling the apple more than the attach radius...
            if (attachedToTree && distance > attachRadius)
            {
                //The apple is gonna be placed at the increasing limit of the radius
                float extraRadius = (treeResistanceTimer / resistanceTime) * resistanceMagnitude;
                target = initialPosition + ((mousePosition - initialPosition).normalized) * ( attachRadius + extraRadius);
                treeResistanceTimer += Time.deltaTime;
                if (treeResistanceTimer > resistanceTime)
                {
                    attachedToTree = false;
                    AppleUnattachedEvent?.Invoke(this);
                }

                //SFX depending on pulling time
                int pitch = Mathf.FloorToInt((treeResistanceTimer / resistanceTime) * 10f);
                if (pitch != previousPitch)
                {
                    previousPitch = pitch;
                    AudioManager.instance.PlayPitchedSfx(SFXNames.TreeResistance, 1f + (float)pitch * 0.1f);
                }
            }
            else
            {
                //Apple spring follows the mouse position
                target = mousePosition;
            }

            //Adds angular velocity depending the cursor direction
            Vector2 dir = lastFramePosition - (Vector2)transform.position;
            float angularVelocity = dir.x > 0 ? 1 : -1;
            angularVelocity *= dir.magnitude * angularAcceleration;
            simpleRigidbody.AddAngularVelocity(angularVelocity);
        }
        //Sets the spring target
        spring.SetTarget(target);
        lastFramePosition = transform.position;
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
}
