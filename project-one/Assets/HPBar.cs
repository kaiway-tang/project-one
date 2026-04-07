using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HPBar : MonoBehaviour
{
    [SerializeField] Transform hpBar, hpHighlight;
    [SerializeField] Fader fader;
    [SerializeField] bool doNotFade;
    [SerializeField] float fadeDelay;
    void Start()
    {
        if (!doNotFade) { fader.SetTo(0); }
    }

    float currentVelocity;
    float fadeTimer;
    void Update()
    {
        if (animatingHighlight)
        {
            if (animationDelay > 0)
            {
                animationDelay -= Time.deltaTime;
            }
            else if (hpHighlight.localScale.x > hpBar.localScale.x + 0.01f)
            {
                SetBarPercentage(hpHighlight, Mathf.SmoothDamp(hpHighlight.localScale.x, hpBar.localScale.x, ref currentVelocity, 0.2f));
                if (hpHighlight.localScale.x < hpBar.localScale.x + 0.01f)
                {
                    SetBarPercentage(hpHighlight, hpBar.localScale.x);
                    animatingHighlight = false;
                    if (!doNotFade) { fadeTimer = fadeDelay; }                    
                }                    
            }
        }

        if (fadeTimer > 0)
        {
            fadeTimer -= Time.deltaTime;
            if (fadeTimer <= 0)
            {
                fader.FadeTo(0);
            }
        }
    }

    [SerializeField] int maxHP = 1;
    public void Initiate(int pMaxHP = 1, int startingHP = -1)
    {
        maxHP = pMaxHP;
        if (startingHP < 0) { startingHP = maxHP; }
        SetHP(startingHP);
        if (!doNotFade) { fader.SetTo(0); }
    }

    bool animatingHighlight;
    float animationDelay;

    int lastHP;
    public void SetHP(int hp)
    {
        if (hp > maxHP) { hp = maxHP; }
        if (hp < 0) { hp = 0; }
        if (hp < lastHP)
        {
            SetBarPercentage(hpBar, ((float)hp) / maxHP);
            animatingHighlight = true;
            animationDelay = 0.75f;
        }
        else
        {
            SetBarPercentage(hpBar, ((float)hp) / maxHP);
            SetBarPercentage(hpHighlight, ((float)hp) / maxHP);
            animatingHighlight = false;
        }
        lastHP = hp;

        if (!doNotFade) {
            fader.SetTo(1);
            fadeTimer = 99;
        }
    }

    Vector2 scale;
    void SetBarPercentage(Transform barTrfm, float percentage)
    {
        if (percentage > 1) { percentage = 1; }
        scale = barTrfm.localScale;
        scale.x = percentage;
        barTrfm.localScale = scale;
    }
}
