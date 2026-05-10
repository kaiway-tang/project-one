using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MobileEntity
{
    [SerializeField] protected float trackingXDistance, trackingYDistance;
    static int terrainLayerMask = 1 << 6; //layer mask to only test for terrain collisions

    protected int channelingTimer;

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

        ApplyXFriction();
        tenthTimer--;
        if (tenthTimer <= 0) {
            TenthSec();
            tenthTimer = 6;
        }

        if (channelingTimer > 0) { channelingTimer--; }
    }

    int tenthTimer;
    protected void TenthSec()
    {
        trackingPlayer = PlayerInTrackingRange() && PlayerInSight();
    }

    protected void FacePlayer(int predictTicks = 0)
    {
        if (PlayerXDiff(0) > 0)
        {
            FaceRight();
        } else
        {
            FaceLeft();
        }
    }

    protected void SetChanneling(int ticks)
    {
        if (channelingTimer < ticks) { channelingTimer = ticks; }
    }

    protected bool IsChanneling() { return channelingTimer > 0; }

    #region PLAYER_INFO
    protected bool trackingPlayer;
    protected float PlayerXDiff(int predictTicks = 0)
    {
        return Player.self.GetPredictedPosition(predictTicks).x - trfm.position.x;
    }
    protected float PlayerPredictedXDiff(int ticks = 0)
    {
        return Player.self.GetPredictedPosition(ticks).x - trfm.position.x;
    }
    protected float PlayerXDistance(int predictTicks = 0)
    {
        return Mathf.Abs(Player.self.GetPredictedPosition(predictTicks).x - trfm.position.x);
    }
    protected float PlayerYDiff(int predictTicks = 0)
    {
        return Player.self.GetPredictedPosition(predictTicks).y - trfm.position.y;
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
