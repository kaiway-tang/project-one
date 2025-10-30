using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPEntity : MonoBehaviour
{
    public Team team;
    [SerializeField] protected int hp, maxHP;
    public Transform trfm;
    public int entityID;

    protected void Start()
    {
        entityID = GameManager.GetEntityID();
        if (!trfm) { trfm = transform; }
    }

    protected void FixedUpdate()
    {
        
    }

    public enum Team
    {
        player,
        enemy,
        neutral,
    }

    public virtual int TakeDamage(int damage, int p_entityID, Team p_team, int attackID)
    {
        if (team == p_team) { return DamageResult.IGNORED; }
        if (entityID == p_entityID) { return DamageResult.IGNORED; }
        hp -= damage;
        if (hp > 0)
        {
            return DamageResult.SURVIVED;
        }
        return DamageResult.DIED;
    }
    
    public virtual Vector2 TakeKnockback(Vector2 kbVector, float influence)
    {
        return Vector2.zero;
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}