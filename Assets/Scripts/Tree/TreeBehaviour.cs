using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
    [Header("Apple")]
    [SerializeField] private Apple apple;
    [SerializeField] private GameObject applePrefab;
    [Header("Tree top")]
    [SerializeField] private GameObject treeTop;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private float dragRadius;
    [Header("Goal")]
    [SerializeField] private Transform goal;
    [Header("Leaves")]
    [SerializeField] private GameObject leavesExplosion;

    private TreeConstructor treeConstructor;
    private Spring topSpring;
    private Vector2 treeTopInitialPosition;
    private Vector2 treeTopTarget;
    private Vector2 appleInitialPosition;
    private Animator goalAnimator;

    private void OnDrawGizmosSelected()
    {
        if (treeTop)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(treeTopInitialPosition, dragRadius);
        }
    }

    private void Awake()
    {
        treeConstructor = GetComponent<TreeConstructor>();
        treeConstructor.TreeConstructedEvent += OnTreeConstructed;
    }

    private void Start()
    {
        goalAnimator = goal.GetComponent<Animator>();
    }

    private void Update()
    {
        if (apple && apple.AttachedToTree)
        {
            //Tree top follows the apple which is currently attached
            Vector2 deltaDistance = (Vector2)apple.transform.position - appleInitialPosition;
            if(deltaDistance.magnitude > dragRadius)
            {
                deltaDistance = deltaDistance.normalized * dragRadius;
            }
            treeTopTarget = treeTopInitialPosition + deltaDistance;
        }
        if (topSpring)
        {
            //Sets the tree top spring target
            topSpring.SetTarget(treeTopTarget);
        }
    }

    /// <summary>
    /// Happens when TreeConstructor finishes tree construction
    /// </summary>
    private void OnTreeConstructed()
    {
        SpawnApple();
        treeTop = treeConstructor.TreeTop;
        treeTopInitialPosition = treeTop.transform.position;
        treeTopTarget = treeTopInitialPosition;
        topSpring = treeTop.GetComponent<Spring>();
        if(topSpring != null )
        {
            topSpring.SetTarget(treeTopTarget);
        }
        AudioManager.instance.PlaySFX(SFXNames.AppleSpawn);
    }

    /// <summary>
    /// Spawns and apple,set its goal and listeners to apple events
    /// </summary>
    private void SpawnApple()
    {
        apple = Instantiate(applePrefab).GetComponent<Apple>();
        apple.transform.position = spawnLocations[Random.Range(0, spawnLocations.Length - 1)].position;
        appleInitialPosition = apple.transform.position;
        apple.SetGoal(goal);
        apple.AppleGrabbedEvent += OnAppleGrabbed;
        apple.AppleReleasedEvent += OnAppleReleased;
        apple.AppleUnattachedEvent += OnAppleUnattached;
    }

    /// <summary>
    /// Stops following the apple, shows VFX and SFX and creates another apple after 2 seconds
    /// </summary>
    /// <param name="apple"></param>
    private void OnAppleUnattached(Apple apple)
    {
        if(this.apple == apple)
        {
            this.apple = null;
        }
        treeTopTarget = treeTopInitialPosition;
        var explosion = Instantiate(leavesExplosion);
        explosion.transform.position = apple.transform.position;
        goalAnimator.SetBool("isShowed", true);
        AudioManager.instance.PlaySFX(SFXNames.TreeShake);
        Invoke(nameof(SpawnApple), 2f);
    }

    /// <summary>
    /// Listener when apple is released by the user. 
    /// Returns the tree to its original position if the apple was removed
    /// </summary>
    /// <param name="apple"></param>
    private void OnAppleReleased(Apple apple)
    {
        if (!apple.AttachedToTree)
        {
            treeTopTarget = treeTopInitialPosition;
        }
        goalAnimator.SetBool("isShowed", false);
    }

    /// <summary>
    /// Listener when apple is grabbed by the user
    /// </summary>
    /// <param name="apple"></param>
    private void OnAppleGrabbed(Apple apple)
    {
        if (apple.AttachedToTree)
        {
            appleInitialPosition = apple.transform.position;
        }
        else
        {
            goalAnimator.SetBool("isShowed", true);
        }
    }
}
