﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaLimit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") {
            other.GetComponent<PlayerController>().Respawning();
        }
    }
}
