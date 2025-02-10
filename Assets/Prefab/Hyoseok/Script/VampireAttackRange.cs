using UnityEngine;

namespace BS.vampire { 
public class VampireAttackRange : MonoBehaviour
{
        public float growSpeed = 2f; // ���� �ӵ�
        private bool isGrowing = false; // ���� ������ Ȯ��

        private Vector3 originalScale; // �ʱ� ũ��
        private Vector3 targetScale; // �ʱ� ũ��

        private void Start()
        {
            // ���� ũ�⸦ �ʱ� ũ��� ����
            originalScale = transform.localScale;
        }

        private void Update()
        {
            UpdateScale();
        }

        public void StartGrowing(Vector3 StartScale, float Range)
        {
            // ��ǥ ũ�� ����
            transform.localScale = new Vector3(StartScale.x, StartScale.y, StartScale.z);
            targetScale = new Vector3(StartScale.x * Range, StartScale.y, StartScale.z * Range);
            // ���� ����
            isGrowing = true;
        }
        public void UpdateScale()
        {
            if (isGrowing)
            {
                // �ε巴�� ũ�� ����
                transform.localScale = Vector3.MoveTowards(transform.localScale, targetScale, growSpeed * Time.deltaTime);

                // ��ǥ ũ�⿡ �����ߴ��� Ȯ��
                if (Vector3.Distance(transform.localScale, targetScale) < 0.01f)
                {
                    isGrowing = false; // ���� ����
                    Debug.Log("Reached target size!");
                }
            }
        }
    }
}