using UnityEngine;

public class BoatController : MonoBehaviour
{
    // === 인스펙터 설정 ===
    [Header("좌석 설정")]
    public Transform seatPosition;      // 플레이어가 앉을 조종석 위치
    public Transform exitPosition;      // 하차 시 플레이어가 이동할 위치

    [Header("배 조종 설정")]
    public float boatSpeed = 10f;       // 배 전진/후진 속도
    public float boatTurnSpeed = 50f;   // 배 회전 속도

    [Header("참조 (인스펙터에서 할당)")]
    public Transform playerTransform;       // 플레이어 Transform
    public PlayerMovement playerMovement;   // 플레이어 이동 스크립트
    public Rigidbody boatRigidbody;         // 배의 Rigidbody

    // === 내부 상태 ===
    private bool isPlayerBoarding = false;  // 플레이어 탑승 상태
    private CharacterController playerCharacterController; // 플레이어 CharacterController

    void Start()
    {
        if (playerTransform != null)
        {
            playerCharacterController = playerTransform.GetComponent<CharacterController>();
        }
    }

    void Update()
    {
        if (!isPlayerBoarding) return;

        // 탑승 중: 플레이어 위치를 seatPosition에 동기화
        if (playerCharacterController != null)
        {
            // CharacterController를 비활성화한 상태이므로 Transform 직접 이동
            playerTransform.position = seatPosition.position;
            playerTransform.rotation = seatPosition.rotation;
        }

        // WASD 입력으로 배 조종
        float verticalInput = Input.GetAxis("Vertical");     // W/S: 전진/후진
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D: 좌/우 회전

        // 배 회전 (Y축 기준)
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            float turnAmount = horizontalInput * boatTurnSpeed * Time.deltaTime;
            boatRigidbody.MoveRotation(boatRigidbody.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }

        // 배 전진/후진 (배의 forward 방향)
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            Vector3 forceDirection = transform.forward * verticalInput * boatSpeed;
            boatRigidbody.AddForce(forceDirection, ForceMode.Force);
        }

        // E키로 하차
        if (Input.GetKeyDown(KeyCode.E))
        {
            ExitBoat();
        }
    }

    /// <summary>
    /// 배에 탑승합니다. InteractionHandler에서 호출됩니다.
    /// </summary>
    public void BoardBoat()
    {
        if (isPlayerBoarding) return;

        isPlayerBoarding = true;

        // 1. 플레이어 이동 비활성화
        if (playerMovement != null)
        {
            playerMovement.canMove = false;
        }

        // 2. CharacterController 비활성화 (Transform 직접 제어를 위해)
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = false;
        }

        // 3. 플레이어를 seatPosition으로 이동
        if (playerTransform != null && seatPosition != null)
        {
            playerTransform.position = seatPosition.position;
            playerTransform.rotation = seatPosition.rotation;
        }

        // 4. 플레이어를 배의 자식으로 설정 (배와 함께 움직이도록)
        if (playerTransform != null)
        {
            playerTransform.SetParent(transform);
        }

        Debug.Log("배에 탑승했습니다!");
    }

    /// <summary>
    /// 배에서 하차합니다.
    /// </summary>
    public void ExitBoat()
    {
        if (!isPlayerBoarding) return;

        isPlayerBoarding = false;

        // 1. 플레이어를 배의 자식에서 해제
        if (playerTransform != null)
        {
            playerTransform.SetParent(null);
        }

        // 2. 플레이어를 exitPosition으로 이동
        if (playerTransform != null && exitPosition != null)
        {
            playerTransform.position = exitPosition.position;
            playerTransform.rotation = exitPosition.rotation;
        }

        // 3. CharacterController 재활성화
        if (playerCharacterController != null)
        {
            playerCharacterController.enabled = true;
        }

        // 4. 플레이어 이동 재활성화
        if (playerMovement != null)
        {
            playerMovement.canMove = true;
        }

        Debug.Log("배에서 하차했습니다!");
    }

    /// <summary>
    /// 플레이어가 현재 탑승 중인지 반환합니다.
    /// </summary>
    public bool IsPlayerBoarding()
    {
        return isPlayerBoarding;
    }
}
