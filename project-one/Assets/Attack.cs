using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] Collider2D col;
    [SerializeField] protected int damage;
    [SerializeField] protected HPEntity hpEntity;
    int enabledTicks;
    protected int attackID;

    [SerializeField] protected float influence;
    protected HPEntity colHPEntity;
    public void Activate(int duration)
    {
        col.enabled = true;
        enabledTicks = Mathf.Max(duration, enabledTicks);
        attackID = GameManager.GetAttackID();
    }

    public void Deactivate()
    {
        col.enabled = false;
        enabledTicks = 0;
    }

    protected void FixedUpdate()
    {
        if (enabledTicks > 0)
        {
            enabledTicks--;
            if (enabledTicks == 0)
            {
                col.enabled = false;
            }
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 9) { return; }        
        col.GetComponent<HPEntity>().TakeDamage(damage, hpEntity.entityID, hpEntity.team, attackID);
    }
}
