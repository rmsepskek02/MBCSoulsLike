using UnityEngine;

public class StraightBeem : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Color laserColor = Color.red; // 레이저 색상
    public float width = 0.2f; // 레이저 너비
    public float laserLength = 50f; // 레이저 길이

    void Start()
    {
        // LineRenderer 컴포넌트를 추가합니다.
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        lineRenderer.positionCount = 2; // 두 개의 점을 사용합니다.

        // 라인 렌더러의 시작점과 끝점을 설정합니다.
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * laserLength;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    void Update()
    {
        // 매 프레임마다 시작점과 끝점 위치를 업데이트합니다.
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * laserLength;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
