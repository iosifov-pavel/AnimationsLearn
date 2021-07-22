using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Control : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float sneakMultiplier = 0.5f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] Vector3 lerpOffset = Vector3.zero;
    Animator animator;
    bool sneakMode = false;
    bool isSitting = false;
    bool blockControl = false;
    private bool canSit;
    bool isJumping = false;
    bool isHanging=false;
    Chair chair = null;
    CapsuleCollider colider;
    Rigidbody body;
    float count = 0.05f;

    void Start()
    {
        animator = GetComponent<Animator>();
        colider = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(canSit){
                ProceedSitting();
            }
            
        }
        if(blockControl){
            if(Input.GetMouseButtonDown(1))
            {
                Standing();
            }
        }
        if(!blockControl){
            Jumping();
            Sneaking();
            Walking();
        }
    }

    private void Standing()
    {
        StopAllCoroutines();
        CinemachineVirtualCamera cameraToSit = chair.GetCamera();
        cameraToSit.Priority = 1;
        animator.SetBool("isSitting", false);
        count = 0.05f;
        StartCoroutine(WaitForStand());
    }

    IEnumerator WaitForStand(){
        yield return new WaitUntil(()=>
        {
            transform.position = Vector3.Lerp(transform.position,chair.PlaceToStandUP(),count*Time.deltaTime);
            count+=0.01f;
            return animator.GetCurrentAnimatorStateInfo(0).IsName("neutral_idle");
        });
        colider.enabled = true;
        blockControl = false;
        body.useGravity = true;
    }

    private void ProceedSitting()
    {
        animator.SetBool("isWalking", false);
        blockControl = true;
        CinemachineVirtualCamera cameraToSit = chair.GetCamera();
        Transform placeToStand = chair.GetPosition();
        cameraToSit.Priority = 3;
        count = 0.05f;
        StartCoroutine(GoToPlace(placeToStand));
    }

    IEnumerator GoToPlace(Transform place)
    {
        animator.SetBool("isWalking", true);
        animator.SetFloat("direction",0.5f);
        Debug.Log("In");
        colider.enabled = false;
        body.useGravity = false;
        yield return new WaitUntil(() => Check(place));
        Debug.Log("Out");
        animator.SetBool("isWalking", false);
        animator.SetBool("isSitting", true);
        yield break;
    }

    private bool Check(Transform place)
    {
        transform.position = Vector3.Lerp(transform.position, place.position, count*Time.deltaTime);
        transform.forward = Vector3.Lerp(transform.forward, -place.forward, count* Time.deltaTime);
        count+=0.01f;
        bool posRady = Vector3.Distance(transform.position,place.position)<0.05f;
        bool reoReady = Mathf.Abs(Vector3.Angle(transform.forward,-place.forward))<1;
        return posRady && reoReady;
    }

    public void NearToObject(Chair chair, bool check)
    {
        this.chair = chair;
        if(!check){
            canSit = false;
            return;
        }
        canSit = true;
        Debug.Log("Can Sit");
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
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            //body.AddForce(Vector3.up*jumpForce,ForceMode.Impulse);
            isJumping = true;
            animator.SetTrigger("isJumping");
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.ResetTrigger("isJumping");
            isJumping = false;
        }
    }

    public void setHanging(bool state, Transform target){
        isHanging = state;
        body.isKinematic = isHanging;
        count = 0.05f;
        StartCoroutine(LerpTo(target));
        animator.SetBool("isHanging",isHanging);
    }

    IEnumerator LerpTo(Transform target){
        transform.forward = -target.up;
        while(true){
            Vector3 lerpTo = new Vector3(transform.position.x,target.position.y,transform.position.z);
            transform.position = Vector3.Lerp(transform.position,lerpTo-lerpOffset,count*Time.deltaTime);
            Debug.Log(Vector3.Distance(transform.position, lerpTo - lerpOffset));
            if(Vector3.Distance(transform.position,lerpTo-lerpOffset)<0.1f) yield break;
            count+=0.01f;
            yield return null;
        }
    }
}
