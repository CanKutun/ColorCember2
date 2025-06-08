using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CemberDonus : MonoBehaviour
{


    public float donusHizi;
    void Start()
    {
        
    }

  
    void Update()
    {
        transform.Rotate(0,0,donusHizi *  Time.deltaTime);
    }
}
