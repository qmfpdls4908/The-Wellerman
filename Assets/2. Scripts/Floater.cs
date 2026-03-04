using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    /*
     * | rigidbody | 리지드바디 참조 | 부유시킬 물체의 Rigidbody 컴포넌트를 연결해요. 없으면 안 떠요! |
     * | depthBeforeSubmerged | 잠기기 시작 깊이 | 물에 얼마나 깊이 들어가야 완전한 부력이 발생할지 설정 (기본값: 1m) |
     * | displacementAmount | 부력 세기 | 물에서 위로 밀어올리는 힘의 크기 (기본값: 3). 클수록 더 강하게 떠요! |
     * | floaterCOunt | 부유체 개수 ⚠️ | (오타 주의: floaterCOunt → floaterCount) 배 전체에 이 스크립트가 몇 개 달려있는지. 중력을 나눠서 계산해요 |
     * | waterDrag | 물 저항 (선형) | 물속에서 움직일 때 속도가 줄어드는 정도 (0.99 = 1% 감속). 1에 가까울수록 미끄러짐 |
     * | waterAngularDrag | 물 저항 (회전) | 물속에서 회전할 때 저항 (0.5 = 회전이 절반으로 줄어듦) |
    */
    public Rigidbody rigidbody;
    public float depthBeforeSubmerged = 1f;
    public float displacementAmount = 3f;
    public int floaterCount = 1;
    public float waterDrag = 0.99f;
    public float waterAngularDrag = 0.5f;

    private void FixedUpdate()
    {
        rigidbody.AddForceAtPosition(Physics.gravity / floaterCount, transform.position, ForceMode.Acceleration);
        float waveHeight = WaveManager.instance.GetWaveHeight(transform.position.x);
        if (transform.position.y < waveHeight)
        {
            float displacementMultiplier = Mathf.Clamp01((waveHeight - transform.position.y) / depthBeforeSubmerged) * displacementAmount;
            rigidbody.AddForceAtPosition(new Vector3(0f, Mathf.Abs(Physics.gravity.y) * displacementMultiplier, 0f), transform.position, ForceMode.Acceleration);
            rigidbody.AddForce(displacementMultiplier * -rigidbody.velocity * waterDrag * Time.fixedDeltaTime, ForceMode.VelocityChange) ;
            rigidbody.AddTorque(displacementMultiplier * -rigidbody.angularVelocity * waterAngularDrag * Time.fixedDeltaTime, ForceMode.VelocityChange) ;
        }
    }

}
