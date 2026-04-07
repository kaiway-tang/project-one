using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    [SerializeField] int attackTmr;
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void FixedUpdate()
    {
        base.FixedUpdate();

        if (attackTmr > 0)
        {
            attackTmr--;
        }
    }

    protected bool PlayerInSight()
    {
        return !Tools.Linecast(trfm.position, Player.self.trfm.position, Tools.terrainLayerMask);
    }

    protected bool LineOfSight(Vector2 start, Vector2 end)
    {
        return !Tools.Linecast(start, end, Tools.terrainLayerMask);
    }
}
