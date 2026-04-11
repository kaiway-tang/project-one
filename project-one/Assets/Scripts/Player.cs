using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MobileEntity
{
    [SerializeField] float groundSpeed, airSpeed, maxSpeed, jumpPower;
    [SerializeField] float wallJumpXPower, wallJumpYPower;
    int wallJumpWindow;
    bool hasDJump, hadDJump;

    [SerializeField] GameObject wakizashi;

    [SerializeField] BaseAnimator basicAttackAnimator, basicAttack1Animator;
    [SerializeField] DirectionalAttack basicAttack, basicAttack1;
    int basicAttackTimer, basicAttackCD, basicAttackComboWindow;
    [SerializeField] float attackStepPower, aerialAttackStepPower;

    int blinkCD, blinkTimer;
    [SerializeField] float blinkPower;
    [SerializeField] int blinkDuration;
    [SerializeField] GameObject dashPoofFX;
    [SerializeField] SpriteRenderer[] sprites;
    Vector2 blinkExitVelocity;

    [SerializeField] GameObject evadeObj;
    int evadeCD;
    public static int evading, evadeTimer;

    int eviscerateCD, eviscerateTimer;
    [SerializeField] float eviscerateSpeed, eviscerateExitVelocity, eviscerateDrag;
    [SerializeField] BaseAnimator eviscerateAnimator;
    [SerializeField] DirectionalAttack eviscerateAttack, empoweredEviscerateAttack;
    [SerializeField] ParticleSystem evisceratePtcl;

    [SerializeField] int focus, maxFocus;
    [SerializeField] HPBar focusBar;

    [SerializeField] Fader focusFader, damageFader;

    public static Player self;

    private void Awake()
    {
        self = this;
    }
    new void Start()
    {
        base.Start();
        trackedPositions = new Vector3[30];
        focusBar.Initiate(100, 0);
    }

    private void Update()
    {
        HandleAttack();
        HandleJump();
        HandleBlink();
        HandleEvade();
        HandleEviscerate();
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
        Movement_FixedUpdate();
        Abilities_FixedUpdate();
        PositionTracking_FixedUpdate();
    }

    #region Movement
    void Movement_FixedUpdate()
    {
        if (IsStunned())
        {
            ApplyXFriction(IsTouchingGround() ? groundFriction : airFriction);
            return;
        }

        if (wallJumpWindow > 0)
        {
            if (backTouchingTerrain() && MovingInFacingDirection())
            {
                DoWallJump();
                if (hadDJump)
                {
                    hasDJump = true;
                }
            }
            wallJumpWindow--;
        }

        if (IsTouchingGround())
        {
            hasDJump = true;

            if (basicAttackTimer < 5)
            {
                if (InputManager.LeftHeld())
                {
                    if (!InputManager.RightHeld())
                    {
                        FaceLeft();
                        AddXVelocity(-groundSpeed, -maxSpeed);
                    }
                }
                else if (InputManager.RightHeld())
                {
                    FaceRight();
                    AddXVelocity(groundSpeed, maxSpeed);
                }
            }
        }
        else
        {
            if (InputManager.LeftHeld())
            {
                if (!InputManager.RightHeld())
                {
                    FaceLeft();
                    AddXVelocity(-airSpeed, -maxSpeed);
                }
            }
            else if (InputManager.RightHeld())
            {
                FaceRight();
                AddXVelocity(airSpeed, maxSpeed);
            }
        }

        ApplyXFriction(IsTouchingGround() ? groundFriction : airFriction);
    }

    void DoWallJump()
    {
        SetXVelocity(IsFacingLeft() ? -wallJumpXPower : wallJumpXPower);
        if (wallJumpWindow > 0)
        {
            SetYVelocity(wallJumpYPower - 4 + wallJumpWindow);
        } else
        {
            SetYVelocity(wallJumpYPower);
        }        
    }

    bool MovingInFacingDirection()
    {
        if (InputManager.LeftHeld() && InputManager.RightHeld()) { return false; }
        return (IsFacingLeft() && InputManager.LeftHeld()) || (IsFacingRight() && InputManager.RightHeld());
    }

    #endregion

    #region Abilities
    void Abilities_FixedUpdate()
    {
        if (basicAttackTimer > 0)
        {
            basicAttackTimer--;
            if (basicAttackTimer == 14)
            {
                if (basicAttackComboWindow > 0)
                {
                    basicAttack.Activate(9, IsFacingRight() ? 0 : 1);
                }
                else
                {
                    basicAttack.Deactivate();
                    basicAttackAnimator.Stop();
                    basicAttack1.Activate(9, IsFacingRight() ? 0 : 1);
                }
            }
            if (basicAttackTimer == 0)
            {
                wakizashi.SetActive(true);
                LockFacing(false);
            }
        }
        if (basicAttackCD > 0) { basicAttackCD--; }

        if (basicAttackComboWindow > 0)
        {
            basicAttackComboWindow--;
        }

        if (blinkTimer > 0)
        {
            SetVelocity(blinkDI.x, blinkDI.y);
            blinkTimer--;
            if (blinkTimer == 0)
            {
                SetIntangible(false);
                DisableSprites(false);
                EnableGravity();
                Instantiate(dashPoofFX, trfm.position, Quaternion.identity);

                SetVelocity(blinkExitVelocity.x, blinkExitVelocity.y);
                if (!IsTouchingGround() && !InputManager.DownHeld())
                {
                    AddYVelocity(jumpPower * 0.5f, jumpPower * 0.5f);
                }
            }            
        }
        if (blinkCD > 0) { blinkCD--; }

        if (evadeTimer > 0)
        {
            ApplyDirectionalFriction(groundFriction);
            evadeTimer--;
            if (evadeTimer == 0)
            {
                self.evadeObj.SetActive(false);
                SetIntangible(false);
                SetStun(false);
                Instantiate(self.dashPoofFX, self.trfm.position, Quaternion.identity);
            }
        }
        if (evadeCD > 0) { evadeCD--; }
        if (evading > 0)
        {
            evading--;
            if (evading == 3)
            {
                SetStun(false);
                self.trfm.position = evadedEnemy.trfm.position + Vector3.right * GetEvadeOffsetAndSetFacing();                
            }
            if (evading == 0)
            {
                DisableSprites(false);
                EnableGravity();
                SetIntangible(false);                
                Instantiate(self.dashPoofFX, self.trfm.position, Quaternion.identity);                
            }
        }

        if (eviscerateTimer > 0)
        {            
            eviscerateTimer--;

            if (eviscerateTimer == 45) {
                eviscerateAnimator.Play();
                wakizashi.SetActive(false);
            }
            if (eviscerateTimer > 30)
            {
                SetVelocity(0, 0);
                if (eviscerateTimer == 31)
                {
                    SetIntangible(true);
                    evisceratePtcl.Play();
                }
            }
            else if (eviscerateTimer > 28)
            {
                SetVelocity((IsFacingLeft() ? -eviscerateSpeed : eviscerateSpeed), 0);
            }
            else if (eviscerateTimer > 0)
            {                
                if (eviscerateTimer == 28)
                {
                    SetVelocity(IsFacingLeft() ? -eviscerateExitVelocity : eviscerateExitVelocity, 0);                                      
                    if (empoweredEviscerate)
                    {
                        empoweredEviscerateAttack.Activate(3, IsFacingRight() ? 0 : 1);
                        Time.timeScale = 0.3f;
                    } else
                    {
                        eviscerateAttack.Activate(3, IsFacingRight() ? 0 : 1);
                    }

                    EnableGravity();                                     
                }
                if (eviscerateTimer == 27) {
                    SetIntangible(false);
                    evisceratePtcl.Stop();
                }
                if (empoweredEviscerate && eviscerateTimer < 16)
                {
                    if (Time.timeScale < 1)
                    {
                        Time.timeScale += 0.1f;
                        if (Time.timeScale > 1)
                        {
                            Time.timeScale = 1;
                            empoweredEviscerate = false;
                        }
                    }                    
                }
                ApplyDirectionalFriction(eviscerateDrag);
                if (eviscerateTimer == 10)
                {
                    wakizashi.SetActive(true);
                }
            }

            if (eviscerateTimer == 0)
            {
                EnableGravity();
                LockFacing(false);
            }
        }

        if (eviscerateCD > 0) { eviscerateCD--; }
    }

    void HandleAttack()
    {
        if (InputManager.AttackPressed() && basicAttackCD < 1 && !IsStunned())
        {
            if (basicAttackComboWindow > 0)
            {
                basicAttackAnimator.Stop();
                basicAttack.Deactivate();

                wakizashi.SetActive(false);                
                basicAttack1Animator.Play();
                basicAttackComboWindow = 0;
                basicAttackCD = 30;
                if (basicAttackTimer < 1)
                {
                    LockFacing(true);
                }
            }
            else
            {
                wakizashi.SetActive(false);
                basicAttackAnimator.Play();
                basicAttackCD = 9;
                basicAttackComboWindow = 30;
                LockFacing(true);
            }
            basicAttackTimer = 18;

            if (InputManager.LeftHeld())
            {
                if (!InputManager.RightHeld())
                {
                    FaceLeft();
                    AddXVelocity(IsTouchingGround() ? -attackStepPower : -aerialAttackStepPower, -attackStepPower);
                }
            }
            else if (InputManager.RightHeld())
            {
                FaceRight();
                AddXVelocity(IsTouchingGround() ? attackStepPower : aerialAttackStepPower, attackStepPower);
            }
            else if (IsTouchingGround())
            {
                AddXVelocity((IsFacingLeft() ? -attackStepPower : attackStepPower) * .66f, IsFacingLeft() ? -attackStepPower : attackStepPower);
            }
            else
            {
                AddXVelocity((IsFacingLeft() ? -aerialAttackStepPower : aerialAttackStepPower) * .66f, IsFacingLeft() ? -aerialAttackStepPower : aerialAttackStepPower);
            }
        }
    }

    void HandleJump()
    {
        if (InputManager.JumpPressed())
        {
            if (IsTouchingGround())
            {
                SetYVelocity(jumpPower);
            }
            else if (backTouchingTerrain())
            {
                if (MovingInFacingDirection())
                {
                    DoWallJump();
                }
            }
            else if (hasDJump)
            {
                wallJumpWindow = 4;
                SetYVelocity(jumpPower);
                hasDJump = false;
                hadDJump = true;
            }
            else
            {
                wallJumpWindow = 4;
                hadDJump = false;
            }
        }
    }

    void CancelAttacks()
    {
        basicAttack.Deactivate();
        basicAttackAnimator.Stop();
        basicAttack1.Deactivate();
        basicAttack1Animator.Stop();
    }

    void HandleBlink()
    {
        if (InputManager.BlinkPressed() && blinkCD < 1 && !IsStunned())
        {
            CancelAttacks();

            blinkExitVelocity = rb.velocity;
            DisableGravity();
            GetBlinkDI();
            SetVelocity(blinkDI.x, blinkDI.y);

            Instantiate(dashPoofFX, trfm.position, Quaternion.identity);
            SetIntangible(true);
            DisableSprites(true);
            blinkTimer = blinkDuration;
            SetTrackingDelay(blinkDuration);
            Stun(blinkDuration - 3);
            blinkCD = 60;
        }        
    }

    Vector2 blinkDI;
    void GetBlinkDI()
    {
        vect2 = InputManager.GetVectorInput();
        blinkDI = vect2 * blinkPower;
        if (Mathf.Abs(vect2.x) < 0.001f && Mathf.Abs(vect2.y) < 0.001f)
        {
            blinkDI.x = IsFacingLeft() ? -blinkPower : blinkPower;
        }
        blinkDI.y *= 0.5f;
        SetVelocity(blinkDI.x, blinkDI.y);
    }

    void HandleEvade()
    {
        if (InputManager.ShadowPressed() && evadeCD < 1 && !IsStunned())
        {
            CancelAttacks();
            evadeObj.SetActive(true);
            SetIntangible(true);

            SetStun(true);
            evadeTimer = 15;
            evadeCD = 60;
        }
    }

    Enemy evadedEnemy;
    Vector3 evadeOrigin;
    public static void Evade(Enemy enemy)
    {
        self.evadeOrigin = self.trfm.position;
        self.SetTrackingDelay(30);

        Instantiate(self.dashPoofFX, self.trfm.position, Quaternion.identity);
        self.evadeObj.SetActive(false);
        evadeTimer = 0;
        self.DisableSprites(true);
        self.DisableGravity();

        self.evadedEnemy = enemy;
        self.GetEvadeOffsetAndSetFacing();

        evading = 15;
        self.AddFocus(10);        
    }

    float GetEvadeOffsetAndSetFacing()
    {
        float offset = (evadedEnemy.trfm.position - evadeOrigin).x;
        if (offset > 0)
        {
            FaceLeft();
            RaycastHit2D hit = Physics2D.Linecast(evadedEnemy.trfm.position, evadedEnemy.trfm.position + Vector3.right * 2, Tools.terrainLayerMask);
            if (hit.collider != null)
            {
                return hit.distance;
            }
            return 2;
        }
        else
        {            
            FaceRight();
            RaycastHit2D hit = Physics2D.Linecast(evadedEnemy.trfm.position, evadedEnemy.trfm.position + Vector3.right * -2, Tools.terrainLayerMask);
            if (hit.collider != null)
            {
                return -hit.distance;
            }
            return -2;
        }
    }

    bool empoweredEviscerate;
    int windUpTime = 25;
    void HandleEviscerate()
    {
        if (InputManager.EvisceratePressed() && eviscerateCD < 1 && !IsStunned())
        {
            CancelAttacks();               

            Stun(30 + windUpTime);
            LockFacing(true);
            DisableGravity();
            eviscerateTimer = 30 + windUpTime;
            eviscerateCD = 90;

            if (focus == maxFocus)
            {
                empoweredEviscerate = true;
                focus = 0;
                self.AddFocus(-999);
            }
        }
    }

    int spritesDisabled;
    void DisableSprites(bool disable)
    {
        if (disable)
        {
            spritesDisabled++;
            if (spritesDisabled == 1)
            {
                foreach (SpriteRenderer sr in sprites)
                {
                    sr.enabled = false;
                }
            }
        }
        else
        {
            spritesDisabled--;
            if (spritesDisabled == 0)
            {
                foreach (SpriteRenderer sr in sprites)
                {
                    sr.enabled = true;
                }
            }
        }
    }
    #endregion

    public void AddFocus(int amount)
    {
        bool focusMaxed = self.focus >= self.maxFocus;
        self.focus += amount;
        if (self.focus >= self.maxFocus) {
            self.focus = self.maxFocus;
            if (!focusMaxed)
            {
                Debug.Log("focus maxed");
                focusFader.SetTo(0.4f);
                focusFader.FadeTo(0);
            }
        }
        if (self.focus < 0) { self.focus = 0; }
        self.focusBar.SetHP(self.focus);
    }

    public override int TakeDamage(int damage, int p_entityID, Team p_team, int attackID)
    {
        Debug.Log("damage taken");
        damageFader.SetTo(Mathf.Min(damage / 50f, 1f));
        damageFader.FadeTo(0);
        return base.TakeDamage(damage, p_entityID, p_team, attackID);
    }

    protected override void Die()
    {
        Time.timeScale = 0.3f;
        SetStun(true);
        DisableSprites(true);
        SetIntangible(true);
    }

    #region POSITION_TRACKING

    [SerializeField] Vector3[] trackedPositions;
    int trackingTimer, trackingIndex, trackingDelay;

    void SetTrackingDelay(int delay)
    {
        trackingDelay = Mathf.Max(delay, trackingDelay);
    }

    Vector3 predictedTerrainCollision;
    void PositionTracking_FixedUpdate()
    {
        if (trackingDelay > 0) {  trackingDelay--; }
        if (trackingTimer > 0) { trackingTimer--; }
        else
        {
            if (trackingDelay > 0) { trackedPositions[trackingIndex] = trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length];                  }
            else { trackedPositions[trackingIndex] = trfm.position; }                
            trackingIndex++;
            trackingIndex %= trackedPositions.Length;
            trackingTimer = 1;

            predictedTerrainCollision = ExtrapolateTerrainCollision(GetRawPredictedPosition(300));
        }
    }
    Vector3 ExtrapolateTerrainCollision(Vector3 predictedPos)
    {
        RaycastHit2D hit = Physics2D.Linecast(trfm.position, predictedPos, Tools.terrainLayerMask);        
        if (hit.collider != null)
        {
            vect3.x = hit.point.x; vect3.y = hit.point.y; vect3.z = 0;
            return vect3;
        }
        return predictedPos;
    }

    Vector3 targetPredictedPos;
    public Vector3 GetRawPredictedPosition(int ticksAhead)
    {
        return trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length] + (trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length] - trackedPositions[trackingIndex]) / 60f * ticksAhead;
    }
    public Vector3 GetPredictedPosition(int ticksAhead)
    {
        targetPredictedPos = GetRawPredictedPosition(ticksAhead);
        if (Vector3.SqrMagnitude(targetPredictedPos - trfm.position) > Vector3.SqrMagnitude(predictedTerrainCollision - trfm.position))
        {
            return predictedTerrainCollision;
        }
        return targetPredictedPos;
    }
    public Vector3 GetDistanceScalingPredictedPosition(Vector3 pos, float multiplier, int tickPredictionCap = 90)
    {
        targetPredictedPos = trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length] + (trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length] - trackedPositions[trackingIndex])
            / 60f * Mathf.Min(Vector3.Distance(pos, trackedPositions[(trackingIndex + trackedPositions.Length - 1) % trackedPositions.Length]) * multiplier,
            tickPredictionCap);
        if (Vector3.SqrMagnitude(targetPredictedPos - trfm.position) > Vector3.SqrMagnitude(predictedTerrainCollision - trfm.position))
        {
            return predictedTerrainCollision;
        }
        return targetPredictedPos;
    }
    #endregion
}
