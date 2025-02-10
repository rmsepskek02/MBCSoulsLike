using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonsSoundPlay : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hoverClip;
    public AudioClip clickClip;

    public Button[] buttons;

    void Start()
    {
        foreach (Button button in buttons)
        {
            button.onClick.AddListener(() => PlayClickSound());
  
            EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

            //호버사운드
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => PlayHoverSound());
            trigger.triggers.Add(entry);
        }
    }

    void PlayHoverSound()
    {
        audioSource.PlayOneShot(hoverClip);
    }

    void PlayClickSound()
    {
        audioSource.PlayOneShot(clickClip);
    }
}


