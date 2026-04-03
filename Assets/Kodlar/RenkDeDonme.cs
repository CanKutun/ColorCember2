using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenkDeDonme : MonoBehaviour
{

    public float donusHizi;

    void Update()
    {
        transform.Rotate(0, 0, donusHizi * Time.deltaTime);
    }
}
