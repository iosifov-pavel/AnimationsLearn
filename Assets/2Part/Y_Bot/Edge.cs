using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    Control player;
    [SerializeField] Transform anchor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Hand"){
            player = other.gameObject.GetComponentInParent<Control>();
            if(player.jumpOff || player.isHanging) return;
            Debug.Log("Hand");
            player.setHanging(true, anchor);
        }
    }
}
