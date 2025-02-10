using UnityEngine;
using System.Collections;
using Unity.Cinemachine;
using BS.Utility;
using BS.Managers;

namespace BS.Enemy.Set
{
    public class SetOpening : MonoBehaviour
    {
        #region Variables
        [SerializeField] GameObject player;
        [SerializeField] GameObject gameHUD;
        [SerializeField] GameObject boss;
        [SerializeField] GameObject AchievementCanvas;
        [SerializeField] GameObject EndingManager;
        [SerializeField] ParticleSystem chargingParticle;
        [SerializeField] ParticleSystem explosionParticle;
        [SerializeField] CinemachineSequencerCamera cutSceneCamera;
        [SerializeField] GameObject fade;
        public AudioClip explosionSound;
        AudioSource audioSource;
        Camera cm;
        Animator animator;

        #endregion

        private void Awake()
        {
            cm = Camera.main;

            cm.GetComponent<CameraManager>().enabled = false;

            audioSource = GetComponent<AudioSource>();
            boss.SetActive(false);
            player.SetActive(false);
            EndingManager.SetActive(false);
            gameHUD.gameObject.SetActive(false);
            chargingParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            explosionParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        void Start()
        {
            animator = GetComponent<Animator>();
            StartCoroutine(Opening());
        }


        IEnumerator Opening()
        {
            GameObject go = Instantiate(fade);
            Destroy(go, 1f);
            yield return new WaitForSeconds(2f);
            animator.SetTrigger(SetProperty.SET_ANIM_TRIGGER_ROAR);
            yield return new WaitForSeconds(1f);
            audioSource.Play();
            chargingParticle.Play();
            yield return new WaitForSeconds(2.2f);
            AudioUtility.CreateSFX(explosionSound, transform.position, AudioUtility.AudioGroups.Explosion);
            chargingParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            explosionParticle.Play();
            yield return new WaitForSeconds(4.5f);
            cm.fieldOfView = 60f;
            player.SetActive(true);
            gameHUD.gameObject.SetActive(true);
            boss.SetActive(true);
            cutSceneCamera.gameObject.SetActive(false);
            AchievementCanvas.SetActive(true);
            EndingManager.SetActive(true);
            cm.GetComponent<CameraManager>().enabled = true;
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            StopCoroutine(Opening());
        }
    }
}