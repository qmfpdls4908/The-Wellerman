using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBoatController : MonoBehaviour
{
    [Header("배 조종 설정")]
    public float moveSpeed = 1f;        // 전진/후진 속도
    public float turnSpeed = 10f;        // 회전 속도

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("TestBoatController: Rigidbody가 필요합니다!");
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // W: 전진, S: 후진
        float moveInput = 0f;
        if (Input.GetKey(KeyCode.W)) moveInput = -1f;
        if (Input.GetKey(KeyCode.S)) moveInput = 1f;

        // A: 왼쪽 회전, D: 오른쪽 회전
        float turnInput = 0f;
        if (Input.GetKey(KeyCode.A)) turnInput = -1f;
        if (Input.GetKey(KeyCode.D)) turnInput = 1f;

        // 배 회전 (월드 Y축 기준) - Rigidbody 제약에 영향받지 않도록 Transform 직접 회전
        if (Mathf.Abs(turnInput) > 0.01f)
        {
            float turnAmount = turnInput * turnSpeed * Time.fixedDeltaTime;
            transform.Rotate(0f, turnAmount, 0f, Space.World);
        }

        // Y축 회전(yaw)만 사용해서 수평 방향 계산 (모델 축 방향과 무관하게 XZ 평면에서만 이동)
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            float yawRad = transform.eulerAngles.y * Mathf.Deg2Rad;
            Vector3 horizontalForward = new Vector3(Mathf.Sin(yawRad), 0f, Mathf.Cos(yawRad));
            Vector3 force = horizontalForward * moveInput * moveSpeed;
            rb.AddForce(force, ForceMode.Force);
        }
    }
}
