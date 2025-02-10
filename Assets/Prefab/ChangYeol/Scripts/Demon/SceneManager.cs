using BS.Managers;
using BS.Player;
using BS.PlayerInput;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace BS.Demon
{
    public class SceneManager : MonoBehaviour
    {
        #region Variables
        public DemonPattern pattern;
        public GameObject WarningCanvas;
        public Camera main;
        public GameObject drectingCamera;
        public GameObject bossCanvas;
        public GameObject presentEffect;
        public PlayerController player;

        [HideInInspector]public bool isPhase = false;
        private ToggleRendererFeature toggleRenderer;
        public GameObject canvas;
        private CameraManager manager;
        [SerializeField] private GameObject angryEffect;

        private bool isheal;
        [SerializeField] private bool ischting;
        public GameObject FadeOut;
        #endregion

        private void Start()
        {
            toggleRenderer = GetComponent<ToggleRendererFeature>();
            toggleRenderer.enabled = false;
            manager = main.GetComponent<CameraManager>();
            manager.enabled = false;
            isPhase = false;
            isheal= false;
            StartCoroutine(OpeningDemon());
        }
        void Update()
        {
            if (WarningCanvas)
            {
                WarningCanvas.transform.LookAt(WarningCanvas.transform.position + main.transform.rotation * Vector3.forward, main.transform.rotation * Vector3.up);

            }
            if (Input.GetKeyDown(KeyCode.X) && ischting)
            {
                if (pattern.demon.currentHealth > 0)
                {
                    pattern.demon.TakeDamage(pattern.demon.maxHealth);
                }
            }
            if (pattern.demon.hasRecovered && isPhase == false)
            {
                StartCoroutine (PhaseDemon());
                
            }
        }
        IEnumerator OpeningDemon()
        {
            FadeOut.SetActive(true);
            yield return new WaitForSeconds(1);
            FadeOut.SetActive(false);
            manager.enabled = false;
            pattern.demon.enabled = false;
            player.enabled = false;
            PlayerInputActions actions = player.GetComponent<PlayerInputActions>();
            actions.UnInputActions();
            pattern.demon.gameObject.SetActive(false);
            GameObject eff = Instantiate(presentEffect, pattern.gameObject.transform.position, presentEffect.transform.rotation);
            yield return new WaitForSeconds(0.2f);
            pattern.demon.gameObject.SetActive(true);
            float descentDuration = 4f; //작아지는 시간
            float elapsedTime = 0f;
            bossCanvas.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            toggleRenderer.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", true);
            while (elapsedTime < descentDuration)
            {
                eff.transform.localScale = Vector3.Lerp(eff.transform.localScale, new Vector3(0.1f,0.1f,0.1f)*-0f, elapsedTime / descentDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            Destroy(eff,0.8f);
            //TODO : 카메라 흔들면서 연출 효과 나오고 이름 나오고 시작
            canvas.SetActive(false);
            bossCanvas.SetActive(false);
            yield return new WaitForSeconds(0.5f);
            toggleRenderer.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false);
            drectingCamera.SetActive(false);
            canvas.SetActive(true);
            pattern.demon.enabled = true;
            pattern.demon.ChangeState(DEMON.Idle);
            player.enabled = true;
            actions.OnInputActions();
            manager.enabled = true;
            pattern.demon.clearTime.StartDungeon();
            yield return null;
        }
        IEnumerator PhaseDemon()
        {
            drectingCamera.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            pattern.demon.enabled = false;
            player.enabled = false;
            //PlayerStateMachine playerState = player.GetComponent<PlayerStateMachine>();
            //playerState.ChangeState(playerState.IdleState);
            PlayerInputActions actions = player.GetComponent<PlayerInputActions>();
            actions.UnInputActions();
            toggleRenderer.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", true);
            yield return new WaitForSeconds(2f);
            angryEffect.SetActive(true);
            if (!isheal)
            {
                GameObject heal = Instantiate(pattern.effect[3], pattern.gameObject.transform.position, Quaternion.identity);
                isheal = true;
                Destroy(heal,0.5f);
                yield return new WaitForSeconds(2f);
            }
            //TODO : 카메라 흔들리면서 시작
            toggleRenderer.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false);
            isPhase = true;
            yield return new WaitForSeconds(0.5f);
            drectingCamera.SetActive(false);
            pattern.demon.enabled = true;
            pattern.demon.ChangeState(DEMON.Idle);
            player.enabled = true;
            actions.OnInputActions();
            yield return new WaitForSeconds(0.5f);
            pattern.demon.animator.SetBool("IsPhase", isPhase);
            yield return null;
        }
    }
}