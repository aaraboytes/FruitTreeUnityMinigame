using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrunkLineRenderer : MonoBehaviour
{
    [SerializeField] private List<Transform> joints;
    private LineRenderer lineRenderer;
    private void Update()
    {
        //Set all the joints positions to the line renderer positions
        if (lineRenderer && lineRenderer.positionCount > 0) {
            lineRenderer.SetPosition(0, transform.position);
            for (int i = 1; i < lineRenderer.positionCount; i++)
            {
                lineRenderer.SetPosition(i, joints[i - 1].position);
            }
        }
    }
    /// <summary>
    /// Set up the line renderer
    /// </summary>
    public void Initialize()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = joints.Count + 1;
        lineRenderer.useWorldSpace = true;
    }
    /// <summary>
    /// Sets a list of transform that are gonna be rendered
    /// </summary>
    /// <param name="joints"></param>
    public void SetJoints(List<Transform> joints)
    {
        this.joints = joints;
    }
}
