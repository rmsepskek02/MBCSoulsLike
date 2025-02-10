using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using System.Reflection;
using System.Collections;

public class TitleToggleRendererFeature : MonoBehaviour
{
    [SerializeField] float timer = 10;
    [SerializeField] string renderName;
    [SerializeField] string metName;

    /* 사용 샘플 코드
     SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false); 
     */
    private void Start()
    {
        SetActiveRendererFeature<ScriptableRendererFeature>(renderName, false);

    }
    public void SetActiveRendererFeature<T>(string featureName, bool active) where T : ScriptableRendererFeature
    {

        // URP Asset의 Renderer List에서 0번 인덱스 RendererData 참조
        ScriptableRendererData rendererData = GetRendererData(0);
        if (rendererData == null) return;

        List<ScriptableRendererFeature> rendererFeatures = rendererData.rendererFeatures;
        if (rendererFeatures == null || rendererFeatures.Count <= 0) return;

        foreach (var rendererFeature in rendererFeatures)
        {
            if (rendererFeature == null) continue;
            if (rendererFeature is T && rendererFeature.name == featureName)
            {
                rendererFeature.SetActive(active);
                break;
            }
        }
#if UNITY_EDITOR
        rendererData.SetDirty();
#endif
    }

    public ScriptableRendererData GetRendererData(int rendererIndex = 0)
    {
        // 현재 Quality 옵션에 세팅된 URP Asset 참조
        UniversalRenderPipelineAsset pipelineAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (!pipelineAsset) return null;

        // URP Renderer List 리플렉션 참조 (Internal 변수라서 그냥 참조 불가능)
        FieldInfo propertyInfo = pipelineAsset.GetType().GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);
        ScriptableRendererData[] rendererDatas = (ScriptableRendererData[])propertyInfo.GetValue(pipelineAsset);
        if (rendererDatas == null || rendererDatas.Length <= 0) return null;
        if (rendererIndex < 0 || rendererDatas.Length <= rendererIndex) return null;

        return rendererDatas[rendererIndex];
    }

    IEnumerator StopRender()
    {
        yield return new WaitForSeconds(timer);
        SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false);
    }
}
