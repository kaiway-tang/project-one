using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] float time;
    [SerializeField] int tickSeconds, ticks;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }

    private void FixedUpdate()
    {
        transform.position = Player.self.GetPredictedPosition(30);
    }
}
