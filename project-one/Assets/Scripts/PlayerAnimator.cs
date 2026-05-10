using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : AnimatorController
{
    public ReferenceState
        Idle = new ReferenceState(0, 1),
        Run = new ReferenceState(1, 1),
        Rising = new ReferenceState(2, 1),
        Falling = new ReferenceState(3, 1),
        AirborneStill = new ReferenceState(4, 1),
        Attack1 = new ReferenceState(5, 50),
        Attack2 = new ReferenceState(6, 50),
        Eviscerate = new ReferenceState(7, 50);

    // Start is called before the first frame update
    new void Start()
    {
        currentState = new ActiveState(Idle);
        defaultState = new ActiveState(Idle);

        animationQue = new ActiveState[] { new ActiveState(), new ActiveState(), new ActiveState(), new ActiveState() };
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
