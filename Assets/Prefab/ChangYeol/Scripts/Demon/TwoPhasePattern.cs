using BS.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Demon
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class PatternPoins
    {
        public Transform[] patternTwoPoints;
    }
    public class TwoPhasePattern : MonoBehaviour
    {
        #region Variables
        private DemonPattern pattern;

        //패턴 4
        public List<PatternPoins> patternTwoPoints;
        public float spawnTime = 1f;
        public GameObject[] effect;
        public GameObject effectdot;

        public Transform balltransform;
        #endregion
        private void Start()
        {
            pattern = GetComponent<DemonPattern>();
        }
        //2페이지 패턴 1
        #region 2Phase Pattern 1
        public void SpawnAndExplodeInOrder()
        {
            if (patternTwoPoints.Count == 0 || !pattern.ball[1])
            {
                Debug.LogWarning("Spawn points or object to spawn not set!");
                return;
            }
            StartCoroutine(SpawnAndExplodeRoutine());
            pattern.demon.lastPesosTime[0] = Time.time;
        }

        private IEnumerator SpawnAndExplodeRoutine()
        {
            foreach (PatternPoins points in patternTwoPoints)
            {
                foreach (Transform transform in points.patternTwoPoints)
                {
                    GameObject Range = Instantiate(pattern.attackRangePrefab[0], transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                    Range.GetComponent<DemonAttackRange>().StartGrowing(pattern.attackRangeScale[0], pattern.rangeSize[0]);
                    Destroy(Range, 1f);

                    // 지정된 위치에 오브젝트 생성
                    GameObject spawnedBall = Instantiate(pattern.ball[1].gameObject, transform.position, Quaternion.identity);
                    spawnedBall.GetComponent<BallRise>().StartRise();
                }
                yield return new WaitForSeconds(spawnTime);
            }
        }
        #endregion
        #region 2Phase Pattern 2
        public void AttackBallDot()
        {
            GameObject attackball = Instantiate(pattern.ballInstantiate, pattern.ballTransfrom.position, Quaternion.identity);
            GameObject effgo = Instantiate(effect[1], attackball.transform.position, Quaternion.identity);
            Destroy(attackball, 1f);
            Destroy(effgo, 1f);
            Vector3 Explosionpos = new Vector3(effgo.transform.position.x, attackball.transform.position.y + -1.9f, effgo.transform.position.z);
            GameObject Explosion = Instantiate(effect[2], Explosionpos, Quaternion.identity);
            AudioUtility.CreateSFX(pattern.audioManager.SetAudioClip(3), Explosionpos, pattern.audioManager.SetGroups(3));
            GameObject dot = Instantiate(effectdot, Explosionpos, effectdot.transform.rotation);
            Destroy(Explosion, 1f);
            StartCoroutine(EffectDot(dot));
            pattern.demon.lastPesosTime[1] = Time.time;
        }
        public void DelayRange()
        {
            StartCoroutine(AttackRangeBall());
        }
        IEnumerator AttackRangeBall()
        {
            GameObject Range = Instantiate(pattern.attackRangePrefab[0], balltransform.position, Quaternion.identity);
            Range.GetComponent<DemonAttackRange>().StartGrowing(pattern.attackRangeScale[1], pattern.rangeSize[1]);
            yield return new WaitForSeconds(1);
            Destroy(Range);
        }
        IEnumerator EffectDot(GameObject effdot)
        {
            if (effdot != null)
            {
                yield return new WaitForSeconds(13);
                Destroy(effdot);
            }
        }
        #endregion
        #region 2Phase Pattern 3
        public void PerformStunTeleport()
        {
            if (pattern.teleportPoints.Length > 1) // 텔레포트 지점이 2개 이상일 때 중복 방지 가능
            {
                Transform closestPoint = null;
                float closestDistance = Mathf.Infinity;

                // 모든 텔레포트 지점을 순회하며 가장 가까운 지점 찾기
                foreach (Transform point in pattern.teleportPoints)
                {
                    float distance = Vector3.Distance(pattern.player.position, point.position);

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
                    transform.LookAt(pattern.player.position);
                    // 텔레포트 효과 생성
                    GameObject effectgo = Instantiate(effect[3], transform.position, Quaternion.identity);
                    AudioUtility.CreateSFX(pattern.audioManager.SetAudioClip(1), transform.position, pattern.audioManager.SetGroups(1));
                    //GameObject trigger = Instantiate(effect[4], transform.position, Quaternion.identity);
                    Destroy(effectgo, 1f);
                    pattern.demon.lastPesosTime[2] = Time.time;
                }
            }
        }
        #endregion
    }
}