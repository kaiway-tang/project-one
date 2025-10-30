using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileEntity : HPEntity
{
    [SerializeField] protected Transform reflectionTrfm;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected float groundFriction, airFriction;
    public float weight = 1;
    public TerrainTrigger[] touchingTerrain; //0-4: ground, front, ceiling, backLow, backHigh
    bool noTerrainTriggers;
    protected bool facing = FACING_RIGHT;
    protected const bool FACING_LEFT = false, FACING_RIGHT = true;

    int gravityDisable; float defaultGravity;

    protected static Vector2 vect2; //passive vect2 to avoid declaring new Vector2 repeatedly
    protected static Vector3 vect3; // ^^

    protected new void Start()
    {
        if (touchingTerrain.Length < 1) { noTerrainTriggers = true; }
        base.Start();
    }
    protected new void FixedUpdate()
    {
        ApplyXFriction(IsTouchingGround() ? groundFriction : airFriction);
        base.FixedUpdate();
    }

    #region MobileEntity
    public void AddXVelocity(float amount, float max)
    {
        vect2 = rb.velocity;

        if (amount > 0)
        {
            if (vect2.x > max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x > max)
                {
                    vect2.x = max;
                }
            }
        }
        else
        {
            if (vect2.x < max)
            {
                return;
            }
            else
            {
                vect2.x += amount;
                if (vect2.x < max)
                {
                    vect2.x = max;
                }
            }
        }

        rb.velocity = vect2;
    }
    protected void SetXVelocity(float value)
    {
        vect2 = rb.velocity;
        vect2.x = value;
        rb.velocity = vect2;
    }
    public void AddForwardXVelocity(float amount, float max)
    {
        if (IsFacingLeft()) { AddXVelocity(-amount, -max); }
        else { AddXVelocity(amount, max); }
    }

    public void AddForwardVelocity(float x, float y)
    {
        if (IsFacingRight()) { vect2.x = x; }
        else { vect2.x = -x; }
        vect2.y = y;

        rb.velocity = vect2;
    }

    protected void AddYVelocity(float amount, float max)
    {
        vect2 = rb.velocity;
        vect2.y += amount;

        vect2 = rb.velocity;

        if (amount > 0)
        {
            if (vect2.y > max)
            {
                return;
            }
            else
            {
                vect2.y += amount;
                if (vect2.y > max)
                {
                    vect2.y = max;
                }
            }
        }
        else
        {
            if (vect2.y < max)
            {
                return;
            }
            else
            {
                vect2.y += amount;
                if (vect2.y < max)
                {
                    vect2.y = max;
                }
            }
        }

        rb.velocity = vect2;
    }
    protected void SetYVelocity(float value)
    {
        vect2 = rb.velocity;
        vect2.y = value;
        rb.velocity = vect2;
    }

    protected void ApplyXFriction(float amount)
    {
        vect2 = rb.velocity;

        if (vect2.x > 0)
        {
            vect2.x -= amount;
            if (vect2.x < 0)
            {
                vect2.x = 0;
            }
        }
        else
        {
            vect2.x += amount;
            if (vect2.x > 0)
            {
                vect2.x = 0;
            }
        }

        rb.velocity = vect2;
    }

    protected void ApplyYFriction(float amount)
    {
        vect2 = rb.velocity;

        if (vect2.y > 0)
        {
            vect2.y -= amount;
            if (vect2.y < 0)
            {
                vect2.y = 0;
            }
        }
        else
        {
            vect2.y += amount;
            if (vect2.y > 0)
            {
                vect2.y = 0;
            }
        }

        rb.velocity = vect2;
    }

    float magnitude, ratio;
    protected void ApplyDirectionalFriction(float amount)
    {
        if (Mathf.Abs(rb.velocity.x) > 0.0001f || Mathf.Abs(rb.velocity.y) > 0.0001f)
        {
            vect2.x = rb.velocity.x;
            vect2.y = rb.velocity.y;
            magnitude = vect2.magnitude;
            ratio = (magnitude - amount) / magnitude;

            if (ratio > 0)
            {
                vect2.x = rb.velocity.x * ratio;
                vect2.y = rb.velocity.y * ratio;

                rb.velocity = vect2;
            }
            else
            {
                vect2.x = 0;
                vect2.y = 0;

                rb.velocity = vect2;
            }
        }
        else
        {
            vect2.x = 0;
            vect2.y = 0;

            rb.velocity = vect2;
        }
    }

    protected void SetVelocity(float x, float y)
    {
        vect2.x = x; vect2.y = y;
        rb.velocity = vect2;
    }

    public bool IsFacingRight()
    {
        return facing;
    }
    public bool IsFacingLeft()
    {
        return !facing;
    }
    protected void FaceRight()
    {
        if (facingLock > 0) { return; }
        reflectionTrfm.localScale = Vector3.one;
        facing = FACING_RIGHT;
    }
    protected void FaceLeft()
    {
        if (facingLock > 0) { return; }
        vect2.x = -1; vect2.y = 1;
        reflectionTrfm.localScale = vect2;
        facing = FACING_LEFT;
    }
    int facingLock;
    protected void LockFacing(bool lockFacing)
    {
        if (lockFacing) {
            facingLock++;
            return;
        }
        facingLock--;
        if (facingLock < 0) { Debug.Log("facing lock < 0 wtf"); }
    }

    public bool IsTouchingGround()
    {
        if (noTerrainTriggers) { return false; }
        return touchingTerrain[0].IsTouchingGround();
    }

    protected void DisableGravity()
    {
        if (gravityDisable < 1)
        {
            defaultGravity = rb.gravityScale;
            rb.gravityScale = 0;
        }
        gravityDisable++;
    }
    protected void EnableGravity()
    {
        gravityDisable--;
        if (gravityDisable < 1)
        {
            rb.gravityScale = defaultGravity;
            gravityDisable = 0;
        }
    }

    #endregion

    #region HPEntity
    public override int TakeDamage(int damage, int p_entityID, Team p_team, int attackID)
    {
        return base.TakeDamage(damage, p_entityID, p_team, attackID);
    }
    public override Vector2 TakeKnockback(Vector2 kbVector, float influence)
    {
        vect2 = kbVector * ((1 - influence) / weight + influence);
        rb.velocity = vect2;
        return vect2;
    }
    #endregion
}