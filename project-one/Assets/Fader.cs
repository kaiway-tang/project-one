using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] spriteRenderer;
    [SerializeField] float fadeRate;

    Color col;
    // Start is called before the first frame update
    void Start()
    {
        col = Color.white;
    }

    [SerializeField]
    void Update()
    {
        if (fading)
        {
            for (int i = 0; i < spriteRenderer.Length; i++)
            {

                col = spriteRenderer[i].color;
                col.a = Mathf.MoveTowards(col.a, targetAlpha, fadeRate * Time.deltaTime);
                spriteRenderer[i].color = col;

                if (Mathf.Abs(col.a - targetAlpha) < fadeRate * Time.deltaTime)
                {
                    col.a = targetAlpha;
                    spriteRenderer[i].color = col;
                    fading = false;
                }
            }
        }
    }

    bool fading;
    float targetAlpha;
    public void FadeTo(float alpha)
    {
        fading = true;
        targetAlpha = alpha;
    }

    public void SetTo(float alpha)
    {
        fading = false;
        for (int i = 0; i < spriteRenderer.Length; i++)
        {
            col = spriteRenderer[i].color;
            col.a = alpha;
            spriteRenderer[i].color = col;
        }
    }
}
