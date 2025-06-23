using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimpleRigidbody))]
public class Spring : MonoBehaviour
{
    [SerializeField] private Vector2 target;
    [Header("Spring physics")]
    [SerializeField] private float damping;
    [SerializeField] private float frequency;

    private SimpleRigidbody rigidbody;
    private void Awake()
    {
        rigidbody = GetComponent<SimpleRigidbody>();
    }
    private void Start()
    {
        target = transform.position;
    }
    private void Update()
    {
        //Spring physics
        Vector2 displacement = target - (Vector2)transform.position;
        Vector2 springForce = displacement * frequency;
        Vector2 velocity = rigidbody.Velocity;
        velocity += springForce * Time.deltaTime;
        velocity *= Mathf.Pow(damping, Time.deltaTime * 60f); //60 frame rate

        //Sets the rigidbody velocity
        rigidbody.SetVelocity(velocity);
    }

    /// <summary>
    /// Sets a point that the spring is attached to
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Vector2 target)
    {
        this.target = target;
    }
}
