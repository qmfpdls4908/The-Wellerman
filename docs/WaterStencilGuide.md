# Unity URP에서 보트 안으로 물 들어오지 않게 하는 방법 (Stencil Buffer)

## 개요

보트가 물 위에 떠 있을 때, 보트 중앙이 뚤려있으면 물이 보트 안으로 들어온다. 이를 해결하기 위해 **Stencil Buffer**를 사용하여 보트 영역 안쪽에는 물이 렌더링되지 않도록 설정한다.

---

##Stencil Buffer 원리

| 단계 | 설명 |
|------|------|
| **Stencil Mask** | 특정 물체(보트)가 렌더링될 때, 해당 영역의 stencil buffer에 값을 기록한다. 실제 색상은 렌더링되지 않고 stencil 값만 기록한다. |
| **Stencil Test** | 다른 물체(물)가 렌der링될 때 stencil buffer를 확인하여, 특정 값이 있으면 그리지 않도록(discard) 설정한다. |

```
┌─────────────────────────────────────────────────────────┐
│  [보트] 렌더링 시: Stencil ID = 1 기록                   │
│                                                         │
│  [물] 렌더링 시: Stencil ID = 1 이면 Alpha = 0 (투명)    │
│                                                         │
│  결과: 보트 안쪽으로는 물이 보이지 않음!                 │
└─────────────────────────────────────────────────────────┘
```

---

## 구현 방법

### 방법 1: 보트 Material에 Stencil 설정 (권장)

이 방법은 보트 Material 자체에 Stencil을 설정하는 가장 간단한 방법이다.

#### Step 1: 보트 Material 선택

프로젝트에서 보트에 적용된 Material을 선택한다.

#### Step 2: Surface Type 확인

Inspector에서 다음을 확인한다:

- **Surface Type**: `Opaque` (중요! Transparent면 Stencil이 동작하지 않음)
- **Alpha Clipping**: 체크 해제

#### Step 3: C# 스크립트로 Stencil 설정

URP Material Inspector에는 Stencil 옵션이 없으므로, 스크립트로 직접 설정해야 한다.

**StencilBoat.cs 스크립트 생성:**

```csharp
using UnityEngine;

[ExecuteInEditMode]
public class StencilBoat : MonoBehaviour
{
    [Header("Stencil Settings")]
    [Tooltip("보트에 적용할 Stencil ID (Water와 맞춰야 함)")]
    public int stencilId = 1;

    [Tooltip("비활성화 시 Stencil 해제")]
    public bool enableStencil = true;

    void Start()
    {
        ApplyStencil();
    }

    void OnValidate()
    {
        ApplyStencil();
    }

    void ApplyStencil()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        Material mat = rend.sharedMaterial;
        if (mat == null) return;

        if (enableStencil)
        {
            // Stencil 설정 (보트가 렌더링 될 때 stencil 값 기록)
            mat.SetInt("_StencilWriteMask", stencilId);
            mat.SetInt("_StencilReadMask", stencilId);
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Replace);
            
            // Reference 값 설정
            mat.SetInt("_Stencil", stencilId);
        }
        else
        {
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Always);
            mat.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Keep);
        }
    }
}
```

#### Step 4: 보트 GameObject에 스크립트 적용

1. 보트 GameObject 선택
2. `StencilBoat.cs` 스크립트 추가
3. **Stencil ID**: 1로 설정

---

### 방법 2: Water Shader에 Stencil 설정

이 방법은 물 Shader에 Stencil Test를 추가하여 보트 영역을 제외한다.

#### Step 1: Water Shader Graph 수정

**Stylized Water.shadergraph** 파일을 더블 클릭하여 연다.

Inspector에서 **Graph Settings**를 찾는다.

> [!NOTE]
> 만약 Advanced Options가 비활성화된 경우:
> - Surface Type을 Opaque로 변경해보세요.
> - Unity 버전에 따라 다를 수 있습니다.

#### Step 2: Render Pipeline Asset 확인

**Project Settings** → **Graphics** → **URP Asset** 에서 현재 사용하는 URP Renderer 설정을 확인한다.

#### Step 3: Alternative - C#으로 Water Material 수정

Water Material에 접근하여 스크립트로 Stencil을 설정한다.

**StencilWater.cs 스크립트 생성:**

