using System.Collections;
using UnityEngine;

public class Pt5Test : MonoBehaviour
{
    public GameObject pt5AttackRange; // 생성할 프리팹
    public float radius = 5f; // 원형 범위 반지름
    public int spawnCount = 5; // 생성할 개수
    public GameObject prefab; // 생성할 이펙트

    private void Start()
    {
        StartCoroutine(SpawnPrefabs());
    }

    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // 원형 범위 내 랜덤한 위치 계산
            Vector2 randomPos = Random.insideUnitCircle * radius;

            // 프리팹 생성 (2D 위치를 3D로 변환)
            Instantiate(pt5AttackRange, new Vector3(randomPos.x, 0, randomPos.y), Quaternion.identity);

            yield return new WaitForSeconds(0.3f);

            Instantiate(prefab, new Vector3(randomPos.x, 0, randomPos.y), Quaternion.identity);
        }
    }
}
