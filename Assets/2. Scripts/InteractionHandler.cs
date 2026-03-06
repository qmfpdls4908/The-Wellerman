using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 필요합니다.

public class InteractionHandler : MonoBehaviour
{
    // === 인스펙터에서 설정할 변수들 ===
    public float interactionDistance = 3.0f; // 상호작용 가능한 최대 거리
    public LayerMask interactableLayer;      // 상호작용 가능한 오브젝트가 속한 레이어 (예: "Interactable")

    [Header("보트 조종 설정")]
    public string boatControllerObjectName = "BoatController"; // 조종 가능한 보트 오브젝트 이름

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

                // 오브젝트 이름이 인스펙터에서 설정한 이름과 일치하는지 확인
                if (currentInteractableObject.name == boatControllerObjectName)
                {
                    BoatController boatController = currentInteractableObject.GetComponentInParent<BoatController>();
                    
                    // 탑승 중이 아닐 때만 프롬프트 표시 및 상호작용 가능
                    if (boatController != null && !boatController.IsPlayerBoarding())
                    {
                        // UI 텍스트 표시
                        if (interactionPromptText != null && !interactionPromptText.gameObject.activeSelf)
                        {
                            interactionPromptText.gameObject.SetActive(true);
                        }

                        // "E" 키를 눌렀는지 확인
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            Debug.Log("E 키로 보트 탑승! 대상: " + currentInteractableObject.name);
                            boatController.BoardBoat();
                            HideInteractionPrompt(); // 탑승 후 프롬프트 숨기기
                        }
                    }
                    else if (boatController == null)
                    {
                        // 이름은 일치하지만 BoatController 컴포넌트가 없는 경우
                        HideInteractionPrompt();
                        Debug.LogWarning("대상 부모에 BoatController 컴포넌트가 없습니다.");
                    }
                    else
                    {
                        // 이미 탑승 중인 경우
                        HideInteractionPrompt();
                    }
                }
                else
                {
                    // 이름이 일치하지 않는 다른 BoatPart일 경우
                    HideInteractionPrompt();
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