using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class selfDest : MonoBehaviour
{
    [SerializeField] int ticks;

    private void FixedUpdate()
    {
        ticks--;
        if (ticks <= 0)
        {
            Destroy(gameObject);
        }

    }
}
