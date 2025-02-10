using System.Collections;
using UnityEngine;

namespace BS.Demon
{
    public abstract class DemonBall : MonoBehaviour
    {
        public abstract void TargetRise();
        #region Variables
        public float targetHeight = 15f; // 목표 높이
        public float riseSpeed = 2f; // 상승 속도
        private Vector3 startPosition; // 시작 위치
        private Vector3 targetPosition; // 목표 위치
        private bool isRising = false; // 상승 상태 플래그
        public DemonPattern pattern;
        [HideInInspector]public TwoPhasePattern twoPhase;
        #endregion
        private void Start()
        {
            // 현재 위치를 기준으로 목표 위치 계산
            startPosition = transform.position;
            targetPosition = startPosition + Vector3.up * targetHeight;
            twoPhase = pattern.GetComponent<TwoPhasePattern>();
        }
        private void Update()
        {
            StartCoroutine(UpRise());
        }
        public void StartRise()
        {
            // 상승 시작
            isRising = true;
        }
        IEnumerator UpRise()
        {
            if (isRising)
            {
                // 부드럽게 상승
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, riseSpeed * Time.deltaTime);

                // 목표 위치에 도달했는지 확인
                if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
                {
                    isRising = false; // 상승 종료
                    yield return new WaitForSeconds(1f);
                    TargetRise();
                }
            }
        }
    }
}