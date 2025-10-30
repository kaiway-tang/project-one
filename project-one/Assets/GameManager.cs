using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager self;

    #region ID_assignment
    static int entityIDAssign;
    public static int GetEntityID()
    {
        return entityIDAssign++;
    }
    static int attackIDAssign;
    public static int GetAttackID()
    {
        return attackIDAssign++;
    }
    #endregion

    private void Awake()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }    

    private void OnMouseUpAsButton()
    {
        
    }
}
