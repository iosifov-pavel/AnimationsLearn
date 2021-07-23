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
    [SerializeField] float climbingSpeed = 3;
    Animator animator;
    bool sneakMode = false;
    bool isSitting = false;
    bool blockAll = false;
    bool blockMove = false;
    bool blockJump = false;
    bool blockRotation = false;
    private bool canSit;
    bool isJumping = false;
    public bool isHanging=false;

    public bool jumpOff=false;
    Chair chair = null;
    CapsuleCollider colider;
    Rigidbody body;
    float count = 0.05f;

    void Start(){
        animator = GetComponent<Animator>();
        colider = GetComponent<CapsuleCollider>();
        body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update(){
        if (Input.GetMouseButtonDown(0)){
            if(canSit){
                ProceedSitting();
            }
        }
        if(blockAll){
            if(Input.GetMouseButtonDown(1)){
                Standing();
            }
        }
        if(!blockAll){
            if(isHanging) Hanging();
            if(!blockJump)Jumping();
            Sneaking();
            if(!blockMove)Walking();
        }
    }

    private void Hanging()
    {
        float move = Input.GetAxis("Horizontal");
        if(move==0){
            animator.SetBool("isShiming", false);
            return;
        }
        animator.SetBool("isShiming", true);
        animator.SetFloat("shimingDirection", move>0? 1 : -1);
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
        blockAll = false;
        body.useGravity = true;
        isHanging = false;
    }

    private void ProceedSitting()
    {
        animator.SetBool("isWalking", false);
        blockAll = true;
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
        if(!blockRotation)transform.Rotate(0, rotation, 0);
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
        if(isHanging && Input.GetKeyDown(KeyCode.W)){
            animator.SetTrigger("climbing");
            StartCoroutine(ClimbUp("StandUpFromCrouch"));
        }
        else if(isHanging && Input.GetKeyDown(KeyCode.S)){
            animator.SetTrigger("jumpOff");
            jumpOff = true;
            StartCoroutine(ClimbUp("jumpdown"));
        }
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

    IEnumerator ClimbUp(string name){
        yield return new WaitUntil(()=>
            animator.GetCurrentAnimatorStateInfo(0).IsName(name)
        );
        animator.ResetTrigger("climbing");
        animator.ResetTrigger("jumpOff");
        isHanging = false;
        animator.SetBool("isHanging",isHanging);
        blockMove = false;
        blockRotation = false;
        body.isKinematic = false;
        yield return new WaitUntil(()=>
            animator.GetCurrentAnimatorStateInfo(0).IsName("neutral_idle")
        );
        jumpOff = false;
        yield break;
    }

    public void setHanging(bool state, Transform target){
        blockRotation = state;
        blockMove = state;
        body.isKinematic = state;
        animator.SetBool("isHanging",state);
        if(state){
            count = 0f;
            StartCoroutine(LerpTo(CalculateOffset(target)));
        }
    }

    Vector3 CalculateOffset(Transform target){
        transform.forward = target.forward;
        Vector3 player = transform.position;
        player.y = target.position.y;
        Vector3 targetPos = target.position;
        Vector3 fromTargetToPlayer = player - targetPos;
        float amount = fromTargetToPlayer.magnitude;
        fromTargetToPlayer.Normalize();
        float angle = Vector3.Angle(target.right,fromTargetToPlayer);
        float sinAngle = Mathf.Sin(angle * Mathf.Deg2Rad);
        float magnB = amount * sinAngle;
        player += transform.forward*magnB;
        return player;
    }

    IEnumerator LerpTo(Vector3 target){
        while(true){
            transform.position = Vector3.Lerp(transform.position,target-lerpOffset,count*Time.deltaTime);
            count+=0.16f;
            if(Vector3.Distance(transform.position,target-lerpOffset)<0.02f){
                Debug.Log("Done");
                isHanging = true;
                yield break;
            } 
            yield return null;
        }
    }
}
