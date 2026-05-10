using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] GameObject[] objects;
    public void Trigger()
    {
        int selected = Random.Range(0, objects.Length);
        for (int i = 0; i < objects.Length; i++)
        {
            if (i != selected)
            {
                Destroy(objects[i]);
            }
        }
    }
}
