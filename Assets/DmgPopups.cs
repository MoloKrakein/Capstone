using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DmgPopups : MonoBehaviour
{
    public float TimeToLive = 2f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, TimeToLive);
    }
    }

    // Update is called once per frame
