using BS.Player;
using BS.PlayerInput;
using TMPro;
using UnityEngine;

namespace BS.Title
{
    public abstract class StageTrigger : MonoBehaviour
    {
        protected abstract void TriggerKeyDown();
        #region Variables
        public TextMeshProUGUI keyText;
        public TextMeshProUGUI stageText;
        public GameObject canvas;
        [HideInInspector]public GameObject Enemy;
        [SerializeField] protected GameObject InstEnemy;
        [HideInInspector] public bool isEnemy = false;
        public string stageName;
        public KeyCode keyCode = KeyCode.V;
        public AudioClip triggerSound;
        private bool isPlayerInside = false;
        #endregion

        private void Start()
        {
            keyText.text = "";
            stageText.text = "";
        }

        private void Update()
        {
            if (isPlayerInside)
            {
                TriggerKeyDown();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                keyText.text = "[ " + keyCode.ToString() + " ]";
                canvas.SetActive(true);
                isPlayerInside = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                stageText.text = "";
                keyText.text = "";
                canvas.SetActive(false);
                isPlayerInside = false;
            }
        }
    }
}
