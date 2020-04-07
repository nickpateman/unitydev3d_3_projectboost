using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] float MovementFactor;
    [SerializeField] Vector3 MovementVector;
    [SerializeField] bool Enabled = true;
    [SerializeField] float Period = 2.0f;

    private const float _tau = Mathf.PI * 2;
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        if(Enabled)
        {
            float cycles = Time.time / Period;
            float rawSinWave = Mathf.Sin(cycles * _tau);
            MovementFactor = rawSinWave / 2f + 0.5f;
        }
        Vector3 offset = MovementVector * MovementFactor;
        transform.position = _startPosition + offset;
    }
}
