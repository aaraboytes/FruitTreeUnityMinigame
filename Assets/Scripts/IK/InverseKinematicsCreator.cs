using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InverseKinematicsCreator : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private List<Transform> joints = new List<Transform>();

    [Header("Settings")]
    [SerializeField] private float tolerance;
    [SerializeField] private int maxIterations;
    [SerializeField] private float[] segmentLengths;
    [SerializeField] private Vector2[] positions;
    [SerializeField] private float totalLength;

    private void Update()
    {
        ResolveIK();
    }

    /// <summary>
    /// Set up the inverse kinematic chain
    /// </summary>
    public void Initialize()
    {
        int count = joints.Count;
        segmentLengths = new float[count - 1];
        positions = new Vector2[count];
        totalLength = 0;

        for (int i = 0; i < count; i++)
        {
            positions[i] = joints[i].position;
            if(i<count-1)
            {
                segmentLengths[i] = Vector2.Distance(joints[i].position, joints[i+1].position);
                totalLength += segmentLengths[i];
            }
        }
    }

    /// <summary>
    /// Resolve the inverse kinematics for each joint for a determined amount
    /// of iterations
    /// </summary>
    private void ResolveIK()
    {
        if(target == null || joints.Count <2) return;

        Vector2 targetPos = (Vector2)target.position;
        Vector2 rootPos = joints[0].position;

        //Checks if target is out of range
        if((targetPos - rootPos).sqrMagnitude > totalLength * totalLength)
        {
            //Extends in straight line
            for (int i = 0; i < joints.Count - 1; i++)
            {
                Vector2 dir = (targetPos - (Vector2)joints[i].position).normalized;
                joints[i + 1].position = (Vector2)joints[i].position + (dir * segmentLengths[i]);
            }
        }
        else
        {
            //Copy current positions
            for (int i = 0; i < joints.Count; i++)
            {
                positions[i] = joints[i].position;
            }

            int iteration = 0;
            float error = Vector2.Distance(positions[positions.Length - 1], targetPos);
            while(error > tolerance && iteration < maxIterations)
            {
                positions[positions.Length - 1] = targetPos;
                for (int i = joints.Count - 2; i >= 0;i--)
                {
                    Vector2 dir = (positions[i] - positions[i + 1]).normalized;
                    positions[i] = positions[i+1] +dir * segmentLengths[i];
                }

                positions[0] = rootPos;
                for (int i = 0; i < joints.Count - 1; i++)
                {
                    Vector2 dir = (positions[i + 1] - positions[i]).normalized;
                    positions[i+1] = positions[i] + dir * segmentLengths[i];
                }

                error = Vector2.Distance(positions[positions.Length - 1], targetPos);
                iteration++;
            }

            for (int i = 0; i < joints.Count; i++)
            {
                joints[i].position = positions[i];
            }

            for (int i = 0; i < joints.Count - 1; i++)
            {
                Vector2 dir = positions[i + 1] - positions[i];
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                joints[i].rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    /// <summary>
    /// Returns last joint of the chain
    /// </summary>
    /// <returns></returns>
    public Transform GetLastJoint()
    {
        if(joints!=null && joints.Count > 0)
        {
            return joints[^1];
        }
        return null;
    }

    /// <summary>
    /// Set a list of joints that are gonna be controlled
    /// </summary>
    /// <param name="joints"></param>
    public void SetJoints(List<Transform> joints)
    {
        this.joints = joints;
    }

    /// <summary>
    /// Set the target that the chains head is gonna follow
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
}
