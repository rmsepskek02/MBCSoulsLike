using BS.Player;
using BS.Utility;
using UnityEngine;

namespace BS.Enemy.Set
{
    public class GravityPull : MonoBehaviour
    {
        #region Variables
        private float pullForce = 10f; // 끌어당기는 힘의 크기
        private float maxDistance; // 최대 끌어당김 거리

        private SphereCollider sphereCollider;
        private LayerMask playerLayer;  //플레이어 레이어 마스크를 할당할 변수
        [SerializeField] private float damageCoefficient = 1f;
        #endregion

        private void Start()
        {
            sphereCollider = GetComponent<SphereCollider>();
            // 레이어마스크를 이름으로 가져와서 비트 연산: "왼쪽 쉬프트" 을 계산한다
            // 6 -> 0000 0100 0000(Total : 256) 이런식으로 레이어마스크가 할당된곳을 찾아 반환시킴
            playerLayer = 1 << LayerMask.NameToLayer(SetProperty.PLAYER_LAYER);
            maxDistance = sphereCollider.radius * Mathf.Max(transform.root.localScale.x, transform.root.localScale.y, transform.root.localScale.z);
        }

        private void FixedUpdate()
        {
            // Physics.OverlapSphere를 사용해 sphereCollider.radius 내의 "Player" 레이어만 검색
            Collider[] colliders = Physics.OverlapSphere(transform.position, maxDistance, playerLayer);

            foreach (Collider collider in colliders)
            {
                Transform target = collider.transform;
                if (target != null)
                {
                    // 방향 벡터 계산 (중심점 - 오브젝트 위치)
                    Vector3 direction = transform.position - target.position;

                    // y 값을 제외하고 x와 z 값만 사용
                    direction.y = 0;

                    // 거리 계산 (수평 거리만)
                    float distance = direction.magnitude;

                    // 거리에 따라 힘 감소 (선형 감쇠)
                    float speed = Mathf.Lerp(pullForce, 0, distance / maxDistance);

                    // 힘 적용 (x, z 축만)
                    target.position += direction.normalized * speed * Time.deltaTime;

                    //TODO : 데미지를 입히는 코드를 구현하기
                    PlayerHealth playerHealth = collider.GetComponentInChildren<PlayerHealth>();
                    if (playerHealth != null)
                    {
                        //프레임당 speed의 데미지
                        playerHealth.TakeDamage(speed * damageCoefficient, false);
                    }

#if UNITY_EDITOR
                    Debug.Log($"거리: {distance}, 대상: {collider.gameObject.name}");
                    Debug.Log($"속도: {speed}, 방향: {direction.normalized}");
#endif

                }
            }
        }

        #region 테스트 범위확인용 기즈모
        //private void OnDrawGizmos()
        //{
        //    Gizmos.color = Color.blue;
        //    Gizmos.DrawWireSphere(transform.position, maxDistance);
        //}
        #endregion
    }
}