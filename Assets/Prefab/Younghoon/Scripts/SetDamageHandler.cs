using System.Collections.Generic;
using UnityEngine;

namespace BS.Enemy.Set
{
    // Renderer와 머티리얼 인덱스를 관리하기 위한 구조체
    [System.Serializable]
    public struct RendererIndexData
    {
        public Renderer renderer;
        public int metarialIndx;

        public RendererIndexData(Renderer _renderer, int index)
        {
            renderer = _renderer;
            metarialIndx = index;
        }
    }

    public class SetDamageHandler : MonoBehaviour
    {
        #region Variables

        //참조변수들
        private SetHealth health;

        #region 피격 효과
        public Material bodyMaterial; // 데미지를 줄 머티리얼
        [GradientUsage(true)] public Gradient hitEffectGradient; // 데미지 컬러 그라디언트 효과
        private List<RendererIndexData> bodyRenderers = new List<RendererIndexData>();
        private MaterialPropertyBlock bodyFlashMaterialPropertyBlock;
        private float lastTimeEffect; // 마지막으로 효과가 발생한 시간
        [SerializeField] private float flashDuration = 0.5f;
        private bool isFlashing; // 반짝거림 상태
        private Gradient currentEffectGradient; // 현재 적용 중인 그라디언트
        #endregion

        #endregion
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            health = GetComponent<SetHealth>();

            health.OnDamaged += TriggerEffect;
            // 반짝임 효과 초기화
            bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
            Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == bodyMaterial)
                    {
                        bodyRenderers.Add(new RendererIndexData(renderer, i));
                    }
                }
            }
        }

        void Update()
        {
            // 데미지 효과 업데이트
            if (isFlashing)
            {
                UpdateEffect();
            }
        }

        private void TriggerEffect(float damage)
        {
            lastTimeEffect = Time.time; // 효과 시작 시간 기록
            currentEffectGradient = hitEffectGradient; // 현재 적용 중인 그라디언트 설정
            isFlashing = true; // 반짝거림 시작
        }

        private void UpdateEffect()
        {
            // 반짝거림 지속 시간 계산
            float elapsed = Time.time - lastTimeEffect;

            if (elapsed > flashDuration)
            {
                // 반짝거림 효과 종료
                ResetMaterialProperties();
                return;
            }

            // 현재 시간에 따른 색상 계산
            Color currentColor = currentEffectGradient.Evaluate(elapsed / flashDuration);
            bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", currentColor);

            // 각 렌더러에 효과 적용
            foreach (var data in bodyRenderers)
            {
                data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.metarialIndx);
            }
        }

        private void ResetMaterialProperties()
        {
            // 반짝거림 효과를 초기화하여 원래 상태로 되돌림
            bodyFlashMaterialPropertyBlock.SetColor("_EmissionColor", Color.black);
            foreach (var data in bodyRenderers)
            {
                data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.metarialIndx);
            }
            isFlashing = false;
        }
    }
}