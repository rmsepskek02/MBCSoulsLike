using System.Collections;
using UnityEngine;

namespace BS.vampire
{
    /// <summary>
    /// ���� �̵� �ý��� 
    /// �߾��̵��� CircleShoot�߻�
    /// �ܰ��̵��� PingPongShot�߻�
    /// </summary>
    public class VamprieTeleport : MonoBehaviour
    {
        #region Variables
        public GameObject pingpongShot;
        //public GameObject CircleShot;
        public GameObject teleportEffect;
        public Transform[] teleports; //�����̵� ��ġ 0~3 ���� 4�� �߾�
        public float time = 20; //�����̵� ��Ÿ��
        public Animator animator;
        private int previousIndex = -1; //���� ��ġ��
        public Transform player;
        #endregion

        private void Start()
        {

            //pingpongShot = pingpongShot.GetComponent<ParticleSystem>();
            //var emission = pingpongShot.emission;
            //emission.rateOverTime = 0f;
            //CircleShot = CircleShot.GetComponent<ParticleSystem>();
            if (animator == null)
            {
                animator = GetComponent<Animator>();
            }
            StartCoroutine(RandomTeleport());
        }

        IEnumerator RandomTeleport()
        {
            while (true)
            {
                yield return new WaitForSeconds(time);
                //�ִϸ��̼� ���� 3���Ŀ� �̵�
                animator.SetTrigger("Teleport");
                GameObject potalEffect = Instantiate(teleportEffect, transform.position, Quaternion.identity);
                potalEffect.transform.parent = transform;
                Destroy(potalEffect, 3.3f);
                yield return new WaitForSeconds(2.5f);
                int randomIndex;
                do
                {
                    randomIndex = Random.Range(0, 3);
                }
                while (randomIndex == previousIndex); //������ ���ӹ���
                //���� ��ġ�� �����̵�
                transform.position = teleports[randomIndex].position;
                previousIndex = randomIndex;

                //�÷��̾� �ٶ󺸸� �ȱ�
                transform.LookAt(player.position);
                //animator.SetTrigger("Walk");

                float walkDuration = 5f;
                float elapsedTime = 0f;

                Vector3 StartPos = transform.position;
                Vector3 endPos = transform.position + new Vector3(transform.forward.x, 0, transform.forward.z) * 5f;

                //�ȴ� ���� ź���߻�
                //�������� ��ü
                Vector3 spawnPosition = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                GameObject tan = Instantiate(pingpongShot, spawnPosition, pingpongShot.transform.rotation);


                Destroy(tan, 10f);
                while (elapsedTime < walkDuration)
                {
                    transform.position = Vector3.Lerp(StartPos, endPos, elapsedTime/walkDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
            

               
           

                yield return new WaitForSeconds(10f);
               
            }
        }
    }
}