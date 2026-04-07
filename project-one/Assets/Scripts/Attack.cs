using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] HPEntity.Team team;
    [SerializeField] Collider2D selfCollider;
    [SerializeField] protected int damage;
    [SerializeField] protected HPEntity hpEntity;
    int enabledTicks;
    protected int attackID;

    [SerializeField] protected float influence;
    protected HPEntity colHPEntity;
    [SerializeField] float trauma, maxTrauma;

    [SerializeField] GameObject hitFX;
    [SerializeField] Collider2D[] ignoreColliders;
    public Enemy attacker;

    [SerializeField] protected int targetFollowing; //0: no following, 1: aerials only, 2: all following
    [SerializeField] protected Vector2 followingMultiplier;

    [SerializeField] int focus;

    protected void Start()
    {
        attackID = GameManager.GetAttackID();
        if (trauma < 0.001f) { trauma = GameManager.GetTrauma(damage); }
        if (maxTrauma < 0.001f) { maxTrauma = trauma * 1.2f; }
        if (selfCollider == null) { selfCollider = GetComponent<Collider2D>(); }
    }

    public void Activate(int duration)
    {
        selfCollider.enabled = true;
        enabledTicks = Mathf.Max(duration, enabledTicks);
        attackID = GameManager.GetAttackID();
    }

    public void Deactivate()
    {
        selfCollider.enabled = false;
        enabledTicks = 0;
    }

    protected void Update()
    {
        
    }

    protected void FixedUpdate()
    {
        if (enabledTicks > 0)
        {
            enabledTicks--;
            if (enabledTicks == 0)
            {
                selfCollider.enabled = false;
            }
        }
    }

    protected virtual int OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 9) { return DamageResult.IGNORED; }
        if (ignoreColliders != null)
        {
            for (int i = 0; i < ignoreColliders.Length; i++)
            {
                if (col == ignoreColliders[i]) { return DamageResult.IGNORED; }
            }
        }
        colHPEntity = col.GetComponent<HPEntity>();
        if (colHPEntity == hpEntity || team == colHPEntity.team || hpEntity == colHPEntity) { return DamageResult.IGNORED; }
        if (colHPEntity.team == HPEntity.Team.player && Player.evadeTimer > 0) {
            Player.Evade(attacker);
            return DamageResult.EVADED;
        }

        int result = colHPEntity.TakeDamage(damage, hpEntity.entityID, hpEntity.team, attackID);

        if (result != DamageResult.IGNORED)
        {
            if (team == HPEntity.Team.player) { Player.self.AddFocus(focus); }
            CinemachineController.AddTrauma(trauma, maxTrauma);
            if (hitFX != null)
            {
                Instantiate(hitFX, colHPEntity.trfm.position, Quaternion.identity);
                //Instantiate(hitFX, col.ClosestPoint(transform.position), Quaternion.identity);
            }
        }

        return result;
    }
}
