using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRigidbody : MonoBehaviour
{
    public Vector2 Velocity => velocity;
    public float AngularVelocity => angularVelocity;
    public bool isUsingGravity;

    [SerializeField] private float gravity;
    [SerializeField] private float angularDamping;
    private float angularVelocity;
    private Vector2 velocity;
    private void Start()
    {
        velocity = Vector2.zero;
    }
    private void Update()
    {
        //Adds vertical acceleration (gravity)
        if (isUsingGravity)
        {
            velocity += Time.deltaTime * gravity * Vector2.down;
        }
        //Adds velocity to position
        Vector2 position = transform.position;
        position += velocity * Time.deltaTime;
        transform.position = position;

        //Deaccelerates the angular velocity
        angularVelocity *= angularDamping;

        //Adds angular velocity to rotation
        float z = transform.eulerAngles.z;
        z += angularVelocity * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0, 0, z);
    }

    /// <summary>
    /// Enable or disable gravity
    /// </summary>
    /// <param name="gravity"></param>
    public void SetGravity(bool gravity)
    {
        isUsingGravity = gravity;
    }

    /// <summary>
    /// Set rigidbody velocity
    /// </summary>
    /// <param name="velocity"></param>
    public void SetVelocity(Vector2 velocity)
    {
        this.velocity = velocity;
    }

    /// <summary>
    /// Accelerates angular velocity
    /// </summary>
    /// <param name="extraAngularVelocity"></param>
    public void AddAngularVelocity(float extraAngularVelocity)
    {
        this.angularVelocity += extraAngularVelocity;
    }
}
