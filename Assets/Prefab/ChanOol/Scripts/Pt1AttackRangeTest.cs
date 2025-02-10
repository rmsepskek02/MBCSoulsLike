using UnityEngine;

public class Pt1AttackRangeTest : MonoBehaviour
{
    public float growSpeed = 2f; // 성장 속도
    private bool isGrowing = false; // 성장 중인지 확인

    private Vector3 originalScale; // 초기 크기
    private Vector3 targetScale; // 목표 크기

    // 페이드 효과
    private Material material;
    [SerializeField] private float startAlpha = 1f; // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    private bool isFading = true; // 알파 값 전환 활성화 여부

    // 부채꼴 메쉬 관련 변수
    private Mesh mesh;
    private MeshFilter meshFilter;
    private int segmentCount = 30; // 부채꼴을 나누는 세그먼트 수 (각각의 작은 삼각형을 위한 개수)

    private void Start()
    {
        // 현재 크기를 초기 크기로 저장
        originalScale = transform.localScale;

        StartGrowing(transform.localScale, 150f);

        // 페이드 효과
        material = GetComponent<MeshRenderer>().material;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        // 메쉬 필터와 메쉬 설정
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        meshFilter.mesh = mesh;
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
                Debug.Log("Reached target size!");
            }

            // 부채꼴 모양 업데이트
            UpdateMesh();
        }
    }

    private void UpdateMesh()
    {
        float radius = transform.localScale.x; // 반지름 설정 (x축 크기를 사용)
        float angleStep = 180f / segmentCount; // 각도 단위로 나누기 (180도 부채꼴이므로)

        // 메쉬 초기화
        Vector3[] vertices = new Vector3[segmentCount + 2]; // +2는 중심과 끝 점을 포함
        int[] triangles = new int[segmentCount * 3]; // 각 세그먼트마다 3개의 삼각형 인덱스
        Vector2[] uvs = new Vector2[segmentCount + 2];

        // 중심점 설정
        vertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f); // UV 좌표

        // 각도에 따른 꼭짓점 계산
        for (int i = 0; i < segmentCount; i++)
        {
            float angle = angleStep * i;
            float radian = Mathf.Deg2Rad * angle;
            float x = Mathf.Cos(radian) * radius;
            float z = Mathf.Sin(radian) * radius;

            vertices[i + 1] = new Vector3(x, 0, z); // 부채꼴 꼭짓점
            uvs[i + 1] = new Vector2((x / radius) * 0.5f + 0.5f, (z / radius) * 0.5f + 0.5f); // UV 좌표

            // 삼각형 설정 (부채꼴을 이루는 삼각형들)
            int triIndex = i * 3;
            triangles[triIndex] = 0; // 중심점
            triangles[triIndex + 1] = i + 1;
            triangles[triIndex + 2] = i + 2;
        }

        // 마지막 꼭짓점의 삼각형을 처리
        triangles[triangles.Length - 3] = 0;
        triangles[triangles.Length - 2] = segmentCount;
        triangles[triangles.Length - 1] = 1;

        // 메쉬에 업데이트된 버텍스, 삼각형, UV 좌표 할당
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // 메쉬의 노멀과 바운드 업데이트
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}

