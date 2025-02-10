using System.Collections;
using UnityEngine;

public class Pt5Test : MonoBehaviour
{
    public GameObject pt5AttackRange; // ������ ������
    public float radius = 5f; // ���� ���� ������
    public int spawnCount = 5; // ������ ����
    public GameObject prefab; // ������ ����Ʈ

    private void Start()
    {
        StartCoroutine(SpawnPrefabs());
    }

    private IEnumerator SpawnPrefabs()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            // ���� ���� �� ������ ��ġ ���
            Vector2 randomPos = Random.insideUnitCircle * radius;

            // ������ ���� (2D ��ġ�� 3D�� ��ȯ)
            Instantiate(pt5AttackRange, new Vector3(randomPos.x, 0, randomPos.y), Quaternion.identity);

            yield return new WaitForSeconds(0.3f);

            Instantiate(prefab, new Vector3(randomPos.x, 0, randomPos.y), Quaternion.identity);
        }
    }
}
