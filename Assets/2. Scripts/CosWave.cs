using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class CosWave : MonoBehaviour
{
    [Header("Perlin Noise Settings")]
    [Tooltip("파도의 속도 (시간 변화율)")]
    public float speed = 0.1f;

    [Tooltip("파도의 높이(강도)")]
    public float noiseStrength = 0.3f;

    [Tooltip("Perlin Noise 이동 속도")]
    public float noiseWalk = 0.5f;

    [Header("Wave Modulation")]
    [Tooltip("Cos 파동 효과 강도 (0 = Perlin만 적용)")]
    public float waveAmplitude = 0.3f;

    [Tooltip("파도의 밀도(주파수)")]
    public float frequency = 0.5f;

    [Header("Optional")]
    [Tooltip("MeshCollider 자동 업데이트 (성능 저하될 수 있음)")]
    public bool updateCollider = false;

    private Mesh _mesh;
    private Vector3[] _baseVertices;
    private MeshCollider _collider;
    private float _baseHeight;

    // IWater 인터페이스 구현
    public float BaseHeight => _baseHeight;

    void Start()
    {
        _mesh = GetComponent<MeshFilter>().mesh;

        // 원본 정점 위치를 저장 (중요!)
        _baseVertices = _mesh.vertices.Clone() as Vector3[];
        
        // 기본 높이 계산 (메시의 평균 Y값)
        CalculateBaseHeight();

        if (updateCollider)
            _collider = GetComponent<MeshCollider>();
    }
    
    void CalculateBaseHeight()
    {
        if (_baseVertices == null || _baseVertices.Length == 0) return;
        
        float sumY = 0f;
        foreach (var v in _baseVertices)
        {
            sumY += v.y;
        }
        _baseHeight = sumY / _baseVertices.Length + transform.position.y;
    }

    void Update()
    {
        Vector3[] vertices = new Vector3[_baseVertices.Length];
        float time = Time.time;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = _baseVertices[i];
            
            // 월드 좌표로 변환하여 파도 계산
            Vector3 worldPos = transform.TransformPoint(vertex);
            float waveHeight = CalculateWaveHeight(worldPos, time);
            
            // 로컬 Y에 적용
            vertex.y = _baseVertices[i].y + waveHeight;
            vertices[i] = vertex;
        }

        _mesh.vertices = vertices;
        _mesh.RecalculateNormals();
        _mesh.RecalculateBounds();

        // MeshCollider 업데이트 (선택적)
        if (updateCollider && _collider != null)
            _collider.sharedMesh = _mesh;
    }
    
    /// <summary>
/// 주어진 월드 위치에서의 파도 높이 계산 (IWater 인터페이스용)
    /// </summary>
    public float GetWaterHeightAtPosition(Vector3 worldPosition)
    {
        // 로컬 좌표로 변환
        Vector3 localPos = transform.InverseTransformPoint(worldPosition);
        float waveHeight = CalculateWaveHeight(worldPosition, Time.time);
        return _baseHeight + waveHeight;
    }
    
    /// <summary>
    /// 파도 높이 계산 공식
    /// </summary>
    private float CalculateWaveHeight(Vector3 worldPos, float time)
    {
        // Perlin Noise 기반 파도 + Cos 변조
        float noiseX = worldPos.x * frequency + noiseWalk * time * speed;
        float noiseZ = worldPos.z * frequency + Mathf.Cos(time * speed) * 0.5f;

        float perlinValue = Mathf.PerlinNoise(noiseX, noiseZ);

        // Cos 함수로 추가적인 파동 효과
        float waveModulation = Mathf.Cos(time + worldPos.x * frequency + worldPos.z * frequency) * waveAmplitude;

        return (perlinValue + waveModulation) * noiseStrength;
    }

    void OnDestroy()
    {
        // 메시 원상복구 (에디터에서 플레이 종료 후에도 원래 모양 유지)
        if (_mesh != null && _baseVertices != null)
        {
            _mesh.vertices = _baseVertices;
            _mesh.RecalculateNormals();
            _mesh.RecalculateBounds();
        }
    }
}
