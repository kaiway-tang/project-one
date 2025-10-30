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

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 9) {
            Debug.Log("shouldnt be hitting: " + col.gameObject);
            return;
        }

        colHPEntity = col.GetComponent<HPEntity>();
        if (colHPEntity == hpEntity) { return; }

        colHPEntity.TakeDamage(damage, hpEntity.entityID, hpEntity.team, attackID);
        colHPEntity.TakeKnockback(knockbacks[direction], influence);
    }
}
;