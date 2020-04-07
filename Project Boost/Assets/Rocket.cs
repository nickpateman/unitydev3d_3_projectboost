using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float RotationSpeed = 100f;
    [SerializeField] float ThrustSpeed = 200f;
    [SerializeField] AudioClip MainEngine;
    [SerializeField] AudioClip DeathSound;
    [SerializeField] AudioClip WinSound;

    private Rigidbody _rigidBody;
    private AudioSource _audioSource;
    private bool _applyThrust;
    private Collision _curCollision;
    private LandingPad _curLandingPad;
    private DateTime _startedLanding;
    private bool _ignoreInput;

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
        if(!_ignoreInput)
        {
            ProcessInput();
            ProcessCollision();
            ProcessLanding();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        _curCollision = collision;
    }

    void OnCollisionExit(Collision collision)
    {
        if (_curLandingPad != null)
        {
            _curLandingPad.Engaged = false;
            _curLandingPad = null;
        }
        _curCollision = null;
    }

    private void ProcessLanding()
    {
        if(_curLandingPad != null)
        {
            var elapsed = DateTime.Now - _startedLanding;
            if(elapsed > new TimeSpan(0, 0, 3))
            {
                _ignoreInput = true;
                _audioSource.PlayOneShot(WinSound);
                Invoke(nameof(NextScene), 3.0f);
            }
        }
    }

    private void NextScene()
    {
        var nextScene = SceneManager.GetActiveScene().buildIndex;
        if (SceneManager.GetActiveScene().buildIndex < (SceneManager.sceneCountInBuildSettings - 1))
        {
            nextScene += 1;
        }
        else
        {
            nextScene = 0;
        }
        SceneManager.LoadScene(nextScene);
    }

    private void PrevScene()
    {
        var prevScene = SceneManager.GetActiveScene().buildIndex;
        if (prevScene > 0)
        {
            prevScene -= 1;
        }
        SceneManager.LoadScene(prevScene);
    }

    private void ProcessCollision()
    {
        if(_curCollision != null)
        {
            switch (_curCollision.gameObject.tag)
            {
                case "Launching":
                    // do nothing
                    break;
                case "Fuel":
                    // refuel
                    break;
                case "Landing":
                    var curLandingPad = _curCollision.gameObject.GetComponent<LandingPad>();
                    if (curLandingPad != null)
                    {
                        curLandingPad.Engaged = IsOrientedInSimilarDirection(_curCollision.transform, 10.0f);
                        if(curLandingPad.Engaged && _curLandingPad == null)
                        {
                            _startedLanding = DateTime.Now;
                            _curLandingPad = curLandingPad;
                        }
                        else if(!curLandingPad.Engaged)
                        {
                            _curLandingPad = null;
                        }
                    }
                    break;
                default:
                    _ignoreInput = true;
                    _audioSource.Stop();
                    _audioSource.PlayOneShot(DeathSound);
                    Invoke(nameof(PrevScene), 3.0f);
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
                _audioSource.PlayOneShot(MainEngine);
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
