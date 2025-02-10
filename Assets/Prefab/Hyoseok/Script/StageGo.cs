using BS.Player;
using BS.PlayerInput;
using BS.Utility;
using UnityEngine;

namespace BS.Title
{
    public class StageGo : StageTrigger
    {
        public GameObject StageCanvas;
        [SerializeField]private PlayerInputActions inputActions;
        private bool isCanvas ;

        protected override void TriggerKeyDown()
        {
            stageText.text = stageName;
            if (Input.GetKeyDown(keyCode))
            {
                isCanvas = !isCanvas;
                StageCanvas.SetActive(isCanvas);
                if (isCanvas)
                {
                    AudioUtility.CreateSFX(triggerSound, transform.position, AudioUtility.AudioGroups.Sound);
                    inputActions.UnInputActions();
                }
                else
                {
                    inputActions.OnInputActions();
                }
            }
        }

    }
}