```csharp
using UnityEngine;

[ExecuteInEditMode]
public class StencilWater : MonoBehaviour
{
    [Header("Stencil Settings")]
    [Tooltip(" Water의 Stencil ID (보트와 맞춰야 함)")]
    public int stencilId = 1;

    [Tooltip("보트 뒤의 물을 투명하게 처리")]
    public bool enableStencilTest = true;

    void Start()
    {
        ApplyStencilToWater();
    }

    void OnValidate()
    {
        ApplyStencilToWater();
    }

    void ApplyStencilToWater()
    {
        Renderer rend = GetComponent<Renderer>();
        if (rend == null) return;

        Material mat = rend.sharedMaterial;
        if (mat == null) return;

        if (enableStencilTest)
        {
            // Stencil Test: 보트 영역(Stencil ID=1)이면 그리지 않음
            mat.SetInt("_StencilWriteMask", 0);
            mat.SetInt("_StencilReadMask", stencilId);
            mat.SetInt("_StencilComp", (int)UnityEngine.Rendering.CompareFunction.Equal);
            mat.SetInt("_StencilOp", (int)UnityEngine.Rendering.StencilOp.Zero);
            
            // Reference 값 설정
            mat.SetInt("_Stencil", stencilId);
            
            // ZWrite를 끄지 않도록 설정 (Depth는 유지)
            mat.SetInt("_ZWrite", 1);
        }
    }
}
```

#### Step 4: Water GameObject에 스크립트 적용

1. Scene의 Water0 GameObject 선택
2. `StencilWater.cs` 스크립트 추가
3. **Stencil ID**: 1로 설정 (보트와 동일)

---

### 방법 3: Stencil Mask Mesh 사용 (정확도 높음)

이 방법은 보트 모양대로 정확히 Mask를 만들 때 사용한다.

#### Step 1: 보트 내부 모양의 Mesh 준비

1. Blender나 Unity에서 보트 내부 단면 모양의 2D Mesh 생성
2. 이 Mesh는 물면보다 약간 위에 위치

#### Step 2: StencilMask Shader 생성

**StencilMask.shader 파일 생성:**

```hlsl
Shader "Custom/StencilMask"
{
    Properties
    {
        _StencilId ("Stencil ID", Int) = 1
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque" 
            "Queue" = "Geometry-100"
        }
        
        // ColorMask 0: 실제 색상은 렌더링하지 않음
        ColorMask 0
        ZWrite off
        
        Stencil
        {
            Ref[_StencilId]
            Comp always
            Pass replace
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return half4(1,1,0,1);
            }
            ENDCG
        }
    }
}
```

#### Step 3: Mesh와 Material 적용

1. 보트 내부 모양 Mesh를 Scene에 추가
2. 위 Shader로 Material 생성 후 적용
3. **Render Queue**를 Water보다 먼저 렌더링 되도록 설정 (Queue = Geometry-100)

#### Step 4: Water Shader에 Stencil Test 추가

방법 2의 StencilWater 스크립트를 Water에 적용한다.

---

## Render Queue 순서 (중요!)

Stencil이 제대로 작동하려면 Render Queue 순서가 중요하다.

| 순서 | 오브젝트 | Queue 값 | 설명 |
|------|----------|----------|------|
| 1 | Stencil Mask | Geometry-100 | 먼저 렌더링되어 stencil 값 기록 |
| 2 | 보트 | Geometry |Stencil ID 기록 |
| 3 | 물 | Geometry+1 | Stencil Test 수행 |

**설정 방법:**

1. 각 Material의 Inspector에서 **Render Queue** 확인
2. 필요시 수동으로 조정:
   - Stencil Mask: `Geometry-100`
   - 보트: `Geometry` (기본값)
   - 물: `Geometry+1` 또는 `Transparent`

---

## 문제 해결

### Q: Stencil이 작동하지 않아요

**해결 방법:**

1. **Surface Type이 Opaque인지 확인** (Transparent면 Stencil 동작 안 함)
2. **Render Queue 순서 확인** (Mask → Water 순서)
3. **Stencil ID가 양쪽에同一하게 설정되어 있는지 확인**
4. **URP Renderer 설정에서 Depth & StencilBuffers 활성화 확인**:
   - Project Settings → Graphics → URP Asset → Renderer 확인

### Q: 보트 안쪽이 하얘 보여요

**해결 방법:**

- Water Material의 **Alpha** 값을 확인
- Stencil Pass Operation을 `Zero` 대신 `Keep`으로 변경해보세요

### Q: 물이 보트 위에도 안 보여요

**해결 방법:**

- Stencil Comparison을 `Equal`에서 `NotEqual`로 변경해보세요
- 또는 Render Queue 순서를 확인하세요

---

## 요약

| 단계 | 작업 |
|------|------|
| 1 | 보트 Material의 Surface Type을 **Opaque**로 설정 |
| 2 | `StencilBoat.cs` 스크립트를 보트에 적용 (Stencil ID = 1) |
| 3 | `StencilWater.cs` 스크립트를 물에 적용 (Stencil ID = 1) |
| 4 | Render Queue 순서 확인 (Mask → Water) |
| 5 | Playして確認! |

---

## 참고 자료

- [데이터 판도라: Unity 스텐실 버퍼(Stencil Buffer)](https://data-pandora.tistory.com/entry/Unity-스텐실-버퍼Stencil-Buffer)
- Unity URP Documentation: Stencil Buffer
- Unity Shader Graph: Render States
