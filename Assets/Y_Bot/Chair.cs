using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Chair : MonoBehaviour
{
    [SerializeField] Transform placeToSit;
    [SerializeField] CinemachineVirtualCamera cameraToSit;
    bool playerIsNear = false;
    Control player = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag=="Player"){
            playerIsNear = true;
            player = other.gameObject.GetComponent<Control>();
            player.NearToObject(this,true);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player")
        {
            playerIsNear = false;
            player.NearToObject(this,false);
        }
    }

    public Transform GetPosition(){
        return placeToSit;
    }

    public CinemachineVirtualCamera GetCamera(){
        return cameraToSit;
    }

}
