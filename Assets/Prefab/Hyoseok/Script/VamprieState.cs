using UnityEngine;

namespace BS.vampire
{
    public class VamprieState : MonoBehaviour
    {
        #region Variables
        public float health;
        private bool isInvincible = false; //公利咯何
        #endregion

        public void SetInvincible(bool invincible)
        {
            isInvincible = invincible;
        }

        public void TakeDamage(int damage)
        {
            if (!isInvincible)
            {
                health -= damage;
                if (health < 0)
                {
                    Die();
                }
            }
        }

        private void Die()
        {
            // 焊胶 荤噶贸府
        }
    }
}