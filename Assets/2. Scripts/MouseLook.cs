using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 100f; // 마우스 감도

    public Transform playerBody; // 플레이어 오브젝트의 Transform (Y축 회전용)

    float xRotation = 0f;

    void Start()
    {
        // 이미 PlayerMovement 스크립트에서 처리했으므로 여기서는 생략 가능.
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void Update()
    {
        // 마우스 입력 값 가져오기
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // 위아래 시점 회전 (카메라)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 고개 꺾임을 방지하기 위해 각도 제한
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 좌우 시점 회전 (플레이어 바디)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}