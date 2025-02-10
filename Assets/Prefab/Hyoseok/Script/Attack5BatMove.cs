using UnityEngine;
using DG.Tweening;

namespace BS.vampire
{
    public class Attack5BatMove : MonoBehaviour
    {
        #region Variables
        public float moveDistance = 10f;
        public float duration = 2f;
        public bool moveInZAxis = true; // true이면 z축, false이면 x축
        private Vector3 endPosition;
        #endregion

        private void Start()
        {
            if (moveInZAxis)
            {
                endPosition = transform.position + new Vector3(0, 0, moveDistance);
            }
            else
            {
                endPosition = transform.position + new Vector3(moveDistance, 0, 0);
            }

            MoveObject();
        }

        void MoveObject()
        {
            transform.DOMove(endPosition, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetLink(gameObject);
        }
    }
}
