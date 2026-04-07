using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MobileEntity
{
    [SerializeField] protected float trackingXDistance, trackingYDistance;
    static int terrainLayerMask = 1 << 6; //layer mask to only test for terrain collisions
    protected new void Start()
    {
        base.Start();
        if (trackingYDistance < 0.001f) { trackingYDistance = trackingXDistance * 0.6f; }
    }

    protected void Update()
    {
        
    }

    protected new void FixedUpdate()
    {
        base.FixedUpdate();

        ApplyXFriction(IsTouchingGround() ? groundFriction : airFriction);
    }

    #region PLAYER_INFO
    protected float PlayerXDiff()
    {
        return Player.self.trfm.position.x - trfm.position.x;
    }
    protected float PlayerPredictedXDiff(int ticks)
    {
        return Player.self.GetPredictedPosition(ticks).x - trfm.position.x;
    }
    protected float PlayerXDistance()
    {
        return Mathf.Abs(Player.self.trfm.position.x - trfm.position.x);
    }
    protected float PlayerYDiff()
    {
        return Player.self.trfm.position.y - trfm.position.y;
    }

    protected bool TrackingPlayer()
    {
        return PlayerInTrackingRange() && PlayerInSight();
    }
    protected bool PlayerInTrackingRange()
    {
        return Mathf.Abs(PlayerXDiff()) < trackingXDistance && Mathf.Abs(PlayerYDiff()) < trackingYDistance;
    }
    protected bool PlayerInSight()
    {
        return !Physics2D.Linecast(trfm.position, Player.self.trfm.position, terrainLayerMask);
    }

    #endregion
}
