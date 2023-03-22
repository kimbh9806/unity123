using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMovement : MonoBehaviour
{
    float hAxis;
    float vAxis;

    Vector3 moveVec;

    public float speed;
    public CharacterController characterController;
    public Vector3 movePoint;
    public Camera mainCamera;
    public Vector3 cameraOffset;
    public Animator animator;
    public GameObject[] weapons;
    public bool[] hasWeapons;


    private Vector3 unitPlanePosition;

    bool wDown; // ��ȣ�ۿ�

    bool jDown1; //���� ��ü
    bool jDown2;


    GameObject nearObject;


    void GetInput()
    {
        wDown = Input.GetButtonDown("Interation");
        jDown1 = Input.GetButtonDown("Swap1");
        jDown2 = Input.GetButtonDown("Swap2");
    }
    void Start()
    {
        speed = 4.0f;
        mainCamera = Camera.main;
        characterController = GetComponent<CharacterController>();

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");

        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * speed * Time.deltaTime;


        GetInput();
        Interation();
        Swap();

        if (Input.GetMouseButtonUp(1)) //���콺 ��Ŭ����
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition); //��Ŭ���ÿ�
            Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red, 1f); //������ ������ ǥ��


            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                movePoint = raycastHit.point; //���콺 ��Ŭ�� �� ��ǥ�� �������� ������ �̵�
                movePoint.y = 0;
            }
        }

        unitPlanePosition.x = transform.position.x;
        unitPlanePosition.z = transform.position.z;

        if (Vector3.Distance(transform.position, movePoint) > 0.5f)
        {
            Rotation();
            Move();
        }
        else
        {
            Stop();
        }

        mainCamera.transform.position = transform.position + cameraOffset;

    }

    void Move()
    {
        Vector3 thisUpdatePoint = (movePoint - transform.position).normalized * speed;
        characterController.SimpleMove(thisUpdatePoint);

        animator.SetBool("isMove", true);
    }

    void Stop()
    {
        animator.SetBool("isMove", false);
    }

    void Rotation()
    {
        Vector3 relativePosition = movePoint - transform.position;
        relativePosition.y = 0;
        Quaternion rotation = Quaternion.LookRotation(relativePosition, Vector3.up);
        transform.rotation = rotation;
    }

    void OnTriggerStay(Collider other) //���� ȹ��
    {
        if (other.tag == "Weapon")
            nearObject = other.gameObject;

        Debug.Log(nearObject.name);
    }

    void OnTriggerExit(Collider other) //���� ����
    {
        if (other.tag == "Weapon")
            nearObject = null;
    }

    void Swap() //���� ��ü
    {
        int weaponIndex = -1;
        if (jDown1) weaponIndex = 0;
        if (jDown2) weaponIndex = 1;
        if (jDown1 || jDown2)
        {
            weapons[weaponIndex].SetActive(true);
        }
    }
    void Interation() //���� ��ȣ�ۿ�
    {
        if (wDown && nearObject != null)
        {
            if (nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }
}