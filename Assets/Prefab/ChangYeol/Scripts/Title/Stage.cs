
using BS.Utility;
using UnityEngine;

namespace BS.Title
{
    public class Stage : StageTrigger
    {
        protected override void TriggerKeyDown()
        {
            stageText.text = stageName;
            if (Input.GetKeyDown(keyCode))
            {
                if (InstEnemy && !isEnemy)
                {
                    AudioUtility.CreateSFX(triggerSound, transform.position, AudioUtility.AudioGroups.Sound);
                    Enemy = Instantiate(InstEnemy, InstEnemy.transform.position, Quaternion.identity);
                    isEnemy = true;
                }
            }

        }
    }
}