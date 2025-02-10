using System.Collections;
using BS.Utility;
using UnityEngine;
using static BS.Utility.AudioUtility;

public class Pt5AttackRange : MonoBehaviour
{
    public float growSpeed = 2f; // 성장 속도
    private bool isGrowing = false; // 성장 중인지 확인
    private bool isShrinking = false; // 축소 중인지 확인
    public GameObject pt5Particle;

    [SerializeField] private Vector3 originalScale; // 초기 크기
    [SerializeField] private Vector3 targetScale; // 목표 크기

    // 페이드 효과
    private Material material;
    [SerializeField] private float startAlpha = 1f; // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    private bool isFading = true; // 알파 값 전환 활성화 여부

    // 사운드
    public AudioClip pt5Sound;             // 패턴 효과음

    private void Start()
    {
        // 현재 크기를 초기 크기로 저장
        originalScale = transform.localScale;

        // 예: 축소 시작
        StartShrinking(transform.localScale, 0.01f);

        // 페이드 효과
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;
    }

    private void Update()
    {
        StartCoroutine(UpdateScale());

        // 다 작아지고 일정 시간 후 오브젝트 삭제
        if (!isGrowing && !isShrinking)
        {
            // 페이드 효과
            if (isFading)
            {
                // 알파 값 전환 로직
                Color color = material.color;
                color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
                material.color = color;

                // 목표 알파 값에 도달하면 전환 멈춤
                if (Mathf.Approximately(color.a, targetAlpha))
                {
                    isFading = false;
                }
            }

            Destroy(gameObject, 4f);  // 오브젝트 삭제
        }
    }

    public void StartGrowing(Vector3 startScale, float range)
    {
        // 목표 크기 설정
        targetScale = new Vector3(startScale.x * range, startScale.y, startScale.z * range);
        isGrowing = true;
        isShrinking = false;
    }

    public void StartShrinking(Vector3 startScale, float shrinkFactor)
    {
        // 목표 크기 설정
        targetScale = new Vector3(startScale.x * shrinkFactor, startScale.y, startScale.z * shrinkFactor);
        isShrinking = true;
        isGrowing = false;
    }

    public IEnumerator UpdateScale()
    {
        if (isGrowing || isShrinking)
        {
            // 부드럽게 크기 변경
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);

            // 목표 크기에 도달했는지 확인
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                isGrowing = false;
                isShrinking = false; // 축소 종료
                //Debug.Log("Reached target size!");

                // pt5Particle 생성 후, 2초 후에 파티클을 삭제하도록 코루틴 호출
                GameObject particle = Instantiate(pt5Particle, transform.position, Quaternion.identity);

                //사운드
                AudioUtility.CreateSFX(pt5Sound, transform.position, AudioGroups.Skill, 1f, 10f, 30f);

                StartCoroutine(DestroyParticleAfterTime(particle, 4f)); // 4초 후 삭제

                yield return new WaitForSeconds(2f);  // 파티클 생성을 위한 대기 시간
            }
        }
    }

    // 파티클을 일정 시간이 지난 후 삭제하는 함수
    private IEnumerator DestroyParticleAfterTime(GameObject particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(particle); // 파티클 삭제
    }
}

