using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerEnemy : Enemy
{
    [SerializeField] GameObject spawnEnemy;
    int attackCD;


    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        attackCD = Random.Range(20, 60);
    }

    // Update is called once per frame
    protected new void Update()
    {
        base.Update();
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        if (trackingPlayer)
        {
            if (attackCD > 0) attackCD--;
            else
            {
                Instantiate(spawnEnemy, trfm.position, trfm.rotation).GetComponentInChildren<MobileEntity>().SetVelocity(0, 30);
                attackCD = Random.Range(90, 150);
            }
        }
    }
}
