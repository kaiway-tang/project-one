using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D;
using UnityEngine;

public class TriangleEnemy : Enemy
{
    [SerializeField] GameObject triangleProjectile;
    [SerializeField] Transform spriteTrfm, firepointTrfm;
    [SerializeField] float rotateSpeed;
    int attackCD;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        attackCD = Random.Range(20, 60);
    }

    protected new void Update()
    {
        base.Update();
        GameManager.RotateTowards(spriteTrfm, Player.self.GetDistanceScalingPredictedPosition(trfm.position, 3), rotateSpeed * Time.deltaTime);
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        if (PlayerInTrackingRange())
        {            
            if (attackCD > 0) attackCD--;
            else
            {
                Instantiate(triangleProjectile, firepointTrfm.position, spriteTrfm.rotation).GetComponent<Projectile>().Initiate(this);
                attackCD = Random.Range(70, 110);
            }
        }
    }
}
