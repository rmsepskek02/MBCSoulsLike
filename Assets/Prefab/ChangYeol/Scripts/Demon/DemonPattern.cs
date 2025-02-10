using BS.Audio;
using BS.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Demon
{
    public class DemonPattern : MonoBehaviour
    {
        #region Variables
        //공통
        [HideInInspector]public DemonNextPhase demon;
        public GameObject Warningeffect;
        public GameObject[] effect;
        public DemonAudioManager audioManager;

        //패턴 1
        public BallRise[] ball;
        public Transform[] patternOnePoints;
        public int minSpawnCount = 4; // 최소 생성 개수
        public int maxSpawnCount = 6; // 최대 생성 개수

        //패턴 2
        public Transform ballTransfrom;
        public Transform rangeTransform;
        public GameObject ballInstantiate;

        //패턴 3
        public Transform[] teleportPoints; // 텔레포트 가능한 지점들

        //공격범위
        public GameObject[] attackRangePrefab;
        public Vector3[] attackRangeScale = new Vector3[2];
        public float[] rangeSize = new float[2];

        //플레이어 찾기
        public Transform player; // 플레이어 참조
        public float rotationSpeed = 5f; // 회전 속도
        #endregion
        private void Start()
        {
            demon = GetComponent<DemonNextPhase>();
        }
        //패턴 1
        #region Pattern 1
        public void SpawnObjects()
        {
            if (patternOnePoints.Length == 0 || ball[0] == null) return;
            int spawnCount = Random.Range(minSpawnCount, maxSpawnCount + 1); // 2 또는 3개 생성
            HashSet<int> selectedIndices = new HashSet<int>(); // 중복 방지

            // 플레이어와의 거리 계산
            List<int> closePoints = new List<int>();
            float maxDistance = 15f; // 가까운 거리의 최대 범위 설정 (예: 10 유닛)

            for (int i = 0; i < patternOnePoints.Length; i++)
            {
                float distanceToPlayer = Vector3.Distance(patternOnePoints[i].position, player.position);
                if (distanceToPlayer <= maxDistance)
                {
                    closePoints.Add(i);
                }
            }
            // 가까운 지점 중에서 랜덤 선택
            if (closePoints.Count > 0)
            {
                while (selectedIndices.Count < spawnCount && selectedIndices.Count < closePoints.Count)
                {
                    int randomIndex = closePoints[Random.Range(0,closePoints.Count)];
                    selectedIndices.Add(randomIndex);
                }
            }
            else
            {
                return;
            }

            // 선택된 지점에 오브젝트 스폰
            foreach (int index in selectedIndices)
            {
                StartCoroutine(AttackRangeSpawn(index));
                GameObject game = Instantiate(ball[0].gameObject, patternOnePoints[index].position, Quaternion.identity);
                game.GetComponent<BallRise>().StartRise();
                demon.lastAttackTime[0] = Time.time;
            }
        }
        IEnumerator AttackRangeSpawn(int index)
        {
            GameObject Range = Instantiate(attackRangePrefab[0], patternOnePoints[index].position + new Vector3(0, 0.2f, 0), Quaternion.identity);
            Range.GetComponent<DemonAttackRange>().StartGrowing(attackRangeScale[0], rangeSize[0]);
            Destroy(Range, 2f);
            yield return new WaitForSeconds(2);
        }
        #endregion
        //패턴 2
        #region Pattern 2
        public void AttackBall()
        {
            StartCoroutine(AttackRangeBall());
        }
        IEnumerator AttackRangeBall()
        {
            GameObject Range = Instantiate(attackRangePrefab[0], rangeTransform.position, Quaternion.identity);
            Range.GetComponent<DemonAttackRange>().StartGrowing(attackRangeScale[1], rangeSize[1]);
            Destroy(Range, 0.8f);
            yield return new WaitForSeconds(0.8f);
            GameObject attackball = Instantiate(ballInstantiate, ballTransfrom.position, Quaternion.identity);
            GameObject effgo = Instantiate(effect[1], attackball.transform.position, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);

            AudioUtility.CreateSFX(audioManager.SetAudioClip(0), transform.position, audioManager.SetGroups(0));
            if (effgo != null && attackball != null)
            {
                Destroy(attackball, 1f);
                Destroy(effgo, 2f);
            }
            demon.lastAttackTime[1] = Time.time;
        }
        #endregion
        //패턴 3
        #region Pattern 3
        public void PerformTeleport()
        {
            if (teleportPoints.Length > 1) // 텔레포트 지점이 2개 이상일 때 중복 방지 가능
            {
                Transform closestPoint = null;
                float closestDistance = Mathf.Infinity;

                // 모든 텔레포트 지점을 순회하며 가장 가까운 지점 찾기
                foreach (Transform point in teleportPoints)
                {
                    float distance = Vector3.Distance(player.position, point.position);

                    // 플레이어와의 거리 비교 및 현재 위치와 중복 방지
                    if (distance < closestDistance && point.position != transform.position)
                    {
                        closestDistance = distance;
                        closestPoint = point;
                    }
                }

                if (closestPoint != null)
                {
                    transform.position = closestPoint.position; // 가장 가까운 위치로 텔레포트
                    transform.LookAt(player.position);
                    // 텔레포트 효과 생성
                    GameObject effectgo = Instantiate(effect[2], transform.position, Quaternion.identity);
                    AudioUtility.CreateSFX(audioManager.SetAudioClip(1), transform.position, audioManager.SetGroups(0));
                    Destroy(effectgo, 1f);
                }
            }
        }
        public void ShootAttack()
        {
            StartCoroutine(SoundShoot());
        }
        IEnumerator SoundShoot()
        {
            AudioUtility.CreateSFX(audioManager.SetAudioClip(2), transform.position, audioManager.SetGroups(2));
            yield return new WaitForSeconds(2.5f);
            demon.lastAttackTime[2] = Time.time;
        }
        #endregion
        //근거리 공격
        public void CloseAttack()
        {
            transform.LookAt(player.position);
        }
        public void Warning()
        {
            StartCoroutine(WarningWindow());
        }
        IEnumerator WarningWindow()
        {
            Warningeffect.SetActive(true);
            AudioUtility.CreateSFX(audioManager.SetAudioClip(5), transform.position, audioManager.SetGroups(5));
            yield return new WaitForSeconds(1);
            Warningeffect.SetActive(false);
        }
    }
}
