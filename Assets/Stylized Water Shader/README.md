# 🌊 Stylized Water Shader 적용 가이드

> Unity 2022.3 LTS + URP (Universal Render Pipeline)용 스타일리시한 물 셰이더

---

## 📋 목차

1. [요구사항](#-요구사항)
2. [URP 설정 확인](#-urp-설정-확인-필수)
3. [빠른 시작](#-빠른-시작)
4. [Material 설정 상세 가이드](#-material-설정-상세-가이드)
5. [부력(Buoyancy) 시스템 적용](#-부력buoyancy-시스템-적용)
6. [문제해결 (Troubleshooting)](#-문제해결-troubleshooting)

---

## ✅ 요구사항

| 항목 | 버전/조건 |
|------|-----------|
| **Unity** | 2021.3 LTS 이상 (2022.3 LTS 권장) |
| **Render Pipeline** | URP (Universal Render Pipeline) |
| **Graphics API** | DirectX 11/12, OpenGL ES 3.0+, Vulkan |
| **Shader Graph** | 14.0.0 이상 |

### 지원 기능
- ✅ Depth-based 색상 그라데이션
- ✅ 굴절(Refraction) 효과
- ✅ Gerstner Waves (파도)
- ✅ Surface Foam (표면 거품)
- ✅ Intersection Foam (교차 거품)
- ✅ Blended Normals (이중 노말맵)
- ✅ 스펙큘러 조명 (메인 + 추가 라이트)
- ✅ 부력(Buoyancy) 시뮬레이션

---

## ⚙️ URP 설정 확인 (필수)

이 셰이더는 **Depth Texture**와 **Opaque Texture**를 필요로 합니다.

### 설정 방법

1. **Edit > Project Settings > Graphics** 메뉴 선택
2. **Scriptable Render Pipeline Settings**에 할당된 Asset 확인
3. 해당 Asset을 클릭하여 Inspector에서 다음 설정 확인:

#### 필수 체크 항목
```
☑️ Rendering > Depth Texture > ✓ 체크
☑️ Rendering > Opaque Texture > ✓ 체크
```

#### 참고 이미지 설명
```
┌─────────────────────────────────────┐
│  Universal Render Pipeline Asset   │
├─────────────────────────────────────┤
│  Rendering                          │
│    ☑️ Depth Texture                 │
│    ☑️ Opaque Texture                │
│       [  ] Distortion               │
└─────────────────────────────────────┘
```

⚠️ **중요**: 두 옵션이 비활성화되어 있으면 물이 검정색으로 보이거나 깊이/굴절 효과가 작동하지 않습니다.

### URP Asset이 없는 경우

`Stylized Water Shader/URP-HighFidelity.asset`을 Graphics Settings에 할당할 수 있습니다.

---

## 🚀 빠른 시작

### Step 1: 물 오브젝트 생성

1. **Hierarchy** 창에서 우클릭
2. **3D Object > Plane** 선택
3. 이름을 "Water" 또는 "WaterPlane"으로 변경 (선택사항)

#### 메쉬 선택 가이드

| 메쉬 타입 | 정점 수 | 사용 시기 |
|-----------|---------|-----------|
| **Plane** | 121 (11×11) | 테스트용 (파도가 각질 수 있음) |
| **Quad** | 4 | 정적인 물, 침수 효과 |
| **Grid** (권장) | 1000+ | 부드러운 파도가 필요할 때 |
| **Custom High-Poly** | 5000+ | 최고 품질의 파도 |

**권장**: Plane의 Scale을 키우는 대신, 고해상도 Grid 메쉬를 사용하세요.

### Step 2: Material 적용

1. **Project** 창에서 `Assets/Stylized Water Shader/Materials/` 이동
2. `Shader Graphs_Stylized Water.mat` 파일을 찾습니다
3. 이 Material을 Hierarchy의 Plane 오브젝트에 **드래그 앤 드롭**하거나:
   - Plane 선택 → **Mesh Renderer** 컴포넌트 → **Materials** 섹션 → Element 0에 할당

### Step 3: 기본 설정값 적용

Inspector에서 Material을 선택하고 다음 기본값을 추천합니다:

```yaml
Colors:
  Shallow Color: RGBA(0, 0.8, 0.9, 0.6)   # 밝은 청록색
  Deep Color: RGBA(0, 0.2, 0.6, 0.9)     # 진한 파란색
  Horizon Color: RGBA(0.8, 0.9, 1, 0.3)  # 하늘색
  Underwater Color: RGBA(0.2, 0.8, 0.9, 0.5)

Waves:
  Steepness: 0.3
  Wavelength: 10
  Speed: 2
  Directions: (0, 0.25, 0.5, 0.75)  # 4개 파도 방향
```

### Step 4: 테스트 환경 구성

물의 효과를 제대로 보기 위해:

1. **바닥 오브젝트 배치**: 물 아래에 Terrain, Plane, 또는 큐브들을 배치하여 깊이 효과 확인
2. **조명 설정**: Directional Light 배치 (스펙큘러 하이라이트 확인용)
3. **침수 오브젝트**: 물에 닿는 돌, 기둥 등을 배치하여 Intersection Foam 확인

---

## 🎨 Material 설정 상세 가이드

### Colors (색상)

#### Shallow Color & Deep Color
- **Shallow**: 얕은 물의 색상 (Alpha는 투명도)
- **Deep**: 깊은 물의 색상 (Alpha는 투명도)
- 두 색상은 Depth Fade 값에 따라 자동 보간됨

**팁**: Alpha 값이 높을수록 물 아래가 덜 보입니다.

#### Horizon Color
- 수평선 근처에서 보이는 색상
- Fresnel 효과로 인해 물을 낮은 각도에서 볼 때 강하게 나타남
- 하늘색이나 안개색과 일치시키면 자연스러움

#### Underwater Color
- 굴절된 부분에 적용되는 색조
- 물 아래 오브젝트에 청록색 틴트를 주는 역할

### Refraction (굴절)

| 속성 | 설명 | 범위 |
|------|------|------|
| **Refraction Strength** | UV 왜곡 강도 | 0 ~ 1 |
| **Refraction Speed** | 노이즈 이동 속도 | 0 ~ 10 |
| **Refraction Scale** | 노이즈 타일링 | 0.1 ~ 10 |

**권장값**: Strength 0.1 ~ 0.3, Speed 0.5 ~ 1

### Waves (Gerstner Waves)

4개의 파도가 서로 다른 방향으로 이동하며 합쳐집니다.

| 속성 | 설명 | 권장값 |
|------|------|--------|
| **Steepness** | 파도의 가파름 (0=평평, 1=뾰족) | 0.2 ~ 0.5 |
| **Wavelength** | 파도 길이 (미터) | 5 ~ 20 |
| **Speed** | 파도 이동 속도 | 1 ~ 5 |
| **Directions** | 4개 파도의 방향 (0~1) | 0, 0.25, 0.5, 0.75 |

**Direction 설명**: 
- 0 = 0°, 0.25 = 90°, 0.5 = 180°, 0.75 = 270°
- 4개 방향이 서로 교차하면 더 자연스러운 파도가 됨

### Surface Foam (표면 거품)

| 속성 | 설명 |
|------|------|
| **Surface Foam Texture** | 거품 패턴 텍스처 (기본: Foam 4.png) |
| **Surface Foam Color** | 거품 색상 (보통 흰색) |
| **Surface Foam Cutoff** | 거품 임계값 (높을수록 거품이 작아짐) |
| **Surface Foam Tiling** | 텍스처 반복 크기 |
| **Surface Foam Speed** | 이동 속도 |
| **Surface Foam Distortion** | UV 왜곡 정도 |

### Intersection Foam (교차 거품)

물과 다른 오브젝트가 만나는 경계에 생성되는 거품입니다.

| 속성 | 설명 |
|------|------|
| **Intersection Foam Texture** | 거품 텍스처 (기본: Foam 5.png) |
| **Intersection Foam Color** | 거품 색상 |
| **Intersection Foam Cutoff** | 거품 임계값 |
| **Intersection Foam Fade** | 깊이 기반 페이드 범위 (작을수록 뾰족) |
| **Intersection Foam Tiling** | 텍스처 타일링 |
| **Intersection Foam Speed** | 이동 속도 |

### Normals (노말맵)

| 속성 | 설명 | 권장값 |
|------|------|--------|
| **Normals Texture** | 노말맵 텍스처 (기본: Normals 1.png) |
| **Normals Speed** | 이동 속도 | 0.5 ~ 1 |
| **Normals Scale** | 타일링 크기 | 2 ~ 5 |
| **Normals Strength** | 노말 강도 (조명에 영향) | 0.5 ~ 1 |

### Lighting (조명)

| 속성 | 설명 | 범위 |
|------|------|------|
| **Smoothness** | 스펙큘러 부드러움 | 0 ~ 1 |
| **Specular Hardness** | 하드/소프트 스펙큘러 전환 | 0 ~ 1 |
| **Specular Color** | 하이라이트 색상 | (1,1,1,1) |

---

## 🛟 부력(Buoyancy) 시스템 적용

부력 시스템을 사용하면 오브젝트가 물 위에 떠다니는 효과를 구현할 수 있습니다.

### 구성 요소

| 스크립트 | 역할 |
|----------|------|
| **GerstnerWaveDisplacement.cs** | CPU에서 파도 수학 계산 |
| **BuoyantObject.cs** | 뜨는 오브젝트에 부력 적용 |

### 설정 단계

#### Step 1: 부력 오브젝트 준비

1. 뜨게 만들고 싶은 오브젝트(예: 보트, 통나무, 부표) 선택
2. 반드시 **Rigidbody** 컴포넌트가 있어야 함 (없으면 Add Component)
   - Mass: 적절한 무게 설정 (예: 100)
   - Drag: 0.5 ~ 2 (물의 저항)
   - Angular Drag: 0.5 ~ 1
   - Use Gravity: ✓ 체크

#### Step 2: Effector 배치

Effector는 부력이 적용되는 "포인트"입니다.

1. 부력 오브젝트를 선택한 상태에서 **자식 오브젝트 생성** (우클릭 > Create Empty)
2. 자식 오브젝트 이름: "Effector1", "Effector2" 등
3. 이 오브젝트들을 물에 닿는 위치에 배치:
   ```
   보트 예시:
   ┌─────────────────┐
   │  ◇         ◇   │  ← Effector 1, 2 (앞쪽)
   │                 │
   │  ◇         ◇   │  ← Effector 3, 4 (뒤쪽)
   └─────────────────┘
   ```
4. Effector의 Y 위치는 물 표면과 비슷하게 설정 (약간 아래)

#### Step 3: BuoyantObject 스크립트 적용

1. 루트 오브젝트(보트 본체)에 **Add Component > BuoyantObject**
2. Inspector 설정:

```yaml
Buoyancy Effectors:
  Size: 4 (Effector 개수)
  Element 0: Effector1 (드래그)
  Element 1: Effector2 (드래그)
  Element 2: Effector3 (드래그)
  Element 3: Effector4 (드래그)

Buoyancy Settings:
  Buoyancy Force: 100 ~ 500 (무게에 따라 조절)
  
Wave Settings (Material과 일치시켜야 함!):
  Steepness: 0.3 (Material의 값과 동일)
  Wavelength: 10 (Material의 값과 동일)
  Speed: 2 (Material의 값과 동일)
  Directions: (0, 0.25, 0.5, 0.75) (Material과 동일)
```

⚠️ **중요**: Wave Settings는 물 Material의 값과 **정확히 일치**해야 합니다. 그렇지 않으면 오브젝트가 파도와 동기화되지 않습니다.

### 팁

- **Effector가 많을수록**: 안정적이지만 계산 비용 증가
- **Effector가 적을수록**: 가볍지만 뒤집힐 수 있음
- 일반적인 오브젝트: 4개 권장
- 큰 선박: 6~8개
- 작은 부표: 1~2개

---

## 🔧 문제해결 (Troubleshooting)

### 문제 1: 물이 검정색/병란색으로 보임

**증상**: 물 표면이 검정색이거나 아래 지형이 보이지 않음

**원인**:
- Depth Texture 또는 Opaque Texture 비활성화
- URP Asset이 제대로 설정되지 않음

**해결**:
1. `Edit > Project Settings > Graphics`에서 URP Asset 확인
2. 해당 Asset의 **Depth Texture**와 **Opaque Texture** 활성화
3. 씬 뷰/게임 뷰 새로고침 (Ctrl+R)

---

### 문제 2: 파도가 보이지 않거나 각진 모양

**증상**: 물이 평평하거나, 파도가 마인크래프트처럼 각짐

**원인**: 메쉬의 정점 수가 부족

**해결**:
- Plane 메쉬 대신 **고해상도 Grid** 사용
- 또는 Custom 메쉬로 정점 수 늘리기
- Material의 Steepness 값이 0인지 확인

---

### 문제 3: 거품(Foam)이 보이지 않음

**증상**: 물 표면이나 교차점에 거품이 없음

**해결**:
1. Material Inspector에서 **Surface Foam Cutoff**가 1이 아닌지 확인 (0.5 권장)
2. Foam Texture가 제대로 연결되어 있는지 확인
3. Intersection Foam의 경우, 오브젝트가 물 표면과 교차하는지 확인
4. Tiling 값이 너무 작거나 큰지 확인 (1~5 권장)

---

### 문제 4: 굴절(Refraction)이 작동하지 않음

**증상**: 물 아래가 왜곡되지 않고 그대로 보임

**원인**: Opaque Texture 비활성화

**해결**:
1. URP Asset에서 **Opaque Texture** 활성화
2. Refraction Strength가 0보다 큰지 확인

---

### 문제 5: Buoyancy가 작동하지 않음 (오브젝트가 물에 뜨지 않음)

**증상**: 오브젝트가 물을 뚫고 가라앉음

**해결 체크리스트**:
- [ ] 오브젝트에 **Rigidbody**가 있는가?
- [ ] Buoyancy Force가 충분히 큰가? (무게 대비)
- [ ] **Wave Settings가 Material과 일치**하는가? (가장 흔한 실수)
- [ ] Effector가 올바르게 할당되었는가?
- [ ] Effector 위치가 물 표면 근처인가?

---

### 문제 6: 조명이 이상하게 보임 (스펙큘러 없음)

**증상**: 물 표면에 하이라이트/반사광이 없음

**해결**:
1. 씬에 **Directional Light**가 있는지 확인
2. Material의 **Smoothness**가 0보다 큰지 확인
3. **Normals Strength**가 0보다 큰지 확인
4. Additional Lights를 사용하려면 씬에 Point/Spot Light 추가

---

### 문제 7: 성능 저하 (FPS 하락)

**원인**: Gerstner Waves 계산이 무거움

**해결**:
1. 메쉬의 정점 수 줄이기
2. Material에서 **Normals Speed**를 0으로 설정 (정지된 노말)
3. Wave 계산을 줄이거나 더 단순한 물 셰이더 사용
4. 모바일의 경우 Steepness를 낮추고 Wavelength를 길게 설정

---

## 📚 추가 자료

### 폴리 카운트 가이드

| 플랫폼 | 권장 정점 수 | 비고 |
|--------|--------------|------|
| **Mobile** | 100 ~ 500 | 단순한 파도 |
| **PC Low** | 500 ~ 2000 | 기본 품질 |
| **PC High** | 2000 ~ 10000 | 고품질 파도 |
| **PC Ultra** | 10000+ | 영화급 품질 |

### 참고 링크

- [Unity URP Documentation](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@14.0/manual/index.html)
- [Shader Graph Documentation](https://docs.unity3d.com/Packages/com.unity.shadergraph@14.0/manual/index.html)
- Gerstner Waves 수학: [Catlike Coding - Waves](https://catlikecoding.com/unity/tutorials/flow/waves/)

---

## 📝 업데이트 로그

- **2024.XX.XX**: 초기 버전 (Unity 2022.3 LTS 기준)

---

**질문이나 문제가 있으시면 프로젝트 담당자에게 문의해 주세요.**

*Made with 💧 for Unity URP*
