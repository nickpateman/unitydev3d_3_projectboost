using System;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 100f;
    [SerializeField] float ThrustSpeed = 200f;

    private Rigidbody _rigidBody;
    private AudioSource _audioSource;
    private bool _applyThrust;
    private Collision _curCollision;

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
        ProcessCollision();
    }

    void OnCollisionEnter(Collision collision)
    {
        _curCollision = collision;
    }

    void OnCollisionExit(Collision collision)
    {
        if (_curCollision != null)
        {
            switch (_curCollision.gameObject.tag)
            {
                case "Fuel":
                    
                    break;
                case "Landing":
                    var landingPad = _curCollision.gameObject.GetComponent<LandingPad>();
                    if (landingPad != null) landingPad.Engaged = false;
                    break;
            }
        }
        _curCollision = null;
    }

    private void ProcessCollision()
    {
        if(_curCollision != null)
        {
            switch (_curCollision.gameObject.tag)
            {
                case "Fuel":
                    // refuel
                    break;
                case "Landing":
                    var landingPad = _curCollision.gameObject.GetComponent<LandingPad>();
                    if (landingPad != null) landingPad.Engaged = IsOrientedInSimilarDirection(_curCollision.transform, 10.0f);
                    break;
                default:
                    Debug.Log("Dying! @" + Environment.TickCount);
                    break;
            }
        }
    }

    private bool IsOrientedInSimilarDirection(
        Transform compareTransform,
        float allowance)
    {
        var landingBlockDirection = compareTransform.rotation.eulerAngles.z;
        var rocketDirection = transform.rotation.eulerAngles.z;
        var difference = rocketDirection - landingBlockDirection;
        if (difference < 0) difference = -difference;
        return difference <= allowance;
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
            float thrustSpeed = ThrustSpeed * Time.deltaTime;
            _rigidBody.AddRelativeForce(Vector3.up * thrustSpeed);
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
                float rotationSpeed = RotationSpeed * Time.deltaTime;
                if (rotateLeft)
                {
                    transform.Rotate(Vector3.forward * rotationSpeed);
                }
                else if (rotateRight)
                {
                    transform.Rotate(-Vector3.forward * rotationSpeed);
                }
            }
        }
        finally
        {
            _rigidBody.freezeRotation = false;
        }
    }
}
