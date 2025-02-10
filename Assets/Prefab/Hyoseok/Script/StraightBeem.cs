using UnityEngine;

public class StraightBeem : MonoBehaviour
{
    private LineRenderer lineRenderer;
    public Color laserColor = Color.red; // ������ ����
    public float width = 0.2f; // ������ �ʺ�
    public float laserLength = 50f; // ������ ����

    void Start()
    {
        // LineRenderer ������Ʈ�� �߰��մϴ�.
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = width;
        lineRenderer.endWidth = width;
        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;
        lineRenderer.positionCount = 2; // �� ���� ���� ����մϴ�.

        // ���� �������� �������� ������ �����մϴ�.
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * laserLength;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }

    void Update()
    {
        // �� �����Ӹ��� �������� ���� ��ġ�� ������Ʈ�մϴ�.
        Vector3 startPoint = transform.position;
        Vector3 endPoint = transform.position + transform.forward * laserLength;

        lineRenderer.SetPosition(0, startPoint);
        lineRenderer.SetPosition(1, endPoint);
    }
}
