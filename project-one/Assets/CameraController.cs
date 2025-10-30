using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float lerpRate, rotationalLerpRate;
    public static Transform trfm;

    [SerializeField] float translationIntensity, rotationIntensity;

    public static CameraController self;
    Vector3 cameraTrackingVect3;

    // Start is called before the first frame update
    void Awake()
    {
        self = GetComponent<CameraController>();
        trfm = transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        trfm.position += ((target.position + Vector3.forward * - 10) - trfm.position) * lerpRate;
    }

    #region SCREEN_SHAKE
    [SerializeField] int trauma;
    public static void AddTrauma(int amount, int max = int.MaxValue)
    {
        self.trauma += amount;
        if (self.trauma > max)
        {
            self.trauma = max;
        }
    }
    public static void SetTrauma(int amount)
    {
        if (self.trauma < amount)
        {
            self.trauma = amount;
        }
    }

    float processedTrauma;
    Vector3 zVect3;
    void ProcessTrauma()
    {
        //rotational "recovery"
        if (trfm.localEulerAngles.z < .1f || trfm.localEulerAngles.z > 359.9f)
        {
            trfm.localEulerAngles = Vector3.zero;
        }
        else
        {
            if (trfm.localEulerAngles.z < 180) { zVect3.z = -trfm.localEulerAngles.z * rotationalLerpRate; }
            else { zVect3.z = (360 - trfm.localEulerAngles.z) * rotationalLerpRate; }
            trfm.localEulerAngles += zVect3;
        }

        //translational "recovery" (lerps rotation back to level)
        cameraTrackingVect3.x = (target.position.x - trfm.position.x) * lerpRate;
        cameraTrackingVect3.y = (target.position.y - trfm.position.y) * lerpRate;

        trfm.position += cameraTrackingVect3;

        //screen shake/rotation
        if (trauma > 0)
        {
            if (trauma > 48) //hard cap trauma at 40
            {
                processedTrauma = 2100;
            }
            else if (trauma > 30) //soft cap at 30 trauma
            {
                processedTrauma = 900 + 60 * (trauma - 30);
            }
            else
            {
                //amount of "shake" is proportional to trauma squared
                processedTrauma = trauma * trauma;
            }

            //generate random Translational offset for camera per tick
            cameraTrackingVect3 = Random.insideUnitCircle.normalized * translationIntensity * processedTrauma;
            trfm.position += cameraTrackingVect3;

            //generate random Rotational offset for camera per tick
            trfm.Rotate(Vector3.forward * rotationIntensity * (Random.Range(0, 2) * 2 - 1) * processedTrauma);

            //decrement trauma as a timer
            trauma--;
        }
    }
    #endregion
}
