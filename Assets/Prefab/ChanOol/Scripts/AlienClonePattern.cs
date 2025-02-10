using UnityEngine;

public class AlienClonePattern : MonoBehaviour
{
    public Animator animator;

    public GameObject cloneProjectile;       // 발사체 프리팹

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 spawnPosition = transform.position + transform.forward * 2f;

        Instantiate(cloneProjectile, spawnPosition, Quaternion.LookRotation(transform.forward));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
