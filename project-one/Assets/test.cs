using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] int tickSeconds, ticks;

    [SerializeField] Fader focusFader;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("set fade");
            focusFader.SetTo(0.3f);
            focusFader.FadeTo(0);
        }
    }

    private void FixedUpdate()
    {
        transform.position = Player.self.GetPredictedPosition(30);
    }
}
