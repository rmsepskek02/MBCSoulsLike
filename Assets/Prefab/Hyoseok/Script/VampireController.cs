using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using BS.Player;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using BS.Particle;
using BS.Utility;

namespace BS.vampire
{
    /// <summary>
    /// 공격상태머신
    /// </summary>
    public class VampireController : MonoBehaviour
    {
        #region Variables

        public Animator animator;
        public GameObject player;   //바닥
        public GameObject playerBody;   //플레이어몸체 
        public float testAttackNumber;
        public GameObject waringSquarePrefab;  //경고창
        private int direction;  // 방향

        public float time = 20f; // 공격 대기 시간

        //패턴쿨타임
        [SerializeField] float attack1;
        [SerializeField] float attack2;
        [SerializeField] float attack3;
        [SerializeField] float attack4;

        [Header("Audio")]
        public AudioClip teleportSound;
        public AudioClip shotSound;
        public AudioClip rushSound;
        public AudioClip warningSound;
        public AudioClip laserSound;
        public AudioClip lightningSound;
        public AudioClip explosionSound;
        public AudioClip castingSound;
        public AudioClip meteoSound;



        [Header("Teleport")]
        public GameObject pingpongShot;
        public GameObject CircleShot;
        public GameObject teleportEffect;
        public Transform[] teleports; //순간이동 위치 0~3 랜덤 4는 중앙
        public float teleportTime = 20; //순간이동 쿨타임
        private int previousIndex = -1; //이전 위치값

        [Header("Attack1")]
        // 공격1 
        // 두 개의 소환몹이 곡선으로 플레이어 방향으로 이동해서 타격
        public GameObject attackObjectPrefab; // 공격 관리 오브젝트
        public GameObject impactEffectPrefab; // 이펙트 프리팹 batEffect에smoke에 공격감지 넣어둠
        [SerializeField] private float attackRange;      //공격범위
        [SerializeField] private float attackCount;     //공격횟수
        public GameObject attackRangePrefab; // 공격범위 프리팹
        public Transform[] attackObjects;
        public Transform hitPoint; // 타격 지점
        [Header("Attack2")]
        //공격2
        //보스가 중앙으로 이동후 공격모션 이펙트 발동후 왼 오 아래 위 레이저 발사 
        public Transform centerTeleport;    //센터 텔레포트
        //public GameObject teleportEffect;
        public GameObject attack2EffectPrefab; //시전 이펙트프리팹 
        public GameObject[] bloodBeams;   //왼오아래위 빔들
        public GameObject[] attak3Ranges;
        [Header("Attack3")]
        //공격3
        //보스가 플레이어를 바라보며 부채꼴 박쥐웨이브 날리기
        public GameObject Attack3BatPrefab; //부딫히면 플레이어에게 데미지주는 박쥐
        public float waveCount = 3f;     //공격 웨이브 카운트
        public GameObject attack3EffectPrefab;  //부딫혔을때 파티클
        public Transform[] batTransforms;

        [Header("Attack4")]
        //공격4
        //보스 주변에 박쥐들(로테이션들이) 랜덤 1~2 방향으로 산개후 플레이어방향으로 레이져 발사

        public GameObject[] summonObject;    //소환위치
        public GameObject attak4Ranges;
        public float moveRadius = 2f; // 이동 반경
        public GameObject attack4EffectPrefab;  //레이저 이펙트
        private Vector3[] originalPositions; // 원래위치
        [SerializeField] float attack4count = 3f;  //반복횟수

        [Header("Attack5")]
        //공격5
        //보스가 위 오른쪽에 레이저이동박쥐 소환 
        public Transform[] attack5Lotations;
        public GameObject attack5BatPrefab;
        public GameObject attack5SummonEffect;
        private bool isAttack5BatSummon = false;  //배트소환 여부

