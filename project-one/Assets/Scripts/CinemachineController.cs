using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class CinemachineController : MonoBehaviour
{
    [SerializeField] float traumaAdd;

    static CinemachineVirtualCamera cinecam;
    static CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] float amplitudeMultiplier;

    public static CinemachineController self;
    static float trauma;
   
    // Start is called before the first frame update
    void Awake()
    {
        self = this;
        cinecam = FindObjectOfType<CinemachineVirtualCamera>();
        noise = cinecam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // Update is called once per frame
    void Update()
    {
        if (trauma > 0)
        {
            noise.m_AmplitudeGain = trauma * trauma * amplitudeMultiplier;
            trauma -= Time.deltaTime;
            if (trauma < 0)
            {
                noise.m_AmplitudeGain = 0;
                trauma = 0;
            }
        }        

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Time.timeScale = 0.3f;
            }
            if (Input.GetKeyDown(KeyCode.Alpha0))
            {
                Time.timeScale = 1f;
            }
        } else
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AddTrauma(traumaAdd);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AddTrauma(traumaAdd * 2);
            }
        }
        
    }

    public static void AddTrauma(float amount, float max = Mathf.Infinity)
    {
        if (trauma < max)
        {
            trauma += amount;
            if (trauma > max)
            {
                trauma = max;
            }
        }
    }

    public static void SetTrauma(float amount)
    {
        if (trauma < amount)
        {
            trauma = amount;
        }
    }
}
