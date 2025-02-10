using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject audioCanvas;
    private bool toggleSwitch = false;

    private void Start()
    {
        audioCanvas = transform.Find("Audio").gameObject;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Toggle();
        }
    }

    private void Toggle()
    {
        toggleSwitch = !toggleSwitch;
        audioCanvas.SetActive(toggleSwitch);
    }
}
