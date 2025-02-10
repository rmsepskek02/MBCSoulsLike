using BS.Player;
using BS.PlayerInput;
using BS.UI;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace BS.vampire
{
    public class Drecting : MonoBehaviour
    {
        #region Variables
        public GameObject fadeinout;
        public GameObject drectingCamera;
        public GameObject boss;
        public GameObject bossCanvas;
        public GameObject playerCanvas;
        public GameObject timer;
        public Animator animator;
        public GameObject presentEffect;
        public GameObject player;
        private DungeonClearTime dungeEndGame;
        private PlayerInputActions playerInputActions;
        #endregion


        private void Awake()
        {
            dungeEndGame = FindFirstObjectByType<DungeonClearTime>();
            PattonSummon pattonSummon = boss.GetComponent<PattonSummon>();
            VampireController vampireController = boss.GetComponent<VampireController>();
            pattonSummon.enabled = false;
            vampireController.enabled = false;
        }
        private void Start()
        {
          
            StartCoroutine(Opening());
        }
        

        IEnumerator Opening()
        {
            GameObject fade = Instantiate(fadeinout,this.gameObject.transform);
            Destroy(fade, 1f);

            yield return new WaitForSeconds(1f);
            ///**/
            //fadeinout.SetActive(false);
            timer.SetActive(false);
            playerCanvas.SetActive(false);
            PattonSummon pattonSummon = boss.GetComponent<PattonSummon>();
            VampireController vampireController = boss.GetComponent<VampireController>();
            playerInputActions = player.GetComponent<PlayerInputActions>();
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerInputActions.UnInputActions();
            playerController.enabled = false;
            boss.transform.position = new Vector3(boss.transform.position.x, 10f, boss.transform.position.z);

            float descentDuration = 5f; // 내려오는 데 걸리는 시간
            float elapsedTime = 0f;
            Vector3 startPos = boss.transform.position;
            Vector3 endPos = new Vector3(boss.transform.position.x, 0f, boss.transform.position.z);
          
            animator.SetBool("IsFlying", true);
            // 내려오는 애니메이션
            while (elapsedTime < descentDuration)
            {
                boss.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / descentDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            animator.SetBool("IsFlying", false);
            yield return new WaitForSeconds(0.5f);
            
            animator.SetTrigger("Attack1");
            bossCanvas.SetActive(true);
            GameObject effectGo = Instantiate(presentEffect,boss.transform.position,Quaternion.identity);
            Destroy(effectGo, 5f);


            // 패턴 시작 시간
            yield return new WaitForSeconds(5f);
            timer.SetActive(true);
            playerCanvas.SetActive(true );
            yield return new WaitForSeconds(0.01f);
            Destroy(drectingCamera);
            Destroy(bossCanvas);

            pattonSummon.enabled = true;
            vampireController.enabled = true;
            playerController.enabled = true;
            playerInputActions.OnInputActions();
            dungeEndGame.StartDungeon();
            
         yield return null;
        }
    }
}
