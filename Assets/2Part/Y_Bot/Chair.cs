using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Chair : MonoBehaviour
{
    [SerializeField] Transform placeToSit;
    [SerializeField] Transform placeToStand;
    [SerializeField] CinemachineVirtualCamera cameraToSit;
    [SerializeField] Material chair;
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



    private void OnMouseOver() {
        if (playerIsNear)
        {
            chair.EnableKeyword("_EMISSION");
        }
        else chair.DisableKeyword("_EMISSION");
    }

    private void OnMouseExit() {
        chair.DisableKeyword("_EMISSION");
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
            chair.DisableKeyword("_EMISSION");
        }
    }

    public Transform GetPosition(){
        return placeToSit;
    }

    public CinemachineVirtualCamera GetCamera(){
        return cameraToSit;
    }

    public void Emission(){
        chair.DisableKeyword("_EMISSION");
    }

    public Vector3 PlaceToStandUP(){
        return placeToStand.position;
    }

}
