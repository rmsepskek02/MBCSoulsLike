using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    public Material uiMaterial;

    void Start()
    {
        
      uiMaterial = GetComponent<Image>().material;
        uiMaterial.SetFloat("Scroll", 3f);


        Debug.Log(uiMaterial.GetFloat("Scroll"));
    }

    void Update()
    {
        //if (uiMaterial != null)
        //{
          
        //    uiMaterial.SetFloat("Scroll", 3f);
        //}
        //else
        //{
        //    Debug.Log("dde");
        //}
    }
}
