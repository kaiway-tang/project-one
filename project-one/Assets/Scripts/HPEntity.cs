using System.Collections;
using System.Collections.Generic;
using System.Security.Authentication.ExtendedProtection;
using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public Team team;
    [SerializeField] protected int hp, maxHP;
    public Transform trfm;
    public int entityID;

    public GameObject hitFX, deathFX;
    [SerializeField] Collider2D[] hurtboxes;
    [SerializeField] HPBar hpBar;

    [SerializeField] int intangible;
    int lastAttackID;
    
    protected int stunDuration, stunCounter;

    protected void Start()
    {
        entityID = GameManager.GetEntityID();
        if (!trfm) { trfm = transform; }
        if (maxHP < hp) { maxHP = hp; }
        if (hpBar) { hpBar.Initiate(maxHP); }
    }

    protected void FixedUpdate()
    {
        if (stunDuration > 0)
        {
            stunDuration--;
        }
    }

    public enum Team
    {
        player,
        enemy,
        neutral,
    }

    public virtual bool IsTouchingGround()
    {
        return false;
    }

    public virtual int TakeDamage(int damage, int p_entityID, Team p_team, int attackID)
    {
        if (team == p_team || entityID == p_entityID || attackID == lastAttackID) { return DamageResult.IGNORED; }
        hp -= damage;
        if (attackID > 0) { lastAttackID = attackID; }

        if (hitFX != null)
        {
            Instantiate(hitFX, trfm.position, Quaternion.identity);
        }

        if (hpBar) { hpBar.SetHP(hp); }
        if (hp > 0)
        {             
            return DamageResult.SURVIVED;
        }
        Die();
        return DamageResult.DIED;
    }

    public virtual Vector2 TakeKnockback(Vector2 kbVector, float influence)
    {
        return Vector2.zero;
    }

    protected virtual void Die()
    {
        if (deathFX != null)
        {
            Instantiate(deathFX, trfm.position, Quaternion.identity);
        } else if (hitFX != null)
        {
            Instantiate(hitFX, trfm.position, Quaternion.identity);
        }
        CinemachineController.AddTrauma(GameManager.GetTrauma(maxHP) * .005f, GameManager.GetTrauma(maxHP) * .006f);
        Destroy(trfm.gameObject);
    }

    protected void SetIntangible(bool isIntangible)
    {
        if (isIntangible)
        {
            intangible++;
            for (int i = 0; i < hurtboxes.Length; i++)
            {
                hurtboxes[i].enabled = false;
            }
        }
        else
        {
            intangible--;
            if (intangible < 1)
            {
                for (int i = 0; i < hurtboxes.Length; i++)
                {
                    hurtboxes[i].enabled = true;
                }
                intangible = 0;
            }
        }
    }

    #region STUN
    protected void SetStun(bool stun)
    {
        if (stun)
        {
            stunCounter++;
        }
        else
        {
            stunCounter--;
            if (stunCounter < 1)
            {
                stunCounter = 0;
            }
        }
    }
    protected void Stun(int duration)
    {
        stunDuration = Mathf.Max(stunDuration, duration);
    }
    protected bool IsStunned()
    {
        return stunDuration > 0 || stunCounter > 0;
    }
    #endregion
}