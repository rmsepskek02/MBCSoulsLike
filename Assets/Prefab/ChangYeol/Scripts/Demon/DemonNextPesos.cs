using UnityEngine;

namespace BS.Demon
{
    public class DemonNextPhase : DemonController
    {
        #region Variables
        [HideInInspector]public float[] lastPesosTime = new float[3];
        [SerializeField]private float[] pesosAttackCool = new float[3];
        #endregion
        public override void NextPhase()
        {
            index = Random.Range(0, pesosDemon.Count);
            if (Time.time - lastPesosTime[index] >= pesosAttackCool[index])
            {
                switch (index)
                {
                    case 0:
                        ChangeState(pesosDemon[0]);
                        break;
                    case 1:
                        ChangeState(pesosDemon[1]);
                        break;
                    case 2:
                        ChangeState(pesosDemon[2]);
                        break;
                }
                return;
            }
            else
            {
                ChangeState(DEMON.Idle);
            }
        }
    }
}