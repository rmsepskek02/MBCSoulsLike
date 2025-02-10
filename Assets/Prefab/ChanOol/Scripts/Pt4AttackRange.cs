using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]

public class Pt4AttackRange : MonoBehaviour
{
    public float degree = 180;
    public float intervalDegree = 5;
    public float beginOffsetDegree = 0;
    public float radius = 10;

    Mesh mesh;
    MeshFilter meshFilter;

    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    int i;
    float beginDegree;
    float endDegree;
    float beginRadian;
    float endRadian;
    float uvRadius = 0.5f;
    Vector2 uvCenter = new Vector2(0.5f, 0.5f);
    float currentIntervalDegree = 0;
    float limitDegree;
    int count;
    int lastCount;

    float beginCos;
    float beginSin;
    float endCos;
    float endSin;

    int beginNumber;
    int endNumber;
    int triangleNumber;

    // 인스펙터에서 마테리얼을 할당할 수 있도록 public으로 설정
    public Material material;

    // 페이드 효과
    [SerializeField] private float startAlpha = 1f; // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    private bool isFading = true; // 알파 값 전환 활성화 여부

    // Use this for initialization 
    void Start()
    {
        mesh = new Mesh();
        meshFilter = (MeshFilter)GetComponent("MeshFilter");

        // 페이드 효과
        material = GetComponent<MeshRenderer>().sharedMaterial;
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;
    }

    // Update is called once per frame 
    void Update()
    {
        // 페이드 효과
        if (isFading)
        {
            // 알파 값 전환 로직
            Color color = material.color;
            color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
            material.color = color;

            // 목표 알파 값에 도달하면 전환 멈춤 및 오브젝트 삭제
            if (Mathf.Approximately(color.a, targetAlpha))
            {
                isFading = false;

                // 에디터 모드와 플레이 모드에 따라 적절한 삭제 함수 사용
                if (Application.isPlaying)
                {
                    Destroy(gameObject); // 플레이 모드
                }
                else
                {
                    DestroyImmediate(gameObject); // 에디터 모드
                }
                return; // 삭제 후 더 이상 처리하지 않도록 종료
            }
        }

        currentIntervalDegree = Mathf.Abs(intervalDegree);

        count = (int)(Mathf.Abs(degree) / currentIntervalDegree);
        if (degree % intervalDegree != 0)
        {
            ++count;
        }
        if (degree < 0)
        {
            currentIntervalDegree = -currentIntervalDegree;
        }

        if (lastCount != count)
        {
            mesh.Clear();
            vertices = new Vector3[count * 2 + 1];
            triangles = new int[count * 3];
            uvs = new Vector2[count * 2 + 1];
            vertices[0] = Vector3.zero;
            uvs[0] = uvCenter;
            lastCount = count;
        }

        i = 0;
        beginDegree = beginOffsetDegree;
        limitDegree = degree + beginOffsetDegree;

        while (i < count)
        {
            endDegree = beginDegree + currentIntervalDegree;

            if (degree > 0)
            {
                if (endDegree > limitDegree)
                {
                    endDegree = limitDegree;
                }
            }
            else
            {
                if (endDegree < limitDegree)
                {
                    endDegree = limitDegree;
                }
            }

            beginRadian = Mathf.Deg2Rad * beginDegree;
            endRadian = Mathf.Deg2Rad * endDegree;

            beginCos = Mathf.Cos(beginRadian);
            beginSin = Mathf.Sin(beginRadian);
            endCos = Mathf.Cos(endRadian);
            endSin = Mathf.Sin(endRadian);

            beginNumber = i * 2 + 1;
            endNumber = i * 2 + 2;
            triangleNumber = i * 3;

            vertices[beginNumber].x = beginCos * radius;
            vertices[beginNumber].y = 0;
            vertices[beginNumber].z = beginSin * radius;
            vertices[endNumber].x = endCos * radius;
            vertices[endNumber].y = 0;
            vertices[endNumber].z = endSin * radius;

            triangles[triangleNumber] = 0;
            if (degree > 0)
            {
                triangles[triangleNumber + 1] = endNumber;
                triangles[triangleNumber + 2] = beginNumber;
            }
            else
            {
                triangles[triangleNumber + 1] = beginNumber;
                triangles[triangleNumber + 2] = endNumber;
            }

            if (radius > 0)
            {
                uvs[beginNumber].x = beginCos * uvRadius + uvCenter.x;
                uvs[beginNumber].y = beginSin * uvRadius + uvCenter.y;
                uvs[endNumber].x = endCos * uvRadius + uvCenter.x;
                uvs[endNumber].y = endSin * uvRadius + uvCenter.y;
            }
            else
            {
                uvs[beginNumber].x = -beginCos * uvRadius + uvCenter.x;
                uvs[beginNumber].y = -beginSin * uvRadius + uvCenter.y;
                uvs[endNumber].x = -endCos * uvRadius + uvCenter.x;
                uvs[endNumber].y = -endSin * uvRadius + uvCenter.y;
            }

            beginDegree += currentIntervalDegree;
            ++i;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.sharedMesh = mesh;
        meshFilter.sharedMesh.name = "CircularSectorMesh";
    }
}
