using BS.Achievement;
using BS.Utility;
using UnityEngine;

namespace BS.Enemy.Set
{
    public class SetDeathState : ISetState
    {
        private SetProperty property;

        public SetDeathState(SetProperty property)
        {
            this.property = property;
        }

        public void Enter()
        {
            property.Animator.SetBool(SetProperty.SET_ANIM_BOOL_DEAD, true);

            //죽을때 내는 SFX
            AudioUtility.CreateSFX(property.Controller.bossDieSFX, property.Controller.transform.position,
                AudioUtility.AudioGroups.Sound, 0, 1, 15, 0.5f); ;
            property.Controller.GetComponent<BoxCollider>().enabled = false;
            AchievementManager.Instance.UpdateAchievement(AchievementType.KillCount, 1);
            // NavMeshAgent 멈추기
            //property.Agent.isStopped = true;
        }

        public void Update()
        {
            //Empty
        }

        public void Exit()
        {
            //Empty
        }


    }
}