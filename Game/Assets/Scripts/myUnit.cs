using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myUnit : MonoBehaviour
{
    public float speed;
    public CharacterController characterController;//p
    public Vector3 movePoint;
    public Camera mainCamera;
    public Vector3 cameraOffset;
    public Animator animator;
    private Vector3 unitPlanePosition;
    public GameObject target;
    public StateType stateType;
    public enum StateType { none, move, attack }
    public float attckRange = 2f;

    public enum AttackStateType { ready, swing, cooltime }
    public AttackStateType attackStateType;
    WaitForSeconds attackCooltimeWaitForSeconds;
    public Coroutine attackCoroutine;

    void Start()
    {
        speed = 4.0f;
        mainCamera = Camera.main;
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        setAttackCooltimeWaitForSecondes(1.5f);
        attackStateType = AttackStateType.ready;
    }

    public void setAttackCooltimeWaitForSecondes(float attackCooltime)
    {
        attackCooltimeWaitForSeconds = new WaitForSeconds(attackCooltime);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            animator.SetBool("Kick", true);
        }
        else
        {
            animator.SetBool("Kick", false);
        }

        if (Input.GetKey(KeyCode.W))
        {
            animator.SetBool("Punch", true);
        }
        else
        {
            animator.SetBool("Punch", false);
        }

        if (Input.GetKey(KeyCode.E))
        {
            animator.SetBool("Hook", true);
        }
        else
        {
            animator.SetBool("Hook", false);
        }


        if (Input.GetMouseButtonUp(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.collider.CompareTag("Enemy"))
                {
                    target = raycastHit.transform.gameObject;
                    stateType = StateType.attack;
                }
                else
                {
                    movePoint = raycastHit.point;
                    movePoint.y = 0;
                    stateType = StateType.move;
                }

                CancelSwing();
            }

        }

        switch (stateType)
        {
            case StateType.move:
                MoveCommand();
                break;
            case StateType.attack:
                AttackCommand();
                break;
        }

        mainCamera.transform.position = transform.position + cameraOffset;
    }

    void MoveCommand()
    {
        unitPlanePosition.x = transform.position.x;
        unitPlanePosition.z = transform.position.z;
        if (Vector3.Distance(unitPlanePosition, movePoint) > 0.3f)
        {
            Move();
        }
        else
        {
            Stop();
        }
    }

    void Move()
    {
        animator.SetBool("Run", true);
        Rotation(movePoint);

        Vector3 thisUpdatePoint = (movePoint - transform.position).normalized * speed;

        characterController.SimpleMove(thisUpdatePoint);

    }

    void Stop()
    {
        animator.SetBool("Run", false);
    }


    void Rotation(Vector3 targetPoint)
    {
        Vector3 relativePosition = targetPoint - transform.position;
        relativePosition.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = rotation;
    }


    void AttackCommand()
    {
        if (target == null) return;
        movePoint = target.transform.position;
        float distance = Vector3.Distance(transform.position, movePoint);

        if (distance < attckRange)
            Swing();
        else
            Move();

    }


    void Swing()
    {
        Rotation(movePoint);
        Stop();
        if (attackStateType == AttackStateType.ready)
        {
            attackCoroutine = StartCoroutine(SwingIEnumerator());
        }
    }

    void CancelSwing()
    {
        if (attackStateType == AttackStateType.swing)
        {
            if (stateType == StateType.move)
                animator.SetTrigger("cancel");

            else
                animator.ResetTrigger("swing");


            StopCoroutine(attackCoroutine);

            attackStateType = AttackStateType.ready;
        }

    }


    public IEnumerator SwingIEnumerator()
    {
        attackStateType = AttackStateType.swing;
        animator.SetTrigger("swing");
        yield return attackCooltimeWaitForSeconds;
        attackStateType = AttackStateType.ready;
    }
}