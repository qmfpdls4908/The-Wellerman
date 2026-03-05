using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    // 외부에서 이동 제어 (BoatController 등에서 사용)
    public bool canMove = true;

    // 이동 속도
    public float moveSpeed = 5.0f;
    // 중력 가속도
    public float gravity = -9.81f;
    // 점프 힘 (원한다면 추가)
    public float jumpHeight = 3.0f;

    // 현재 수직 속도 (중력 및 점프에 사용)
    private Vector3 velocity;

    // === Moving Platform (배 위에서 부드럽게 따라가기) ===
    private Transform currentPlatform;       // 현재 밟고 있는 플랫폼
    private Vector3 platformLastPosition;    // 이전 프레임의 플랫폼 위치
    private Vector3 platformDelta;           // 플랫폼의 이동량

    // Start는 게임 시작 시 한 번 호출됩니다.
    void Start()
    {
        // 현재 GameObject에서 CharacterController 컴포넌트를 가져옵니다.
        controller = GetComponent<CharacterController>();

        // 1인칭 시점에서 마우스 커서를 잠그고 숨깁니다.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        if (!canMove) return;

        // 0. Moving Platform: 플랫폼의 이동량을 먼저 적용
        if (currentPlatform != null)
        {
            platformDelta = currentPlatform.position - platformLastPosition;
            controller.Move(platformDelta);
            platformLastPosition = currentPlatform.position;
        }

        // 1. 플레이어가 땅에 닿아있는지 확인하고, 닿아있으면 수직 속도를 0으로 초기화
        // CharacterController.isGrounded는 CharacterController가 땅에 닿아있는지 여부를 반환합니다.
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 땅에 살짝 붙어있도록 작은 음수 값 설정
        }

        // 2. WASD 입력 값 가져오기
        float horizontalInput = Input.GetAxis("Horizontal"); // 좌우 (A, D)
        float verticalInput = Input.GetAxis("Vertical");     // 전후 (W, S)

        // 3. 이동 방향 계산 (Transform.Translate 방식과 동일)
        Vector3 move = transform.right * horizontalInput + transform.forward * verticalInput;

        // 4. 대각선 이동 속도 보정 (정규화)
        if (move.magnitude > 0.01f)
        {
            move.Normalize();
        }

        // 5. CharacterController를 이용한 이동
        // controller.Move()는 벡터만큼 CharacterController를 이동시키며,
        // 충돌을 자동으로 처리합니다. (Rigidbody처럼 물리력을 받지는 않습니다.)
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 6. 점프 처리 (선택 사항)
        if (Input.GetButtonDown("Jump") && controller.isGrounded) // "Jump"는 스페이스바에 매핑되어 있습니다.
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 7. 중력 적용
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime); // 중력에 의한 수직 이동 적용
    }

    // CharacterController가 콜라이더에 닿았을 때 호출됩니다.
    // 발밑의 플랫폼(배)을 감지하여 추적을 시작합니다.
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // 발밑(아래쪽)에서 닿은 경우만 플랫폼으로 인식
        if (hit.moveDirection.y < -0.3f && hit.normal.y > 0.5f)
        {
            if (currentPlatform != hit.transform)
            {
                currentPlatform = hit.transform;
                platformLastPosition = currentPlatform.position;
            }
        }
    }

    // 매 프레임 끝에서 플랫폼 추적 해제 여부를 판단합니다.
    void LateUpdate()
    {
        // 공중에 있으면 플랫폼 추적 해제
        if (!controller.isGrounded)
        {
            currentPlatform = null;
        }
    }
}
