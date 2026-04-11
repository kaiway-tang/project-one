using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ChunkInfo : MonoBehaviour
{
    public bool up, down, left, right;
    public int directionID;
    public Tilemap tileMap;
    public bool disabled;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    public void SetDirectionID()
    {
        directionID = (up ? 1 : 0) + (down ? 2 : 0) + (left ? 4 : 0) + (right ? 8 : 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
