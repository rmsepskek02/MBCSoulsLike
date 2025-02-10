using System.Collections;
using BS.Player;
using BS.Utility;
using UnityEngine;
using UnityEngine.Audio;
using static BS.Utility.AudioUtility;

public class AlienBossPattern : MonoBehaviour
{
    public Animator animator;              // 보스 애니메이터

    public GameObject pt1Particle;         // 발사체 프리팹
    public Transform pt1SpawnPoint;        // 발사체 생성 위치
    public GameObject pt1AttackRange;      // 공격범위 프리팹
    public AudioClip pt1Sound;             // 패턴 효과음

    public GameObject pt2Particle;         // 발사체 프리팹
    public Transform pt2SpawnPoint;        // 발사체 생성 위치
    public GameObject pt2AttackRange;      // 공격범위 프리팹
    public AudioClip pt2Sound;             // 패턴 효과음

    public GameObject pt3Particle;         // 이펙트 프리팹
    public Transform pt3SpawnPoint;        // 이펙트 생성 위치
    public GameObject pt3AttackRange;      // 공격범위 프리팹
    public AudioClip pt3Sound;             // 패턴 효과음

    public GameObject pt4Particle;         // 이펙트 프리팹
    public Transform pt4SpawnPoint;        // 이펙트 생성 위치
    public GameObject pt4AttackRange;      // 공격범위 프리팹
    public AudioClip pt4Sound;             // 패턴 효과음

    public GameObject pt5Particle;         // 이펙트 프리팹
    public Transform pt5SpawnPoint;        // 이펙트 생성 위치
    public GameObject pt5AttackRange;      // 공격범위 프리팹
    public float pt5radius = 5f;           // 공격범위 반지름
    public int pt5spawnCount = 5;          // 공격 횟수
    //public AudioClip pt5Sound;             // 패턴 효과음

    public GameObject alien;               // 보스
    public GameObject player;              // 플레이어
    private float distance;                // 보스와 플레이어의 거리

    private bool canAttack = true;
    private bool canMeleeAttack = true;
    private bool canRangedAttack = true;
    private int randomNumber;

    private int previousPattern;           // 같은 패턴 방지용

    private AudioSource audioSource;
    public AlienHealth alienHealth;

    //public AlienSound alienSound;
    //public AudioUtility audioUtility;     // 오디오

