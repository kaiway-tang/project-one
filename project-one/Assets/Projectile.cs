using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Attack
{
    [SerializeField] float speed;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform spriteTrfm;
    [SerializeField] GameObject destroyFX;
    Transform trfm;

    private void Awake()
    {
        trfm = transform;
    }
    protected new void Start()
    {        
        base.Start();
    }

    protected new void Update()
    {
        base.Update();
        spriteTrfm.Rotate(Vector3.forward * 400f * Time.deltaTime);
        trfm.position += trfm.up * speed * Time.deltaTime;
    }

    public void Initiate(Enemy pAttacker)
    {
        attacker = pAttacker;
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected new int OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer != 9)
        {
            if (destroyFX != null)
            {
                Instantiate(destroyFX, trfm.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
        int damageResult = base.OnTriggerEnter2D(col);
        if (damageResult != DamageResult.IGNORED && damageResult != DamageResult.EVADED)
        {
            Destroy(gameObject);
        }
        return damageResult;
    }
}
