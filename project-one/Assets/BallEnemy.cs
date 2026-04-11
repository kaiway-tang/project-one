using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEnemy : Enemy
{
    [SerializeField] int attackCD, attackTimer;
    [SerializeField] float jumpHeight, jumpDistance;
    [SerializeField] Attack attack;
    [SerializeField] SpriteRenderer attackSprite;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        attackCD = Random.Range(15, 90);
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        if (attackCD > 0) { attackCD--; }
        else
        {
            if (trackingPlayer)
            {
                attackCD = Random.Range(70,110);
                SetYVelocity(jumpHeight);
                SetXVelocity(jumpDistance * PlayerPredictedXDiff(30));
                attackTimer = 40;
            }            
        }

        if (attackTimer > 0)
        {
            attackTimer--;
            if (attackTimer == 6)
            {
                attack.Activate(6);
                attackSprite.enabled = true;
            }
            if (attackTimer == 0)
            {
                attackSprite.enabled = false;
            }
        }
    }
}
