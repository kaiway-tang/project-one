using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    [SerializeField] BaseAnimator[] animators;
    [SerializeField] int[] priorities;

    void PlayAnimation(int animationID)
    {
        //TODO: priority system & que
        animators[animationID].Play();
    }

    private void FixedUpdate()
    {
        
    }
}
