using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

public class InteractionHandler : MonoBehaviour
{
    // === 인스펙터에서 설정할 변수들 ===
    public float interactionDistance = 3.0f; // 상호작용 가능한 최대 거리
    public LayerMask interactableLayer;      // 상호작용 가능한 오브젝트가 속한 레이어 (예: "Interactable")

    // UI 텍스트 오브젝트 참조
    public TextMeshProUGUI interactionPromptText;

    // === 내부 사용 변수 ===
    private Camera playerCamera; // 플레이어 카메라 컴포넌트
    private GameObject currentInteractableObject; // 현재 바라보고 있는 상호작용 오브젝트

    // Start는 게임 시작 시 한 번 호출됩니다.
    void Start()
    {
        // Main Camera 컴포넌트 참조
        playerCamera = GetComponent<Camera>();

        // UI 텍스트가 Start 시점에 비활성화되어 있는지 확인
        if (interactionPromptText != null)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("InteractionPromptText가 InteractionHandler 스크립트에 할당되지 않았습니다. UI Canvas의 Text (TMP)를 할당해주세요.");
        }
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // 1. Raycast를 쏴서 특정 오브젝트를 바라보고 있는지 확인
        RaycastHit hit;
        // 카메라의 정중앙에서 앞으로 Ray를 발사합니다.
        // Physics.Raycast(시작 위치, 방향, out hit 정보, 최대 거리, 레이어 마스크)
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionDistance, interactableLayer))
        {
            // Raycast가 특정 레이어의 오브젝트에 맞았다면
            if (hit.collider.CompareTag("BoatPart")) // "BoatPart" 태그를 가진 오브젝트인지 확인
            {
                // 현재 바라보고 있는 오브젝트가 상호작용 가능한 오브젝트라면
                currentInteractableObject = hit.collider.gameObject;

                // UI 텍스트 표시
                if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                {
                    interactionPromptText.gameObject.SetActive(true);
                }

                // "E" 키를 눌렀는지 확인
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("E 키로 상호작용! 대상: " + currentInteractableObject.name);
                    // 여기에 실제 낚시 시작, 이벤트 트리거 등의 로직을 추가합니다.
                    // 예: currentInteractableObject.GetComponent<BoatInteractionScript>().StartFishing();
                }
            }
            else
            {
                // Raycast는 맞았지만, "BoatPart" 태그가 아닌 경우
                HideInteractionPrompt();
            }
        }
        else
        {
            // Raycast가 아무것도 맞지 않았거나, 상호작용 가능한 레이어가 아닌 경우
            HideInteractionPrompt();
        }
    }

    // 상호작용 프롬프트 UI를 숨기는 헬퍼 함수
    void HideInteractionPrompt()
    {
        if (interactionPromptText != null && interactionPromptText.gameObject.activeSelf)
        {
            interactionPromptText.gameObject.SetActive(false);
        }
        currentInteractableObject = null; // 현재 상호작용 오브젝트 초기화
    }
}