    int[] patternNumbers = new int[] { 1, 2, 3, 4 };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        alienHealth = GetComponent<AlienHealth>();
        audioSource = GetComponent<AudioSource>();
        randomNumber = Random.Range(1, 5);  // 1 ~ 4중 랜덤한 숫자
        previousPattern = 0;
        Debug.Log($"첫 패턴 : {randomNumber}");
    }

    // Update is called once per frame
    void Update()
    {
        if (alienHealth.isDie) 
        {
            StopAllCoroutines();
            return;
        }
        // 플레이어와 거리재기
        distance = Vector3.Distance(alien.transform.position, player.transform.position);

        // 플레이어와의 거리가 8 미만일때
        if (distance < 8 && canMeleeAttack)
        {
            canMeleeAttack = false;
            StartCoroutine(Pattern4());
            //Debug.Log($"근접공격 : {canMeleeAttack}");
        }

        if (canAttack)
        {
            
            if (randomNumber == 1 && canRangedAttack)
            {
                canAttack = false;                  // 매 프레임마다 실행되는거 막기용 bool형 변수
                canRangedAttack = false;            // 매 프레임마다 실행되는거 막기용 bool형 변수
                StartCoroutine(Pattern1());

                // 다음 패턴 재설정
                do
                {
                    randomNumber = Random.Range(1, 5);
                } while (randomNumber == previousPattern);

                previousPattern = randomNumber; // 이전 패턴 업데이트
                Debug.Log($"다음패턴 : {randomNumber}");
                //Debug.Log($"원거리공격 : {canRangedAttack}");
            }
            else if (randomNumber == 2 && canRangedAttack)
            {
                canAttack = false;
                canRangedAttack = false;
                StartCoroutine(Pattern2());

                // 다음 패턴 재설정
                do
                {
                    randomNumber = Random.Range(1, 5);
                } while (randomNumber == previousPattern);

                previousPattern = randomNumber; // 이전 패턴 업데이트
                Debug.Log($"다음패턴 : {randomNumber}");
                //Debug.Log($"원거리공격 : {canRangedAttack}");
            }
            else if (randomNumber == 3 && canRangedAttack)
            {
                canAttack = false;
                canRangedAttack = false;
                StartCoroutine(Pattern3());

                // 다음 패턴 재설정
                do
                {
                    randomNumber = Random.Range(1, 5);
                } while (randomNumber == previousPattern);

                previousPattern = randomNumber; // 이전 패턴 업데이트
                Debug.Log($"다음패턴 : {randomNumber}");
                //Debug.Log($"원거리공격 : {canRangedAttack}");
            }
            else if (randomNumber == 4 && canRangedAttack)
            {
                canAttack = false;
                canRangedAttack = false;
                StartCoroutine(Pattern5());

                // 다음 패턴 재설정
                do
                {
                    randomNumber = Random.Range(1, 5);
                } while (randomNumber == previousPattern);

                previousPattern = randomNumber; // 이전 패턴 업데이트
                Debug.Log($"다음패턴 : {randomNumber}");
                //Debug.Log($"원거리공격 : {canRangedAttack}");
            }
        }
        /*
        // 테스트용
        if (Input.GetKeyDown(KeyCode.Q))        // 공격패턴1
        {
            StartCoroutine(Pattern1());
        }
        else if (Input.GetKeyDown(KeyCode.W))   // 공격패턴2
        {
            StartCoroutine(Pattern2());
        }
        else if (Input.GetKeyDown(KeyCode.E))   // 공격패턴3
        {
            StartCoroutine(Pattern3());
        }
        else if (Input.GetKeyDown(KeyCode.R))   // 공격패턴4
        {
            StartCoroutine(Pattern4());
        }
        else if (Input.GetKeyDown(KeyCode.T))   // 공격패턴5
        {
            StartCoroutine(Pattern5());
        }*/
    }

    // 공격패턴1 : 부채꼴 공격
    public IEnumerator Pattern1()
    {
        canAttack = false;

        // 플레이어 방향 계산
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;  // 플레이어 방향으로 벡터 계산
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);  // 플레이어를 바라보는 회전값 생성

        // 공격범위 표시 (플레이어 방향을 바라보게 회전, 기존 회전 값 추가)
        Quaternion attackRangeRotation = lookRotation * Quaternion.Euler(0f, 67.5f, 180f);  // 기존 회전 값 추가

        // pt1AttackRange를 플레이어 방향으로 회전시키고, 기존 회전 값 추가
        Instantiate(pt1AttackRange, new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z + -5.0f), attackRangeRotation);

        yield return new WaitForSeconds(1.3f);

        animator.SetInteger("Pattern", 1); // 애니메이터 "Pattern" 값을 1로 설정

        // 사운드
        // AudioUtility.CreateSFX(alienSound.alienSound1, transform.position, AudioGroups.SKILL, 1f, 1f, 15f);
        audioSource.resource = pt1Sound;
        audioSource.Play();

        StartCoroutine(PatternReset(0.5f)); // 일정시간 후 "Pattern" 값을 0으로 초기화

        // pt1Particle도 플레이어를 바라보게 하고, 같은 방향으로 회전
        GameObject particle = Instantiate(pt1Particle, pt1SpawnPoint.position, lookRotation);

        yield return new WaitForSeconds(5.0f); // 다음 패턴을 위한 대기시간

        canAttack = true;
        canRangedAttack = true;
    }



    // 공격패턴2 : 맵 전체공격(원형)
    public IEnumerator Pattern2()
    {
        // 공격범위 표시
        Instantiate(pt2AttackRange, transform.position, Quaternion.Euler(0f, 0f, 0f));

        yield return new WaitForSeconds(2.0f);

        animator.SetInteger("Pattern", 2); // 애니메이터 "Pattern" 값을 2로 설정

        yield return new WaitForSeconds(1.1f);

        animator.SetInteger("Pattern", 0); // 애니메이터 "Pattern" 값을 0으로 초기화

        // 사운드
        //alienSound.audioSource.PlayOneShot(alienSound.alienSound1);
        //alienSound.audioSource.clip = alienSound.alienSound2;
        //alienSound.audioSource.Play();
        //audioSource.resource = pt2Sound;
        //audioSource.Play();
        AudioUtility.CreateSFX(pt2Sound, player.transform.position, AudioGroups.Skill, 0f);


        Instantiate(pt2Particle, pt2SpawnPoint.position, Quaternion.Euler(0f, 0f, 0f));

        yield return new WaitForSeconds(2.0f); // 다음 패턴을 위한 대기시간

        canAttack = true;
        canRangedAttack = true;
    }

    // 공격패턴3 : 맵 전체공격(파도형)
    public IEnumerator Pattern3()
    {
        // 공격범위 표시
        StartCoroutine(pt3AttackRanges());

        yield return new WaitForSeconds(1.5f);

        animator.SetInteger("Pattern", 3); // "Pattern" 값을 3로 설정

        StartCoroutine(pt3Projectiles());

        StartCoroutine(PatternReset(0.5f)); // 일정 시간 후 "Pattern" 값 0으로 초기화

        yield return new WaitForSeconds(2.0f); // 다음 패턴을 위한 대기시간

        canAttack = true;
        canRangedAttack = true;
    }

    // 공격패턴3 공격범위 표시
    private IEnumerator pt3AttackRanges()
    {
        for (int i = 0; i < 5; i++)
        {
            // 현재 위치에서 x축으로 -20 * i 만큼 이동
            Vector3 spawnPosition = pt3SpawnPoint.position + new Vector3(-7.5f * i, 0f, 0f);

            // pt3AttackRange 생성
            Instantiate(pt3AttackRange, spawnPosition, Quaternion.Euler(0f, 0f, 0f));

            // 딜레이
            yield return new WaitForSeconds(0.3f);
        }
    }

    // 공격패턴3 공격 이펙트
    private IEnumerator pt3Projectiles()
    {
        for (int i = 0; i < 5; i++)
        {
            // 현재 위치에서 x축으로 -20 * i 만큼 이동
            Vector3 spawnPosition = pt3SpawnPoint.position + new Vector3(-7.5f * i, 0f, 0f);

            // pt3Projectile 생성
            GameObject spawnedObject = Instantiate(pt3Particle, spawnPosition, Quaternion.Euler(0f, 0f, 0f));

            //사운드
            AudioUtility.CreateSFX(pt3Sound, player.transform.position, AudioGroups.Skill, 0f);

            // 딜레이
            yield return new WaitForSeconds(0.3f);

            Destroy(spawnedObject, 1.0f);
        }
    }

    // 공격패턴4 : 근접 공격
    public IEnumerator Pattern4()
    {
        // 공격범위 표시
        Instantiate(pt4AttackRange, new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z + -2.0f), Quaternion.Euler(0f, 180f, 0f));

        yield return new WaitForSeconds(1.5f);

        animator.SetInteger("Pattern", 4); // "Pattern" 값을 4로 설정

        //사운드
        AudioUtility.CreateSFX(pt4Sound, player.transform.position, AudioGroups.Skill, 0f);

        yield return new WaitForSeconds(0.2f);

        Instantiate(pt4Particle, pt4SpawnPoint.position, Quaternion.Euler(0f, 0f, 0f));

        yield return new WaitForSeconds(0.3f);

        animator.SetInteger("Pattern", 0); // "Pattern" 값 0으로 초기화

        yield return new WaitForSeconds(5.0f); // 다음 패턴을 위한 대기시간

        canMeleeAttack = true;
    }

    // 공격패턴5 : 낙석 공격
    public IEnumerator Pattern5()
    {
        animator.SetInteger("Pattern", 5); // "Pattern" 값을 5로 설정

        yield return new WaitForSeconds(1.0f);

        animator.SetInteger("Pattern", 0); // "Pattern" 값 0으로 초기화

        // 공격범위 표시
        StartCoroutine(InstantiatePt5AttackRange());

        //Instantiate(pt4Particle, pt4SpawnPoint.position, Quaternion.Euler(0f, 0f, 0f));

        yield return new WaitForSeconds(6.0f); // 다음 패턴을 위한 대기시간

        canAttack = true;
        canRangedAttack = true;
    }

    private IEnumerator InstantiatePt5AttackRange()
    {
        for (int i = 0; i < pt5spawnCount; i++)
        {
            // 원형 범위 내 랜덤한 위치 계산
            Vector2 randomPos = Random.insideUnitCircle * pt5radius;

            // 프리팹 생성 (2D 위치를 3D로 변환)
            Instantiate(pt5AttackRange, new Vector3(randomPos.x, 0, randomPos.y), Quaternion.identity);

            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator PatternReset(float delay)
    { 
        yield return new WaitForSeconds(delay);
        animator.SetInteger("Pattern", 0);
    }

    private IEnumerator pt1ProjectileTiming(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 발사체 생성
        GameObject projectile = Instantiate(pt1Particle, pt1SpawnPoint.position, Quaternion.Euler(0f, 0f, 0f));

        // 발사체가 플레이어 방향으로 회전하도록 설정
        Vector3 directionToPlayer = (player.transform.position - pt1SpawnPoint.position).normalized;  // 플레이어 방향으로 벡터 계산
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);  // 방향으로 회전할 쿼터니언 생성

        projectile.transform.rotation = targetRotation;  // 발사체의 회전 설정
    }

    private IEnumerator pt2ProjectileTiming(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(pt2Particle, pt2SpawnPoint.position, Quaternion.Euler(0f, 0f, 0f));
    }
}
