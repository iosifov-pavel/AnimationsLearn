using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Control : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float sneakMultiplier = 0.5f;
    [SerializeField] float rotationSpeed = 100f;
    Animator animator;
    bool sneakMode = false;
    bool isSitting = false;
    bool blockControl = false;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (BusyProceedMouseAction()) return;
            
        }
        if(blockControl){
            if(Input.GetMouseButtonDown(1)){
                blockControl = false;
            }
        }
        if(!blockControl){
            Jumping();
            Sneaking();
            Walking();
        }
    }

    private void Walking()
    {
        float move = Input.GetAxis("Vertical") * speed * Time.deltaTime;
        if (sneakMode) move *= sneakMultiplier;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Translate(0, 0, move);
        transform.Rotate(0, rotation, 0);
        if (move != 0) animator.SetBool("isWalking", true);
        else animator.SetBool("isWalking", false);
        animator.SetFloat("direction", move > 0 ? 1 : -1);
    }

    private void Sneaking()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            sneakMode = !sneakMode;
            animator.SetBool("isSneaking", sneakMode);
        }
    }

    private void Jumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("isJumping");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.ResetTrigger("isJumping");
        }
    }

    private bool BusyProceedMouseAction()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition),out hit,100f)){
            if(hit.collider.gameObject.tag=="chair"){
                transform.LookAt(hit.transform);
                SitOnChair(hit.collider.ClosestPointOnBounds(transform.position));
                blockControl = true;
            }
        }
        return false;
    }

    private void SitOnChair(Vector3 chair)
    {
        Debug.Log(Vector3.Distance(transform.position,chair));
        if(Vector3.Distance(transform.position,chair)>0.6f){
            print("farAway");
        }
        else{
            blockControl = true;
            print("chair");
        } 
    }
}