        [Header("Attack6")]
        //공격 6 즉사기
        //보스가 공중날기 이후 기모아서 메테오 시전
        // 공중날기하는 중에 위험구역과 안전지대 생성
        // 안전 지대에 있을 시 플레이어 무적 , 그 밖에 즉사 
        public GameObject attack6Range;
        public GameObject[] safeRanges;
        public GameObject attack6BossEffect;
        public GameObject meteorPrefab;
        [Header("Attack7")]
        //공격 7 돌진기 
        //보스가 플레이어 방향으로 돌진 앞에 콜라이더생성
        public GameObject head;
        public GameObject attack7Range;
        public GameObject attack7collider;
        public GameObject attack7Effect;


        private int nextPattern = 0;
        #endregion
        void Start()
        {
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            //Waring();
            //StartCoroutine(Attack6());

            NextPatternPlay();


        }

        //private void Update()
        //{
        //    transform.LookAt(player.transform);
        //}
        IEnumerator RandomTeleport()
        {

           

            // 애니메이션 연출 3초 후에 이동
            //animator.SetTrigger("Teleport");
            GameObject potalEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
            potalEffect.transform.parent = transform;
            Destroy(potalEffect, 3.3f);
            yield return new WaitForSeconds(3.3f);
            AudioUtility.CreateSFX(teleportSound, transform.position, AudioUtility.AudioGroups.Sound);
        
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, 3);
            }
            while (randomIndex == previousIndex); // 같은 값 연속 방지
                                                  // 보스 위치를 랜덤 이동
            transform.position = teleports[randomIndex].position;
            previousIndex = randomIndex;

            // 플레이어 바라보며 걷기
            transform.LookAt(player.transform.position);
            animator.SetTrigger("Walk");
            yield return new WaitForSeconds(0.5f);
            AudioUtility.CreateSFX(shotSound, transform.position, AudioUtility.AudioGroups.Sound);
            Waring();
            float walkDuration = 4f;
            float elapsedTime = 0f;

            Vector3 startPos = transform.position;
            Vector3 endPos = transform.position + new Vector3(transform.forward.x, 0, transform.forward.z) * 5f;

            // 걷는 동안 탄막 발사
            // 생성으로 교체
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            GameObject tan = Instantiate(pingpongShot, spawnPosition, pingpongShot.transform.rotation);
            Destroy(tan, 10f);

            while (elapsedTime < walkDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / walkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }

        //배트자폭
        IEnumerator Attack1()
        {
       
            transform.LookAt(player.transform);
            yield return new WaitForSeconds(1f);
            Waring();
            animator.SetTrigger("Attack1");
            for (int i = 0; i < attackCount; i++)
            {
                // 공격 모션 시간
                GameObject attackObject1 = Instantiate(attackObjectPrefab, attackObjects[0].position, Quaternion.identity);
                GameObject attackObject2 = Instantiate(attackObjectPrefab, attackObjects[1].position, Quaternion.identity);
               

                hitPoint = playerBody.transform;

                yield return new WaitForSeconds(0.5f);

                Debug.Log("Attack1 start");

                // 이동 경로 (y 값을 0으로 설정)
                Vector3[] path1 = {
            new Vector3(attackObject1.transform.position.x, 0, attackObject1.transform.position.z),
            new Vector3((attackObject1.transform.position.x + hitPoint.position.x) / 2, 0, hitPoint.position.z + 2),
            new Vector3(hitPoint.position.x, 0, hitPoint.position.z)
        };

                Vector3[] path2 = {
            new Vector3(attackObject2.transform.position.x, 0, attackObject2.transform.position.z),
            new Vector3((attackObject2.transform.position.x + hitPoint.position.x) / 2, 0, hitPoint.position.z - 2),
            new Vector3(hitPoint.position.x, 0, hitPoint.position.z)
        };

                // 곡선 이동 및 플레이어 바라보게 설정
                attackObject1.transform.DOPath(path1, 0.5f, PathType.CatmullRom).SetOptions(false, AxisConstraint.Y).SetEase(Ease.InOutSine).OnUpdate(() =>
                {
                    attackObject1.transform.LookAt(player.transform);
                }).OnComplete(() =>
                {
                    AudioUtility.CreateSFX(rushSound, transform.position, AudioUtility.AudioGroups.Sound); AudioUtility.CreateSFX(explosionSound, transform.position, AudioUtility.AudioGroups.Sound);
                   
                     // 타격 지점에 도달 시 터트리고 제거
                     GameObject impactEffect = Instantiate(impactEffectPrefab, attackObject1.transform.position, Quaternion.identity);
                    Destroy(attackObject1, 1f);
                    Destroy(impactEffect, 1.5f);
                });

                attackObject2.transform.DOPath(path2, 0.5f, PathType.CatmullRom).SetOptions(false, AxisConstraint.Y).SetEase(Ease.InOutSine).OnUpdate(() =>
                {
                    attackObject2.transform.LookAt(player.transform);
                }).OnComplete(() =>
                {
                    // 타격 지점에 도달 시 터트리고 제거
                    GameObject impactEffect = Instantiate(impactEffectPrefab, attackObject2.transform.position, Quaternion.identity);
                    Destroy(attackObject2, 1f);
                  
                    Destroy(impactEffect, 1.5f);
                    Invoke("PlayExplosionSound",1f);
                });
            
                yield return new WaitForSeconds(0.1f);
            }
           
            yield return null;
        }
        //폭발할때 사운드 키기 invoke사용
        void PlayExplosionSound()
        {
            AudioUtility.CreateSFX(explosionSound, transform.position, AudioUtility.AudioGroups.Sound);
        }

