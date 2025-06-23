using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TreeConstructor : MonoBehaviour
{
    public UnityAction TreeConstructedEvent;
    public GameObject TreeTop => treeTop;
    [Header("Tree parameters")]
    [SerializeField] private int totalJoins = 20;
    [SerializeField] private float height = 1f;
    [SerializeField] private GameObject treeTop;
    [Header("Components")]
    [SerializeField] private TrunkLineRenderer trunkLineRenderer;
    [SerializeField] private InverseKinematicsCreator inverseKinematicsCreator;

    [SerializeField] private List<Transform> joints = new List<Transform>();

    private GameObject treeTarget;
    private void Start()
    {
        Initialize();
        //Set up ik with trunk parts
        inverseKinematicsCreator.SetJoints(joints);
        inverseKinematicsCreator.Initialize();
        //Set the joints that are gonna be rendered
        trunkLineRenderer.SetJoints(joints);
        trunkLineRenderer.Initialize();
        //Set the tree top as the ik target
        inverseKinematicsCreator.SetTarget(treeTop.transform);

        TreeConstructedEvent?.Invoke();
    }

    /// <summary>
    /// Places all the joints depending the height of the trunk
    /// Places the tree top on the top of the trunk
    /// </summary>
    private void Initialize()
    {
        Vector2 position = transform.position;
        float deltaDistance = height / (float)totalJoins;
        for (int i = 0; i < totalJoins; i++)
        {
            var joint = new GameObject("Joint " + (i + 1));
            position += Vector2.up * deltaDistance;
            joint.transform.position = position;
            joint.transform.SetParent(transform);
            joints.Add(joint.transform);
        }
        if (treeTop)
        {
            treeTop.transform.position = joints[^1].transform.position;
        }
    }
}
