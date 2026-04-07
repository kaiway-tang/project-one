using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionalAttack : Attack
{
    [SerializeField] Vector2[] knockbacks;    
    protected int direction;

    public void Activate(int duration, int p_direction)
    {
        direction = p_direction;
        base.Activate(duration);
    }

    Vector2 kbResult;
    protected override int OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 9) {
            return DamageResult.IGNORED;
        }

        int result = base.OnTriggerEnter2D(col);

        if (result != DamageResult.IGNORED)
        {
            kbResult = colHPEntity.TakeKnockback(knockbacks[direction], influence);
            if ((targetFollowing == 1 && !hpEntity.IsTouchingGround()) || targetFollowing == 2) {
                kbResult.y = Mathf.Max(kbResult.y * followingMultiplier.y, 9);
                hpEntity.TakeKnockback(kbResult * followingMultiplier, influence);
            }
        }
        return result;
    }
}
;