        //상하좌우 레이져 패턴
        IEnumerator Attack2()
        {

            //보스 중앙이동
            //animator.SetTrigger("Teleport");
            GameObject potalEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
            potalEffect.transform.parent = transform;
            Destroy(potalEffect, 3.3f);
            AudioUtility.CreateSFX(teleportSound, transform.position, AudioUtility.AudioGroups.Sound);
            yield return new WaitForSeconds(2.5f);
            transform.position = centerTeleport.position;
            transform.LookAt(player.transform);
            Waring();
            //보스 스킬 연출
            GameObject skillEffectGo = Instantiate(attack2EffectPrefab, transform.position, Quaternion.identity);
            Destroy(skillEffectGo, 2f);
            //애니메이션
            animator.SetTrigger("Attack2");
            yield return new WaitForSeconds(1f);    //동작대기시간
            //이펙트

            for (int i = 0; i < attak3Ranges.Length; i++)
            {
                //스킬 레이 그려야됌 일직선 빨간색 범위
                attak3Ranges[i].SetActive(true);
                yield return new WaitForSeconds(1f);
                attak3Ranges[i].SetActive(false);
            }

            for (int i = 0; i < bloodBeams.Length; i++)
            {
                AudioUtility.CreateSFX(laserSound, transform.position, AudioUtility.AudioGroups.Sound);
                //레이보여주는시간
                bloodBeams[i].SetActive(true);
                yield return new WaitForSeconds(1f);
                bloodBeams[i].SetActive(false);
            }

         
            yield return null;

        }

