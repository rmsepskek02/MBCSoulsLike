using UnityEngine;

namespace BS.Demon
{
public class DemonAttackRange : MonoBehaviour
    {
        public float growSpeed = 2f; // 성장 속도
        public bool isGrowing = false; // 성장 중인지 확인

        private Vector3 originalScale; // 초기 크기
        private Vector3 targetScale; // 초기 크기
        private void Start()
        {
            // 현재 크기를 초기 크기로 저장
            originalScale = transform.localScale;
        }

        private void Update()
        {
            UpdateScale();
        }

        public void StartGrowing(Vector3 StartScale, float Range)
        {
            // 목표 크기 설정
            targetScale = StartScale * Range;
            // 성장 시작
            isGrowing = true;
        }
        private void UpdateScale()
        {
            if (isGrowing)
            {
                // 부드럽게 크기 증가
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);

                // 목표 크기에 도달했는지 확인
                if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
                {
                    isGrowing = false; // 성장 종료
                }
            }
        }
    }
}