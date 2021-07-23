using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKControl : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float targetSpeed=4;
    Animator animator;
    float weight = 1;
     // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex) {
        animator.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        animator.SetLookAtPosition(target.position);
        animator.SetLookAtWeight(weight);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 move = Vector2.zero;
        move.x = Input.GetAxis("Horizontal");
        move.y = Input.GetAxis("Vertical");
        target.Translate(move*targetSpeed*Time.deltaTime);
    }
}
