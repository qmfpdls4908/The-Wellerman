# 배 내부 물 마스킹 가이드

배가 반쯤 잠길 때 물 메쉬가 배 안까지 보이는 문제를 해결합니다.

---

## 원리 (간단 요약)

- 배 안에 **보이지 않는 Quad 메쉬**를 깔아둡니다
- 이 메쉬가 Stencil Buffer에 "여기는 물 그리지 마" 라고 표시합니다
- 물 셰이더가 표시된 영역은 건너뜁니다

---

## STEP 1. Layer 생성

`Edit > Project Settings > Tags and Layers` 로 이동합니다.

빈 슬롯에 아래 2개 Layer를 추가합니다.

| 슬롯 | 이름 |
|------|------|
| User Layer 6 (또는 빈 슬롯) | WaterMask |
| User Layer 4 (또는 빈 슬롯) | Water |

> Water Layer가 이미 있으면 새로 안 만들어도 됩니다.

---

## STEP 2. Material 생성

1. Project 창에서 우클릭 > `Create > Material`
2. 이름: **WaterMask_Mat**
3. Inspector 상단의 Shader 드롭다운 클릭
4. `Custom > WaterMask` 선택

> WaterMask 셰이더 파일 위치:
> `Stylized Water Shader/Shaders/WaterMask.shader`

---

## STEP 3. 마스크 메쉬 배치

### 3-1. Quad 생성

1. Hierarchy에서 **배(Boat) 오브젝트** 우클릭
2. `3D Object > Quad` 선택
3. 이름을 `WaterMask` 로 변경

### 3-2. Transform 설정

| 항목 | 값 | 설명 |
|------|----|------|
| Position X | 0 | 배 중앙 |
| Position Y | 수면 높이에 맞춤 | 배 기준 로컬 Y |
| Position Z | 0 | 배 중앙 |
| Rotation X | 90 | Quad를 수평으로 눕힘 |
| Scale X, Z | 배 내부를 덮을 크기 | 넉넉하게, 배 밖으로 나가지 않게 |

### 3-3. Material 적용

- WaterMask 오브젝트의 `Mesh Renderer > Materials > Element 0`
- 아까 만든 **WaterMask_Mat** 을 드래그해서 넣기

### 3-4. Layer 변경

- WaterMask 오브젝트의 Inspector 상단
- Layer 드롭다운 > **WaterMask** 선택

### 3-5. 물 오브젝트 Layer 변경

- 물(Water Plane) 오브젝트도 Layer를 **Water** 로 변경

---

## STEP 4. Renderer Feature 추가

프로젝트에서 사용 중인 **URP Renderer Asset** 을 선택합니다.

> 확인 방법: `Edit > Project Settings > Graphics` 에서 연결된 Asset 클릭
> 또는 `Stylized Water Shader/URP-HighFidelity-Renderer.asset` 사용

### Feature 1번 추가

Inspector 하단의 `Add Renderer Feature` 클릭 > `Render Objects` 선택

| 설정 | 값 |
|------|----|
| Name | WaterMask_Write |
| Event | Before Rendering Transparents |

**Filters 섹션:**

| 설정 | 값 |
|------|----|
| Queue | Opaque |
| Layer Mask | WaterMask만 체크 |

**Overrides 섹션 > Stencil 체크 활성화:**

| 설정 | 값 |
|------|----|
| Value | 1 |
| Compare Function | Always |
| Pass | Replace |
| Fail | Keep |

---

### Feature 2번 추가

다시 `Add Renderer Feature` > `Render Objects`

| 설정 | 값 |
|------|----|
| Name | Water_StencilTest |
| Event | Before Rendering Transparents |

**Filters 섹션:**

| 설정 | 값 |
|------|----|
| Queue | Transparent |
| Layer Mask | Water만 체크 |

**Overrides 섹션 > Stencil 체크 활성화:**

| 설정 | 값 |
|------|----|
| Value | 1 |
| Compare Function | NotEqual |
| Pass | Keep |
| Fail | Keep |

---

## STEP 5. 확인

1. Play 모드 진입
2. 배를 측면에서 바라보기
3. 배 안에 물이 안 보이면 성공!

---

## 문제 해결

| 증상 | 확인할 것 |
|------|-----------|
| 마스크가 안 됨 | Layer 설정 확인 (WaterMask, Water) |
| 물이 전부 사라짐 | Renderer Feature의 Layer Mask가 정확한지 확인 |
| 배 밖 물도 잘림 | 마스크 메쉬가 배 밖으로 나가지 않게 크기 조절 |
| 특정 각도에서 깨짐 | 마스크 메쉬의 Y 위치를 수면보다 살짝 위로 조정 |

---

## Hierarchy 구조 (최종)

```
Boat (Root)
 ├── Hull (선체)
 ├── Deck (갑판)
 ├── Effector1 (부력)
 ├── Effector2 (부력)
 └── WaterMask (Quad) ← Layer: WaterMask, Material: WaterMask_Mat
```

> 배의 자식이므로 배가 움직이면 마스크도 같이 따라갑니다.
