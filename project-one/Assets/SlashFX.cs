using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashFX : MonoBehaviour
{
    [SerializeField] float vanishRate, stretchRate;
    [SerializeField] Transform trfm;
    Vector2 scale;
     void Awake()
    {
        scale = trfm.localScale;
        trfm.Rotate(Vector3.forward * Random.Range(0, 360));
    }
    void Start()
    {
        
    }

    void Update()
    {
        scale.x -= vanishRate * Time.deltaTime * 60;
        scale.y += scale.y * stretchRate * Time.deltaTime * 60;
        trfm.localScale = scale;

        if (scale.x < 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
