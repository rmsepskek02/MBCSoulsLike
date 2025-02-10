using BS.Utility;
using System.Collections;
using UnityEngine;

namespace BS.vampire
{
    public class PattonSummon : MonoBehaviour
    {
        public AudioClip shildSummonSound;
        public AudioClip shildBreakSound;
        public GameObject batPrefab;
        public GameObject summonEffect;
        public GameObject shildEffect;
        public Transform[] summonLocations; // 소환 위치
        public Transform effectLocation; // 소환이펙트 위치
        public Transform shildLocation; // 쉴드 위치
        public float summonInterval = 5f; // 소환 간격 

        private GameObject currentShild; // 현재 쉴드 상태
        public VampireHealth boss;
        public float summonTimeRemaining; //남은 소환시간

        void Start()
        {
            if (boss == null)
            {
                boss = FindAnyObjectByType<VampireHealth>();
            }
            Summon();
            StartCoroutine(SummonBat());
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                boss.currentHealth =0;
            }
            
            CheckShieldStatus();
        }

        IEnumerator SummonBat()
        {
            while (true)
            {

                summonTimeRemaining = summonInterval;
                while (summonTimeRemaining > 0)
                {
                    summonTimeRemaining -= Time.deltaTime;
                    yield return null;
                }
                Summon();
            }
        }

        void Summon()
        {
            bool Summoned = false;
            foreach (Transform location in summonLocations)
            {
                if (location.childCount == 0)
                {
                    GameObject bat = Instantiate(batPrefab, location.position, location.rotation, location);
                    Summoned = true;
                }
            }
            if (Summoned)
            {
                GameObject effectGo = Instantiate(summonEffect, effectLocation.position, effectLocation.rotation);
                effectGo.transform.parent = effectLocation.transform;
                Destroy(effectGo, 3f);
            }
        }

        void CheckShieldStatus()
        {
            bool batExist = false;
            foreach (Transform location in summonLocations)
            {
                if (location.childCount > 0)
                {
                    batExist = true;
                    break;
                }
            }
            shild(batExist);
        }

        void shild(bool batExist)
        {
            if (batExist)
            {
                if (currentShild == null)
                {
                    AudioUtility.CreateSFX(shildSummonSound, transform.position, AudioUtility.AudioGroups.Sound);
                    currentShild = Instantiate(shildEffect, shildLocation.position, shildLocation.rotation);
                    currentShild.transform.parent = shildLocation.transform;
                    boss.SetInvincible(true);
                }
            }
            else
            {
                if (currentShild != null)
                {
                    AudioUtility.CreateSFX(shildBreakSound, transform.position, AudioUtility.AudioGroups.Sound);
                    Destroy(currentShild);
                    currentShild = null;
                    boss.SetInvincible(false);
                }
            }
        }
    }
}
