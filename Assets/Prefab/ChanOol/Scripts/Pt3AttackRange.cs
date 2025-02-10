using UnityEngine;

public class Pt3AttackRange : MonoBehaviour
{
    // 페이드 효과
    private Material material;
    [SerializeField] private float startAlpha = 1f;  // 시작 알파 값
    [SerializeField] private float targetAlpha = 0f; // 목표 알파 값
    [SerializeField] private float fadeSpeed = 0.5f; // 초당 변경 속도
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        material = GetComponent<MeshRenderer>().material;

        // 시작 알파 값 설정
        Color color = material.color;
        color.a = startAlpha;
        material.color = color;

        Destroy(gameObject, 0.4f);
    }

    // Update is called once per frame
    void Update()
    {
        Color color = material.color;
        color.a = Mathf.MoveTowards(color.a, targetAlpha, fadeSpeed * Time.deltaTime);
        material.color = color;

        // 목표 알파 값에 도달하면 Update 비활성화
        if (Mathf.Approximately(color.a, targetAlpha))
        {
            enabled = false;
        }
    }
}
