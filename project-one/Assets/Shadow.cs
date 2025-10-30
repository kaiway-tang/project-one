using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MobileEntity
{
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void SetVelocity(float x, float y)
    {
        base.SetVelocity(x, y);
    }
}
