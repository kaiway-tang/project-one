using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAnimator : MonoBehaviour
{
    [SerializeField] Sprite[] sprites;    
    [SerializeField] int[] setSpriteOrderFrame; //if currentSprite is this index, set the sorting order to the corresponding index in orderToSetSprite
    [SerializeField] int[] orderToSetSprite;
    int setSpriteOrderInLayerIndex;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] int ticksPerFrame;
    [SerializeField] bool playOnStart, loop, dontClearSprite;
    bool playing;
    int timer, currentSprite;
    // Start is called before the first frame update
    void Start()
    {
        if (playOnStart) { Play(); }
        else { currentSprite = 999; }
    }

    private void FixedUpdate()
    {
        if (currentSprite < sprites.Length && playing)
        {
            if (timer > 0)
            {
                timer--;
                if (timer <= 0)
                {
                    if (setSpriteOrderFrame.Length > 0 && setSpriteOrderInLayerIndex < setSpriteOrderFrame.Length && currentSprite == setSpriteOrderFrame[setSpriteOrderInLayerIndex])
                    {
                        spriteRenderer.sortingOrder = orderToSetSprite[setSpriteOrderInLayerIndex];
                        setSpriteOrderInLayerIndex++;
                    }

                    timer = ticksPerFrame;
                    currentSprite++;
                    if (currentSprite == sprites.Length)
                    {
                        if (loop)
                        {
                            currentSprite = 0;
                            spriteRenderer.sprite = sprites[0];
                        }
                        else if (!dontClearSprite)
                        { spriteRenderer.sprite = null; }
                    }
                    else
                    {
                        spriteRenderer.sprite = sprites[currentSprite];
                    }
                }
            }
        }
    }

    public void Play()
    {
        currentSprite = 0;
        setSpriteOrderInLayerIndex = 0;
        spriteRenderer.sprite = sprites[0];
        timer = ticksPerFrame;

        playing = true;
    }

    public void Stop(bool setToSprite0 = false)
    {
        if (setToSprite0) { spriteRenderer.sprite = sprites[0]; }
        else if (!dontClearSprite) { spriteRenderer.sprite = null; }
        playing = false;
    }
}
