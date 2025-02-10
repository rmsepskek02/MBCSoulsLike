using BS.Player;
using UnityEngine;
using UnityEngine.Events;

namespace BS.Enemy.Set
{
    public class AttackTrigger : MonoBehaviour
    {
        private bool hitCheck;
        [SerializeField] float damage = 10;
        public UnityAction OnBlocked;

        private void OnEnable()
        {
            hitCheck = false;
        }
        private void OnTriggerEnter(Collider other)
        {
            if (hitCheck) return;

            // 충돌한 오브젝트의 레이어가 "Player"인지 확인
            if (other.gameObject.layer == LayerMask.NameToLayer(SetProperty.PLAYER_LAYER))
            {
                // 플레이어에게 데미지를 주는 로직 처리
                hitCheck = true;

                //TODO : 데미지를 주고 필요시 넉백, 경직 구현
                PlayerHealth playerHealth = other.GetComponentInChildren<PlayerHealth>();
                if (playerHealth != null)
                {
                    bool isAttackFailed = playerHealth.TakeDamage(damage);
                    //TODO : 블락당하면 데미지 증폭여부 연결
                    if (isAttackFailed)
                    {
                        OnBlocked?.Invoke();
                    }
                }
                else
                {
                    Debug.Log("PlayerHealth를 찾을 수 없습니다");
                }
            }
        }
    }
}