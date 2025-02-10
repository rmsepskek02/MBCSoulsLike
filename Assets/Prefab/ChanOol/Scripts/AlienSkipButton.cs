using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlienSkipButton : MonoBehaviour
{
    private TextMeshProUGUI skipText;
    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skipText = GetComponentInChildren<TextMeshProUGUI>(); // GetComponentInChildren<>() : 현재 오브젝트의 모든 자식(손자 포함) 중에서 첫 번째 <컴포넌트> 를 찾기
        button = GetComponent<Button>();                      // GetComponent<>() : 현재 오브젝트의 <컴포넌트> 를 찾기
        skipText.color = Color.white;                         // TextMeshProUGUI.color : 텍스트 기본 색상 설정
        button.onClick.AddListener(SkipAction);               // Button.onClick.AddListener() : 버튼에 이벤트 추가
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPointerEnter()
    {
        skipText.color = Color.gray; // 마우스 오버 시 회색 변경
    }
    public void OnPointerExit()
    {
        skipText.color = Color.white; // 마우스 벗어나면 다시 흰색
    }
    public void SkipAction()
    {
        // 오프닝 스킵 동작을 여기에 추가
        Debug.Log("오프닝 스킵.");
    }
}
