using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;

public class ClickMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Animator animator;

    public GameObject plug;
    public GameObject clickPoint;
    [SerializeField]
    private bool isCarryingPlug = false;    // 캐릭터가 plug를 잡고 있는지 여부
    public Transform carryPosition;         // plug가 종속될 위치 (캐릭터 앞)
    public float pickUpDistance = 2.0f;     // plug를 잡을 수 있는 최대 거리

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        HandleMovement();
        HandleCarryingPlug();
    }

    private void HandleMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                clickPoint.SetActive(false);
                clickPoint.transform.position = hit.point;
                clickPoint.SetActive(true);

                agent.SetDestination(hit.point);

                // 에이전트가 이동 중인지 확인하여 애니메이터 상태 변경
                animator.SetBool("IsMoving", agent.remainingDistance > 0.1f);
            }
        }
    }

    private void HandleCarryingPlug()
    {
        if (isCarryingPlug)
        {
            // 캐릭터가 Plug를 운반 중일 때, Plug를 캐릭터와 함께 이동
            plug.transform.position = carryPosition.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Plug와의 충돌을 확인하고 클릭 시 Plug를 들 수 있도록 함
        if (other.gameObject == plug && Vector3.Distance(transform.position, plug.transform.position) <= pickUpDistance)
        {
            if (Input.GetMouseButtonDown(0))
            {
                PickUpPlug();
            }
        }
    }

    private void PickUpPlug()
    {
        isCarryingPlug = true;
        plug.transform.SetParent(carryPosition);
        plug.transform.localPosition = Vector3.zero;
        plug.transform.localRotation = Quaternion.identity;
    }

}
