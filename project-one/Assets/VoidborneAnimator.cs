using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidborneAnimator : AnimatorController
{
    public ReferenceState
         Idle = new ReferenceState(0, 1),
         Run = new ReferenceState(1, 1),
         Attack1 = new ReferenceState(2, 50),
         SpecialAttack = new ReferenceState(3, 50);
    // Start is called before the first frame update
    protected new void Start()
    {
        currentState = new ActiveState(Idle);
        defaultState = new ActiveState(Idle);

        animationQue = new ActiveState[] { new ActiveState(), new ActiveState(), new ActiveState() };
    }

    new void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