        //배트날리기
        IEnumerator Attack3()
        {
            yield return new WaitForSeconds(4f);
            transform.LookAt(player.transform);
            Waring();
            animator.SetTrigger("Attack1");
            //웨이브
            for (int i = 0; i < waveCount; i++)
            {
                AudioUtility.CreateSFX(shotSound, transform.position, AudioUtility.AudioGroups.Sound);
                foreach (Transform batTransform in batTransforms)
                {
                    GameObject bat = Instantiate(Attack3BatPrefab, batTransform.position, batTransform.rotation);
                    Rigidbody rb = bat.GetComponent<Rigidbody>();

                   
                    Vector3 directionToPlayer = (player.transform.position - batTransform.position).normalized;
                    directionToPlayer.y = 0;
                    //directionToPlayer.z = 0;
                    Attack3Bat attack3Bat = bat.GetComponent<Attack3Bat>();
                    attack3Bat.Initialize(directionToPlayer, 20f);

                    Destroy(bat, 7f);

                    //충돌시 이펙트 



                }
                yield return new WaitForSeconds(0.3f);
            }
        
            yield return null;
        }
        //소환된 배트가 플레이어타격 레이져발사
        IEnumerator Attack4()
        {
            yield return new WaitForSeconds(5f);

            transform.LookAt(player.transform);

            for (int j = 0; j < attack4count; j++)
            {
                Waring();
                // 공격4용 위치 저장
                originalPositions = new Vector3[summonObject.Length];
                for (int i = 0; i < summonObject.Length; i++)
                {
                    originalPositions[i] = summonObject[i].transform.position;
                }

                // 위치 이동
                float elapsedTime = 0f;
                float moveDuration = 0.2f; // 이동에 걸릴 시간
                animator.SetTrigger("Attack1");
                Vector3[] targetPositions = new Vector3[summonObject.Length];
                for (int i = 0; i < summonObject.Length; i++)
                {
                    targetPositions[i] = originalPositions[i] + new Vector3(Random.Range(-moveRadius, moveRadius), 0, Random.Range(-moveRadius, moveRadius));
                }

                while (elapsedTime < moveDuration)
                {
                    for (int i = 0; i < summonObject.Length; i++)
                    {
                        if (summonObject[i] != null)
                        {
                            Vector3 newPosition = Vector3.Lerp(originalPositions[i], targetPositions[i], elapsedTime / moveDuration);
                            newPosition.y = originalPositions[i].y; // y값 고정
                            summonObject[i].transform.position = newPosition;

                            // 플레이어를 바라보도록 회전한 후, 랜덤한 값을 더하기
                            summonObject[i].transform.LookAt(playerBody.transform);

                            Vector3 eulerAngles = summonObject[i].transform.rotation.eulerAngles;
                            eulerAngles.y += Random.Range(-10f, 10f);
                            summonObject[i].transform.rotation = Quaternion.Euler(eulerAngles);
                        }
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                for (int i = 0; i < summonObject.Length; i++)
                {
                    if (summonObject[i] != null)
                    {
                        summonObject[i].transform.position = targetPositions[i];
                    }
                }

                //공격범위
                for (int i = 0; i < summonObject.Length; i++)
                {
                    GameObject attakRange = Instantiate(attak4Ranges, summonObject[i].transform.position, summonObject[i].transform.rotation);
                    attakRange.transform.parent = summonObject[i].transform;
                    Destroy(attakRange, 0.5f);
                }

                yield return new WaitForSeconds(0.5f);

                // 레이저 발사
                for (int i = 0; i < summonObject.Length; i++)
                {
                    AudioUtility.CreateSFX(laserSound, transform.position, AudioUtility.AudioGroups.Sound);

                    // 레이저 발사 위치의 y값을 타겟 위치의 y값과 동일하게 설정
                    Vector3 laserPosition = new Vector3(summonObject[i].transform.position.x, targetPositions[i].y, summonObject[i].transform.position.z);

                    GameObject attackEffect = Instantiate(attack4EffectPrefab, laserPosition, summonObject[i].transform.rotation);
                    attackEffect.transform.parent = summonObject[i].transform;
                    attackEffect.transform.rotation *= Quaternion.Euler(90f, 0f, 0f);
                    Destroy(attackEffect, 0.5f);
                }

                yield return new WaitForSeconds(1f);

                // 다시 원래 위치로
                elapsedTime = 0f;
                while (elapsedTime < moveDuration)
                {
                    for (int i = 0; i < summonObject.Length; i++)
                    {
                        if (summonObject[i] != null)
                        {
                            summonObject[i].transform.position = Vector3.Lerp(targetPositions[i], originalPositions[i], elapsedTime / moveDuration);
                        }
                    }
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                for (int i = 0; i < summonObject.Length; i++)
                {
                    if (summonObject[i] != null)
                    {
                        summonObject[i].transform.position = originalPositions[i];
                    }
                }
            }
            yield return new WaitForSeconds(3f);

            yield return null;
        }

        //레이저 배트소환
        IEnumerator Attack5()
        {
            Waring();
            if (isAttack5BatSummon)
            {
                
                yield break;
            }
            transform.LookAt(player.transform);
            isAttack5BatSummon = true;
            animator.SetTrigger("Attack1");
            AudioUtility.CreateSFX(lightningSound, transform.position, AudioUtility.AudioGroups.Sound);
            GameObject summonEffect = Instantiate(attack5SummonEffect, transform.position, Quaternion.identity);
            summonEffect.transform.parent = transform;
            Destroy(summonEffect, 3f);
            yield return new WaitForSeconds(2f);
            // 공격 모션 시간
            GameObject attackObject1 = Instantiate(attack5BatPrefab, attack5Lotations[0].position, attack5Lotations[0].rotation);
            GameObject attackObject2 = Instantiate(attack5BatPrefab, attack5Lotations[1].position, attack5Lotations[1].rotation);


            // 첫 번째 오브젝트 (z축으로 이동)
            Attack5BatMove firstObject = attackObject1.GetComponent<Attack5BatMove>();
            firstObject.moveInZAxis = true;

            // 두 번째 오브젝트 (x축으로 이동)
            Attack5BatMove secondObject = attackObject2.GetComponent<Attack5BatMove>();
            secondObject.moveInZAxis = false;


         
            yield return null;
        }

        //공격 6 즉사기

        IEnumerator Attack6()
        {
            transform.position = centerTeleport.position;
            transform.LookAt(player.transform);
            Waring();
            // 공중 날기 애니메이션
            animator.SetBool("IsFly", true);
            float flyHeight = 4f; // 높이
            float flyDuration = 4f; // 걸리는 시간
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + new Vector3(0, flyHeight, 0);
            float elapsedTime = 0f;
            AudioUtility.CreateSFX(castingSound, transform.position, AudioUtility.AudioGroups.Sound);
            // 보스를 느린 속도로 위로 이동
            while (elapsedTime < flyDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flyDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(2f);
            Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z);

            // 마법진 이펙트
       
            GameObject bossEffectGo = Instantiate(attack6BossEffect, spawnPosition, attack6BossEffect.transform.rotation);
            bossEffectGo.transform.parent = transform;
            Destroy(bossEffectGo, 14f);
            // 애니메이션 시전 시간
            yield return new WaitForSeconds(5f);
            // 안전지대 위험구역 생성
            attack6Range.SetActive(true);

            int randomIndex;
            int safeIndex;
            randomIndex = Random.Range(0, 3);
            safeIndex = randomIndex;
            safeRanges[safeIndex].SetActive(true);


            // 기모으기 4초
            yield return new WaitForSeconds(4f);
            // 메테오 시전
            AudioUtility.CreateSFX(meteoSound, transform.position, AudioUtility.AudioGroups.Sound);
            GameObject Meteors = Instantiate(meteorPrefab, meteorPrefab.transform.position, meteorPrefab.transform.rotation);
            Destroy(Meteors, 5f);

            // 플레이어 위치 확인 및 데미지 적용
            DangerZone dangerZone = attack6Range.GetComponent<DangerZone>();
            dangerZone.enabled = true;

            // 6초 이후 안전지대 위험구역 끄기, 보스 내려오기
            yield return new WaitForSeconds(6f);

            attack6Range.SetActive(false);
            yield return new WaitForSeconds(0.5f);

            safeRanges[safeIndex].SetActive(false);

            // 보스가 다시 내려오게 하기
            startPos = transform.position;
            endPos = startPos - new Vector3(0, flyHeight, 0);
            elapsedTime = 0f;

            while (elapsedTime < flyDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / flyDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            animator.SetBool("IsFly", false);

          
            yield return null;
        }
        IEnumerator Attack7()
        {
            // 벽 레이어 가져오기
            int wallLayerMask = LayerMask.GetMask("Wall");
            // 공격 이펙트 생성
            attack7Effect.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                Waring();
                transform.LookAt(player.transform);
       
                // 공중 날기 - 날기 애니메이션
                animator.SetBool("IsFlying", true);

             

                // Range 활성화
                Vector3 dashDirection = (player.transform.position - transform.position).normalized;
                attack7Range.SetActive(true);

                yield return new WaitForSeconds(0.5f);

                attack7collider.SetActive(true);

                // 돌진 시작
                Vector3 startPos = transform.position;
                float dashDistance = 35f; // 돌진 거리 증가
                float elapsedTime = 0f;
                float flyDuration = 0.75f; // 돌진 지속 시간 단축 (빠르게 돌진)

                Vector3 targetPosition = startPos + dashDirection * dashDistance;
                float safeDistance = 3.0f; // 벽과의 안전 거리

                // 벽 감지를 위한 변수 추가
                bool hitWall = false;
                AudioUtility.CreateSFX(rushSound, transform.position, AudioUtility.AudioGroups.Sound);
                while (elapsedTime < flyDuration)
                {
              
                    float remainingDistance = Vector3.Distance(transform.position, targetPosition);
                    Debug.DrawRay(transform.position, dashDirection * remainingDistance, Color.red, 1f);

                    // 벽과의 충돌 감지 (레이어 기반)
                    if (Physics.Raycast(transform.position, dashDirection, out RaycastHit hit, remainingDistance, wallLayerMask))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
                        {
                            // 벽과 충돌을 감지한 경우, 안전한 위치로 목표를 변경
                            targetPosition = hit.point - dashDirection * safeDistance;
                            hitWall = true;
                        }
                    }

                    // 벽과 충돌하지 않았거나, 조정된 목표 위치로 계속 이동
                    transform.position = Vector3.Lerp(startPos, targetPosition, elapsedTime / flyDuration);
                    elapsedTime += Time.deltaTime;

                    // 벽과 충돌한 경우 더 이상 진행하지 않고 종료
                    if (hitWall && Vector3.Distance(transform.position, targetPosition) <= 0.1f)
                        break;

                    yield return null;
                }

                // 착륙 애니메이션
                animator.SetBool("IsFlying", false);
               
                attack7Range.SetActive(false);

                // 돌진 종료 
                attack7collider.SetActive(false);
                yield return new WaitForSeconds(0.5f);
            }
            attack7Effect.SetActive(false);
        }



        void Waring()
        {
            AudioUtility.CreateSFX(warningSound, transform.position, AudioUtility.AudioGroups.Sound);
            GameObject waring = Instantiate(waringSquarePrefab, waringSquarePrefab.transform.position,Quaternion.identity);
            waring.transform.SetParent(transform,false);
            Destroy(waring, 3f);
        }



        void NextPatternPlay()
        {


            // 패턴을 순환 (1~6)
            nextPattern = (nextPattern % 8) + 1;

            // 플레이어를 바라보게 설정
            transform.LookAt(player.transform);

            // 패턴 실행
            StartCoroutine(ExecutePattern(nextPattern));
        }

        IEnumerator ExecutePattern(int pattern)
        {
            Debug.Log($"{pattern}번 패턴 실행");

            //// 패턴에 따라 추가 행동 실행
            //if (pattern == 1 || pattern == 2 || pattern == 5)
            //    yield return StartCoroutine(RandomTeleport());

            //if (pattern == 3 || pattern == 5)
            //    yield return StartCoroutine(Attack7()); // 돌진 공격 실행

            // 패턴 실행
            switch (pattern)
            {
                case 2: yield return StartCoroutine(RandomTeleport()); break;
                case 1: yield return StartCoroutine(Attack7()); break;
                case 3: yield return StartCoroutine(Attack4()); break;
                case 4: yield return StartCoroutine(Attack2()); break;
                case 5: yield return StartCoroutine(Attack3()); break;
                case 6: yield return StartCoroutine(Attack1()); break;
                case 7: yield return StartCoroutine(Attack5()); break;
                case 8: yield return StartCoroutine(Attack6()); break;
                //case 8: yield return StartCoroutine(Attack1()); break;
                //case 9: yield return StartCoroutine(Attack6()); break;
            }
            NextPatternPlay();
        }
    }
}
