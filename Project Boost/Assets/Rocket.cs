using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rigidBody;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if(_rigidBody == null)
        {
            throw new InvalidOperationException($"Parent of '{name}' does not have a rigid body component.");
        }
    }

    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        bool thrust = Input.GetKey(KeyCode.Space);
        if (thrust)
        {
            _rigidBody.AddRelativeForce(Vector3.up);
        }

        var rotateLeft = Input.GetKey(KeyCode.A);
        var rotateRight = Input.GetKey(KeyCode.D);
        if(!(rotateLeft && rotateRight))
        {
            if (rotateLeft)
            {
                transform.Rotate(Vector3.forward);
            }
            else if (rotateRight)
            {
                transform.Rotate(-Vector3.forward);
            }
        }
    }
}
