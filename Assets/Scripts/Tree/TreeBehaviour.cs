using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreeBehaviour : MonoBehaviour
{
    [Header("Apple")]
    [SerializeField] private GameObject applePrefab;
    [Header("Tree top")]
    [SerializeField] private GameObject treeTop;
    [SerializeField] private Transform[] spawnLocations;
    [SerializeField] private float dragRadius;
    [Header("Goal")]
    [SerializeField] private Transform goal;
    [Header("Leaves")]
    [SerializeField] private GameObject leavesExplosion;

    private Apple currentApple;
    private TreeConstructor treeConstructor;
    private Spring topSpring;
    private Vector2 treeTopInitialPosition;
    private Vector2 treeTopTarget;
    private Vector2 appleInitialPosition;
    private Animator goalAnimator;
    private IEnumerator spawnTask;
    private Dictionary<Transform, Apple> apples = new Dictionary<Transform, Apple>();

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
        spawnTask = SpawnMissingApples();
        treeConstructor = GetComponent<TreeConstructor>();
        treeConstructor.TreeConstructedEvent += OnTreeConstructed;
    }

    private void Start()
    {
        goalAnimator = goal.GetComponent<Animator>();
    }

    private void Update()
    {
        if (currentApple && currentApple.AttachedToTree)
        {
            //Tree top follows the apple which is currently attached
            Vector2 deltaDistance = (Vector2)currentApple.transform.position - appleInitialPosition;
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
        foreach(var apple in apples)
        {
            apple.Value.SetOriginPoint(apple.Key.position);
        }
    }

    /// <summary>
    /// Happens when TreeConstructor finishes tree construction
    /// </summary>
    private void OnTreeConstructed()
    {
        StartCoroutine(spawnTask);
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
    private Apple SpawnApple(Transform location)
    {
        var apple = Instantiate(applePrefab).GetComponent<Apple>();
        apple.transform.position = location.position;
        apple.SetGoal(goal);
        apple.AppleGrabbedEvent += OnAppleGrabbed;
        apple.AppleReleasedEvent += OnAppleReleased;
        apple.AppleUnattachedEvent += OnAppleUnattached;
        return apple;
    }

    /// <summary>
    /// Stops following the apple, shows VFX and SFX and creates another apple after 2 seconds
    /// </summary>
    /// <param name="apple"></param>
    private void OnAppleUnattached(Apple apple)
    {
        if(this.currentApple == apple)
        {
            this.currentApple = null;
        }
        treeTopTarget = treeTopInitialPosition;
        var explosion = Instantiate(leavesExplosion);
        explosion.transform.position = apple.transform.position;
        goalAnimator.SetBool("isShowed", true);
        AudioManager.instance.PlaySFX(SFXNames.TreeShake);
        foreach (var treeApple in apples.ToList())
        {
            if (apple == treeApple.Value)
            {
                apples.Remove(treeApple.Key);
            }
        }
        if (spawnTask != null)
        {
            StopCoroutine(spawnTask);
            spawnTask = SpawnMissingApples();
            StartCoroutine(spawnTask);
        }
    }

    /// <summary>
    /// Listener when apple is released by the user. 
    /// Returns the tree to its original position if the apple was removed
    /// </summary>
    /// <param name="apple"></param>
    private void OnAppleReleased(Apple apple)
    {
        currentApple = null;
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
        currentApple = apple;
        if (apple.AttachedToTree)
        {
            appleInitialPosition = apple.transform.position;
        }
        else
        {
            goalAnimator.SetBool("isShowed", true);
        }
    }
    /// <summary>
    /// Coroutine that spawn apples on missing spawn points.
    /// The apples are stored on a dictionary based on which point it was spawned
    /// </summary>
    private IEnumerator SpawnMissingApples()
    {
        if (apples.Count == spawnLocations.Length)
            yield break;
        yield return new WaitForSeconds(0.5f);
        Transform selectedSpawnPoint = null;
        if(apples.Count> 0)
        {
            //Finds which are the missing spawn points on the tree
            List<Transform> occupiedSpawnPoints = apples.Keys.ToList();
            List<Transform> missingSpawnPoints = spawnLocations.Except(occupiedSpawnPoints).ToList();
            selectedSpawnPoint = missingSpawnPoints[Random.Range(0, missingSpawnPoints.Count)];
        }
        else
        {
            selectedSpawnPoint = spawnLocations[Random.Range(0,spawnLocations.Length)];
        }
        Apple apple = SpawnApple(selectedSpawnPoint);
        apples[selectedSpawnPoint] = apple;
        if(apples.Count < spawnLocations.Length)
        {
            yield return new WaitForSeconds(1f);
            spawnTask = SpawnMissingApples();
            StartCoroutine(spawnTask);
        }
    }
}
