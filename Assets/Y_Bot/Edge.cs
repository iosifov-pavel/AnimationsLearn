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
            Debug.Log("Hand");
            player = other.gameObject.GetComponentInParent<Control>();
            player.setHanging(true, anchor);
        }
    }
}
