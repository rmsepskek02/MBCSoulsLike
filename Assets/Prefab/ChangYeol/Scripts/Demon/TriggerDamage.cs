using BS.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BS.Demon
{
    public class TriggerDamage : MonoBehaviour
    {
        #region Variables
        private HashSet<GameObject> damagedObjects = new HashSet<GameObject>();
        public Collider triggerCollider;
        public int damageAmount = 1;
        public GameObject StunEffect;
        #endregion
        private void Start()
        {
            if (triggerCollider)
            {
                StartCoroutine(TriggerOn());
            }
        }
        void OnTriggerEnter(Collider other)
        {
            if (DemonController.isDie) return;
            // 자식 객체에서 PlayerHealth 컴포넌트를 찾음
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if(playerHealth != null )
            {
                PlayerState state = playerHealth.GetComponent<PlayerState>();
                if (state.isBlockable) return;
            }
            if (playerHealth != null && !damagedObjects.Contains(other.gameObject))
            {
                Debug.Log($"{damageAmount}만큼 데미지 입음");
                playerHealth.TakeDamage(damageAmount, false);
                damagedObjects.Add(other.gameObject);
                StartCoroutine(ResetCollision(other.gameObject));
            }
        }
        // 일정 시간 후 충돌 정보 리셋
        IEnumerator ResetCollision(GameObject other)
        {
            yield return new WaitForSeconds(0.5f);
            damagedObjects.Remove(other);
        }
        IEnumerator TriggerOn()
        {
            if(triggerCollider)
            {
                triggerCollider.enabled = false;
                yield return new WaitForSeconds(0.5f);
                triggerCollider.enabled = true;
                yield return new WaitForSeconds(0.5f);
                triggerCollider.enabled = false;
            }
        }
        //IEnumerator PlayerStun(PlayerController controller, PlayerHealth player, float Time)
        //{
        //    PlayerStateMachine playerState = controller.GetComponent<PlayerStateMachine>();
        //    PlayerInputActions inputActions = playerState.GetComponent<PlayerInputActions>();
        //    inputActions.MousePositionInput(player.transform.position);
        //    inputActions.UnInputActions();
        //    effectpos = new Vector3(player.transform.position.x, StunEffect.transform.position.y, player.gameObject.transform.position.z);
        //    if (isstuning)
        //    {
        //        GameObject stun = Instantiate(StunEffect, effectpos, Quaternion.identity);
        //        playerState.ChangeState(playerState.IdleState);
        //        Debug.Log("Stun");
        //        yield return new WaitForSeconds(Time);
        //        Destroy(stun, 0.2f);
        //        isstuning = false;
        //    }
        //    Destroy(this.gameObject, 0.2f);
        //    inputActions.OnInputActions();
        //}
    }
}