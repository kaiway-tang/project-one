using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MobileEntity
{
    [SerializeField] float groundSpeed, airSpeed, maxSpeed, jumpPower;
    bool hasDJump;

    [SerializeField] GameObject shadowObj;
    [SerializeField] Shadow shadow;

    [SerializeField] BaseAnimator basicAttackAnimator;
    [SerializeField] DirectionalAttack basicAttack;
    int basicAttackTimer, basicAttackCD;

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        shadow.trfm.parent = null;
    }

    private void Update()
    {
        if (InputManager.AttackPressed() && basicAttackCD < 1)
        {            
            basicAttackAnimator.Play();
            basicAttackCD = 30;
            basicAttackTimer = 18;
            LockFacing(true);
        }

        if (InputManager.JumpPressed())
        {
            if (IsTouchingGround())
            {
                SetYVelocity(jumpPower);
            } else if (hasDJump)
            {
                SetYVelocity(jumpPower);
                hasDJump = false;
            }
        }

        if (InputManager.ShadowPressed())
        {
            vect2 = shadow.trfm.position;
            shadow.trfm.position = trfm.position;
            trfm.position = vect2;
            shadow.SetVelocity(rb.velocity.x, rb.velocity.y);
        }
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        Movement_FixedUpdate();
        Abilities_FixedUpdate();
    }

    #region Movement
    void Movement_FixedUpdate()
    {
        if (IsTouchingGround())
        {
            hasDJump = true;
        }

        if (InputManager.LeftHeld())
        {
            if (!InputManager.RightHeld())
            {
                FaceLeft();
                AddXVelocity(IsTouchingGround() ? -groundSpeed : -airSpeed, -maxSpeed);
            }
        } else if (InputManager.RightHeld())
        {
            FaceRight();
            AddXVelocity(IsTouchingGround() ? groundSpeed : airSpeed, maxSpeed);
        } else
        {
            
        }
    }
    #endregion

    #region Abilities
    void Abilities_FixedUpdate()
    {
        if (basicAttackTimer > 0)
        {
            basicAttackTimer--;
            if (basicAttackTimer == 15)
            {
                basicAttack.Activate(12, IsFacingRight() ? 0 : 1);
            }
            if (basicAttackTimer == 0)
            {
                LockFacing(false);
            }
        }
        if (basicAttackCD > 0) { basicAttackCD--; }
    }
    #endregion
}
