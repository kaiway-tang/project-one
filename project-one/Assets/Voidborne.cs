using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voidborne : Enemy
{
    [SerializeField] int attackCD, specialCD, tpCD;
    [SerializeField] float meleeRange, bladeBeamRange, spacingRange;
    [SerializeField] float accl, maxSpeed;

    int attackTimer, specialTimer;
    [SerializeField] DirectionalAttack basicAttack, specialAttack;

    [SerializeField] GameObject bladeBeam;
    [SerializeField] Transform bladebeamFirepoint;

    [SerializeField] VoidborneAnimator animator;
    [SerializeField] SpriteRenderer spriteRenderer;

    bool playerTracked;
    // Start is called before the first frame update
    protected new void Start()
    {
        base.Start();
        tpCD = Random.Range(180, 480);
    }

    // Update is called once per frame
    protected new void FixedUpdate()
    {
        base.FixedUpdate();
        ApplyXFriction();

        if (Mathf.Abs(rb.velocity.x) > 0.1f) { animator.RequestAnimatorState(animator.Run); }
        else { animator.RequestAnimatorState(animator.Idle); }

        if (tpTimer > 0)
        {
            rb.velocity = Vector3.zero;
            tpTimer--;
            if (tpTimer == 85)
            {
                spriteRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                hpBarFader.FadeTo(0);
            }
            if (tpTimer == 70)
            {
                SetIntangible(true);
                spriteRenderer.enabled = false;
                trfm.position = tpDestination;
            }
            if (tpTimer == 30)
            {                
                Instantiate(portal, trfm.position + Vector3.up * -0.5f, Quaternion.identity);
                SetIntangible(false);
                spriteRenderer.enabled = true;
            }
            if (tpTimer == 15) { spriteRenderer.maskInteraction = SpriteMaskInteraction.None; }
            if (tpTimer == 0)
            {
                EnableGravity();
            }
            return;
        }

        

        if (trackingPlayer)
        {
            if (!playerTracked) { playerTracked = true; }

            if (tpCD > 0) { tpCD--; }
            else if (!IsChanneling()) { Teleport(Player.self.GetPredictedPosition(0)); }

            if (!IsChanneling()) {
                FacePlayer();
            }

            if (attackCD > 0) { attackCD--; }
            if (specialCD > 0) { specialCD--; }

            if (attackTimer > 0)
            {
                attackTimer--;
                if (attackTimer == 2) { AddForwardXVelocity(maxSpeed * 1.5f, maxSpeed * 1.5f); }
                if (attackTimer == 0)
                {
                    basicAttack.Activate(8);
                }
            }
            if (specialTimer > 0)
            {
                specialTimer--;
                if (specialTimer == 0)
                {
                    specialAttack.Activate(8);
                    Instantiate(bladeBeam, bladebeamFirepoint.position, bladebeamFirepoint.rotation).GetComponent<Projectile>().Initiate(this);
                }
            }

            if (channelingTimer > 0) { return; }

            if (PlayerXDistance() > bladeBeamRange)
            {
                AddForwardXVelocity(accl, maxSpeed);
            } else
            {
                if (PlayerXDistance() < meleeRange)
                {
                    if (attackCD < 1)
                    {
                        attackCD = Random.Range(70, 110);
                        specialCD = attackCD;
                        animator.QueAnimation(animator.Attack1, 39);
                        SetChanneling(41);
                        attackTimer = 27;
                    }
                    else
                    {
                        AddForwardXVelocity(-accl, -maxSpeed);
                    }
                } else if (specialCD < 1 && PlayerXDistance() > spacingRange)
                {
                    attackCD = Random.Range(70, 110);
                    specialCD = Random.Range(200, 280);
                    animator.QueAnimation(animator.SpecialAttack, 51);
                    //blade beam
                    SetChanneling(53);
                    specialTimer = 39;
                } else if (attackCD < 1 && PlayerXDistance() > meleeRange)
                {
                    AddForwardXVelocity(accl, maxSpeed);
                } else if (PlayerXDistance() < spacingRange)
                {
                    AddForwardXVelocity(-accl, -maxSpeed);
                } else
                {

                }
            }
        } else if (playerTracked && !IsChanneling())
        {
            Teleport(Player.self.GetPredictedPosition(0));
        }
    }

    [SerializeField] GameObject portal;
    [SerializeField] Fader hpBarFader;
    Vector3 tpDestination;
    int tpTimer;
    void Teleport(Vector3 dest)
    {
        tpDestination = dest;        
        SetChanneling(100);
        tpTimer = 100;
        Instantiate(portal, trfm.position + Vector3.up * -0.56f, Quaternion.identity);

        DisableGravity();
        hpBarFader.FadeTo(0);
        tpCD = Random.Range(180, 480);
    }
}
