using System.Collections;
using UnityEngine;

public class LandingPad : MonoBehaviour
{
    public string Key;
    public bool Engaged;
    private Light[] _lights;

    private void Start()
    {
        _lights = GetComponentsInChildren<Light>();
    }

    void Update()
    {
        //change colour of lights depending on engaged state
        if(_lights != null && _lights.Length > 0)
        {
            foreach(var curLight in _lights)
            {
                curLight.color = Engaged ? Color.green : Color.red;
            }
        }
    }
}
