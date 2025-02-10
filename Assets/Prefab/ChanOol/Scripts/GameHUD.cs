using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.Rendering.Universal;

public class GameHUD : MonoBehaviour
{
    public BS.Managers.SceneManager Manager;
    public GameObject gameHUD;
    public GameObject pauseCanvas;
    public GameObject soundCanvas;
    public Button continueButton;
    public Button restartButton;
    public Button soundButton; // Sound 버튼
    public Button backButton; // SoundCanvas에서 돌아가는 버튼
    public Button quitToShelterButton; // Quit to Shelter 버튼 추가
    public Button resetDefaultButton; // Reset Default 버튼 추가
    public Slider masterSlider;
    public Slider soundSlider;
    public Slider musicSlider;

    private bool isPaused = false;

    //렌더러
     private ToggleRendererFeature toggleRendererFeature;

    private void Awake()
    {
        {
            // ToggleRendererFeature가 연결된 게임 오브젝트를 찾습니다.
            GameObject obj = GameObject.Find("NameOfObjectWithToggleRendererFeature");

            if (obj != null)
            {
                // ToggleRendererFeature 스크립트의 인스턴스를 가져옵니다.
                toggleRendererFeature = obj.GetComponent<ToggleRendererFeature>();
            }
        }
    }
    void Start()
    {
        // 비활성화된 상태로 시작
        pauseCanvas.SetActive(false);
        soundCanvas.SetActive(false);

        // Continue 버튼 클릭 이벤트 연결
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueButtonClick);
        }

        // Restart 버튼 클릭 이벤트 연결
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(OnRestartButtonClick);
        }

        // Sound 버튼 클릭 이벤트 연결
        if (soundButton != null)
        {
            soundButton.onClick.AddListener(OnSoundButtonClick);
        }

        // Back 버튼 클릭 이벤트 연결
        if (backButton != null)
        {
            backButton.onClick.AddListener(OnBackButtonClick);
        }

        // Quit to Shelter 버튼 클릭 이벤트 연결
        if (quitToShelterButton != null)
        {
            quitToShelterButton.onClick.AddListener(OnQuitToShelterButtonClick);
        }

        // Reset Default 버튼 클릭 이벤트 연결
        if (resetDefaultButton != null)
        {
            resetDefaultButton.onClick.AddListener(OnResetDefaultButtonClick);
        }
    }

    void Update()
    {
        // ESC 키를 눌렀을 때 Pause 상태 변경
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    // Pause 상태를 토글하는 함수
    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0 : 1;
    }

    // Continue 버튼 클릭 시 호출되는 함수
    public void OnContinueButtonClick()
    {
        // Pause 해제
        isPaused = false;
        pauseCanvas.SetActive(false);
        Time.timeScale = 1;
    }

    // Restart 버튼 클릭 시 호출되는 함수
    public void OnRestartButtonClick()
    {
        //렌더러 초기화
        if (toggleRendererFeature != null)
        {
            // SetActiveRendererFeature 메서드를 호출하여 렌더러 기능을 활성화/비활성화합니다.
            toggleRendererFeature.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false);
        }

        // DOTween.KillAll(); // 모든 Tween 정리
        Time.timeScale = 1; // 타임스케일 초기화
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // Sound 버튼 클릭 시 호출되는 함수
    public void OnSoundButtonClick()
    {
        // pauseCanvas 비활성화 및 soundCanvas 활성화
        pauseCanvas.SetActive(false);
        soundCanvas.SetActive(true);
    }

    // Back 버튼 클릭 시 호출되는 함수
    public void OnBackButtonClick()
    {
        // soundCanvas 비활성화 및 pauseCanvas 활성화
        soundCanvas.SetActive(false);
        pauseCanvas.SetActive(true);
    }

    // Quit to Shelter 버튼 클릭 시 호출되는 함수
    public void OnQuitToShelterButtonClick()
    {
        // 타임스케일 초기화 후 Shelter 씬 로드
        //렌더러 초기화
        if (toggleRendererFeature != null)
        {
            // SetActiveRendererFeature 메서드를 호출하여 렌더러 기능을 활성화/비활성화합니다.
            toggleRendererFeature.SetActiveRendererFeature<ScriptableRendererFeature>("FullScreenOpening", false);
        }
        Time.timeScale = 1;
        if(Manager) Manager.ResetPlayer();
        SceneManager.LoadScene("Shelter");
    }

    // Reset Default 버튼 클릭 시 호출되는 함수
    public void OnResetDefaultButtonClick()
    {
        if (masterSlider != null)
        {
            masterSlider.value = 1; // MasterSlider의 값을 1로 설정
            soundSlider.value = 1;
            musicSlider.value = 1;

        }
    }
}
