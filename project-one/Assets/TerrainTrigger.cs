using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTrigger : MonoBehaviour
{
    [SerializeField] int touchCounter;
    public bool IsTouchingGround()
    {
        return touchCounter > 0;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        touchCounter++;
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        touchCounter--;
    }    
}
