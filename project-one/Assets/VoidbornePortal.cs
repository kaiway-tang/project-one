using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidbornePortal : MonoBehaviour
{
    float time;
    [SerializeField] Transform portalTrfm;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        portalTrfm.localScale = Vector3.one * Mathf.Sin(2 * 3.14f * time) * 1f;
        if (time >= 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
