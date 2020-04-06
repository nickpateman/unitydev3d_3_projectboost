using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private AudioSource _audioSource;

    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        if(_rigidBody == null)
        {            
            throw new InvalidOperationException($"'{transform.parent.name}' does not have a rigid body component.");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            throw new InvalidOperationException($"'{transform.parent.name}' does not have a audio source component.");
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
            if (!_audioSource.isPlaying)
            {
                _audioSource.Play();
            }
        }
        else
        {
            _audioSource.Stop();
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
