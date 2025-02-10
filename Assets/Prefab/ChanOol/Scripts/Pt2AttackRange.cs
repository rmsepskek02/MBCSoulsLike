using UnityEngine;

public class Pt2AttackRange : MonoBehaviour
{
    public float growSpeed = 2f; // 성장 속도
    private bool isGrowing = false; // 성장 중인지 확인

    [SerializeField] private Vector3 originalScale; // 초기 크기
    [SerializeField] private Vector3 targetScale; // 초기 크기

    // 페이드 효과
    private Material material;
    [SerializeField] private float startAlpha = 1f; // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    private bool isFading = true; // 알파 값 전환 활성화 여부

    private void Start()
    {
        // 현재 크기를 초기 크기로 저장
        originalScale = transform.localScale;

        StartGrowing(transform.localScale, 100f);

        // 페이드 효과
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;
    }

    private void Update()
    {
        UpdateScale();

        // 다 커지고 일정시간 후 오브젝트 삭제
        if (isGrowing == false)
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

            Destroy(gameObject, 2f);
        }
    }

    public void StartGrowing(Vector3 StartScale, float Range)
    {
        // 목표 크기 설정
        //targetScale = StartScale * Range;
        targetScale = new Vector3(StartScale.x * Range, StartScale.y, StartScale.z * Range);
        // 성장 시작
        isGrowing = true;
    }
    public void UpdateScale()
    {
        if (isGrowing)
        {
            // 부드럽게 크기 증가
            transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);

            // 목표 크기에 도달했는지 확인
            if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
            {
                isGrowing = false; // 성장 종료
                //Debug.Log("Reached target size!");
            }
        }
    }
}
