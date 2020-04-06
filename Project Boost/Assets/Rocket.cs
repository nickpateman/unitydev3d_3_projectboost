using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody _rigidBody;
    private AudioSource _audioSource;
    private bool _applyThrust;

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
        ApplyThrust();        
        ApplyRotation();
    }

    private void ApplyThrust()
    {
        _applyThrust = Input.GetKey(KeyCode.Space);
        if (_applyThrust)
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
    }

    private void ApplyRotation()
    {
        try
        {
            _rigidBody.freezeRotation = true;

            var rotateLeft = Input.GetKey(KeyCode.A);
            var rotateRight = Input.GetKey(KeyCode.D);
            if (_applyThrust && !(rotateLeft && rotateRight))
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
        finally
        {
            _rigidBody.freezeRotation = false;
        }
    }
}
