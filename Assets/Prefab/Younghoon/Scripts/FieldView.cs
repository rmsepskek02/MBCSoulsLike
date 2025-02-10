using BS.Player;
using BS.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BS.Enemy.Set
{
    [System.Serializable]
    public struct CastInfo
    {
        public bool Hit;               // 맞았는지 여부
        public Vector3 Point;          // 맞은 지점
        public float Distance;         // 도달 거리
    }

    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class FieldView : MonoBehaviour
    {
        [Header("Circle")]
        [Range(0, 30)]
        [SerializeField] private float maxViewRange = 10f;  // 최종 범위
        [Range(0, 360)]
        [SerializeField] private float viewAngle = 90f;    // 각도

        [Header("Target")]
        private LayerMask obstacleMask;   // 장애물 대상
        private LayerMask targetMask;     // 타겟 대상

        [Header("Mesh")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 0.05f, 0f);           // 위치 보정용 벡터
        [Range(0.1f, 1f)]
        [SerializeField] private float angleStep = 1f;     // 선이 표시될 각도. 작을 수록 촘촘해짐

        [Header("Animation")]
        [SerializeField] private float growthSpeed = 8.5f;   // 영역 커지는 속도

        private float currentViewRange = 0f;               // 현재 범위
        private Mesh viewMesh;
        private MeshFilter meshFilter;
        [SerializeField] private Material material;
        private MeshRenderer meshRenderer;

        private bool isAttacked = false;

        [Header("SFX")]
        public AudioClip audioClip;

        [Header("AttackDamage")]
        [SerializeField] private float damage = 50f;

        [Header("WaringSquare")]
        [SerializeField] private GameObject waringSquarePrefab;
        private GameObject waringSquare;
        [SerializeField] private Vector3 waringSquareOffset;

        [Header("Player")]
        private PlayerController playerController;


        private void Awake()
        {
            //참조
            meshFilter = GetComponent<MeshFilter>();
            meshRenderer = GetComponent<MeshRenderer>();

            //초기화
            viewMesh = new Mesh();
            meshFilter.mesh = viewMesh;
            meshRenderer.material = material;

            //레이어 마스크 설정
            obstacleMask = 1 << LayerMask.NameToLayer(SetProperty.OBSTACLE_LAYER);
            targetMask = 1 << LayerMask.NameToLayer(SetProperty.PLAYER_LAYER);

            //waringSquarePrefab을 생성시켜주기위한 플레이어 참조
            playerController = FindAnyObjectByType<PlayerController>();
        }

        private void OnEnable()
        {
            Debug.Log($"OnEnable{currentViewRange}");
            // 영역 초기화
            isAttacked = false;

            //오브젝트 활성화시 프리팹 생성 및 시간제약
            waringSquare = Instantiate(waringSquarePrefab, playerController.transform.position + waringSquareOffset, Quaternion.identity);
            Destroy(waringSquare, 3f);
        }

        private void Update()
        {
            //공격했으면(else문 진입시) 코드 실행 방지
            if (isAttacked) return;

            //위험표시 플레이어 위에 표시
            if (waringSquare != null)
            {
                waringSquare.transform.position = playerController.transform.position + waringSquareOffset;
            }

            // 영역이 최대 범위에 도달하지 않았으면 천천히 증가
            if (currentViewRange < maxViewRange)
            {
                currentViewRange += Time.deltaTime * growthSpeed;
                currentViewRange = Mathf.Min(currentViewRange, maxViewRange); // 최대치를 넘지 않도록 제한
            }
            else
            {
                currentViewRange = 0f;
                if (gameObject.name == "RoarHitBox")
                {
                    AudioUtility.CreateSFX(audioClip, transform.position, AudioUtility.AudioGroups.Explosion);
                }
                else
                {
                    AudioUtility.CreateSFX(audioClip, transform.position, AudioUtility.AudioGroups.Skill);
                }
                CheckForTargets();
            }
            // 영역 그리기
            DrawFieldOfView();

        }

        private void DrawFieldOfView()
        {
            List<Vector3> viewPoints = new List<Vector3>();

            // 첫 번째 꼭짓점 (중심점)
            viewPoints.Add(Vector3.zero + offset);

            // 부채꼴 각도 계산
            int stepCount = Mathf.RoundToInt(viewAngle / angleStep);
            float startAngle = -viewAngle * 0.5f + transform.eulerAngles.y; // 회전 값 적용

            for (int i = 0; i <= stepCount; i++)
            {
                float currentAngle = startAngle + (i * angleStep);
                CastInfo castInfo = GetCastInfo(currentAngle);

                // Transform 좌표계에 맞게 변환
                viewPoints.Add(transform.InverseTransformPoint(castInfo.Point));
            }

            // Mesh 갱신
            UpdateMesh(viewPoints);
        }

        private void UpdateMesh(List<Vector3> viewPoints)
        {
            viewMesh.Clear();

            int vertexCount = viewPoints.Count;
            Vector3[] vertices = viewPoints.ToArray();
            int[] triangles = new int[(vertexCount - 2) * 3];

            for (int i = 0; i < vertexCount - 2; i++)
            {
                triangles[i * 3] = 0;         // 시작점
                triangles[i * 3 + 1] = i + 1; // 첫 번째 꼭짓점
                triangles[i * 3 + 2] = i + 2; // 두 번째 꼭짓점
            }

            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();
        }

        private CastInfo GetCastInfo(float angle)
        {
            Vector3 dir = new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f, Mathf.Cos(angle * Mathf.Deg2Rad));
            CastInfo info;
            RaycastHit hit;

            if (Physics.Raycast(transform.position + offset, dir, out hit, currentViewRange, obstacleMask))
            {
                info.Hit = true;
                info.Point = hit.point;
                info.Distance = hit.distance;
            }
            else
            {
                info.Hit = false;
                info.Point = transform.position + offset + dir * currentViewRange;
                info.Distance = currentViewRange;
            }

            return info;
        }

        private void CheckForTargets()
        {
            if (isAttacked) return;

            isAttacked = true;

            // 범위 내의 모든 타겟 찾기
            Collider[] targetsInView = Physics.OverlapSphere(transform.position + offset, maxViewRange, targetMask);

            foreach (var target in targetsInView)
            {
                Vector3 directionToTarget = (target.transform.position + offset - (transform.position + offset)).normalized;

                // 각도 제한 확인
                if (Vector3.Angle(transform.forward, directionToTarget) <= viewAngle / 2)
                {
                    float distanceToTarget = Vector3.Distance(transform.position + offset, target.transform.position + offset);

                    // 장애물이 있는지 확인
                    if (!Physics.Raycast(transform.position + offset, directionToTarget, distanceToTarget, obstacleMask))
                    {
                        PlayerHealth playerHealth = target.GetComponentInChildren<PlayerHealth>();
                        if (playerHealth != null)
                        {
                            playerHealth.TakeDamage(damage, false);
                        }
                        return;
                    }
                }
            }
        }
        // 기즈모로 범위 확인하기
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            // OverlapSphere 범위
            Gizmos.DrawWireSphere(transform.position + offset, maxViewRange);

            // RayCast 시각화
            Collider[] targetsInView = Physics.OverlapSphere(transform.position + offset, maxViewRange, targetMask);
            foreach (var target in targetsInView)
            {
                Vector3 directionToTarget = (target.transform.position + offset - (transform.position + offset)).normalized;
                float distanceToTarget = Vector3.Distance(transform.position + offset, target.transform.position + offset);

                // RayCast를 기즈모로 그려줌
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position + offset, transform.position + offset + directionToTarget * distanceToTarget);

                // 각도 확인
                if (Vector3.Angle(transform.forward, directionToTarget) <= viewAngle / 2)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(transform.position + offset, directionToTarget, out hit, distanceToTarget, obstacleMask))
                    {
                        Gizmos.color = Color.red;

                    }
                    else
                    {
                        Gizmos.color = Color.green;  // 장애물이 없으면 초록색
                    }
                    //Gizmos.color = Color.yellow;
                    Gizmos.DrawLine(transform.position + offset, transform.position + offset + directionToTarget * maxViewRange);
                }
            }
        }
#endif
    }
}
