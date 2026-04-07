using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager self;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        { 
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    #region ID_assignment
    static int entityIDAssign;
    public static int GetEntityID()
    {
        return ++entityIDAssign;
    }
    static int attackIDAssign;
    public static int GetAttackID()
    {
        return ++attackIDAssign;
    }
    #endregion

    private void Awake()
    {
        transform.parent = null;
    }

    static Vector3 vec3;    
    public static void RotateTowards(Transform trfm, Vector3 target, float rotateSpeed)
    {
        trfm.rotation = Quaternion.RotateTowards(trfm.rotation, Quaternion.LookRotation(Vector3.forward, target - trfm.position), rotateSpeed);
        //trfm.up = Vector3.RotateTowards(trfm.up, target - trfm.position, rotateSpeed, 0f);
        //vec3.x = 0; vec3.y = 0; vec3.z = trfm.localEulerAngles.z;
        //trfm.localEulerAngles = vec3;
    }

    public static float GetTrauma(int damage)
    {
        return damage * 0.018f;
    }
}
