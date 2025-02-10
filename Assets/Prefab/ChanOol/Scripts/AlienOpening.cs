using System.Collections;
using BS.UI;
using UnityEngine;

public class AlienOpening : MonoBehaviour
{
    public Animator animator;
    public AudioSource audioSource;
    public DungeonClearTime clearTime;
    public GameObject fadeInOut;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AlienOpen());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator AlienOpen()
    {
        fadeInOut.SetActive(true);
        yield return new WaitForSeconds(2.0f);
        fadeInOut.SetActive(false);

        // 여기에 시퀀스카메라 활성화
        //sequeneCamera.SetActive(true);

        yield return new WaitForSeconds(6.5f);

        animator.SetInteger("Pattern", 1);

        // 사운드 넣기
        audioSource.Play();

        yield return new WaitForSeconds(0.5f);

        animator.SetInteger("Pattern", 0);

        clearTime.StartDungeon();
    }
}
