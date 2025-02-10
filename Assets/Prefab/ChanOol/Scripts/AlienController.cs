using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using BS.PlayerInput;
using BS.UI;

public class AlienController : MonoBehaviour
{
    #region Variables
    public GameObject boss;
    public GameObject player;
    public Camera mainCamera;
    public GameObject OpeningSequencerCamera;
    public DungeonClearTime dungeonClearTime;
    //public GameObject skipCanvas;

    #endregion

    private void Start()
    {
        StartCoroutine(ScriptsEnabledControll());
    }

    IEnumerator ScriptsEnabledControll()
    {
        AlienBossPattern alienBossPattern = boss.GetComponent<AlienBossPattern>();
        PlayerInputActions playerInputActions = player.GetComponent<PlayerInputActions>();
        //CinemachineSequencerCamera cinemachineCamera = sequencerCamera.GetComponent<CinemachineSequencerCamera>();

        //skipCanvas.SetActive(true);             // 스킵 캔버스 활성화
        alienBossPattern.enabled = false;       // 보스 패턴 끄기
        playerInputActions.UnInputActions();    // 플레이어 입력 끄기
        dungeonClearTime.enabled = false;       // 클리어 타임 끄기

        //여기서 2초 딜레이?
        yield return new WaitForSeconds(2.0f);  // FadeInOut 틀어지면

        OpeningSequencerCamera.SetActive(true); // 오프닝 카메라 활성화

        //cinemachineCamera.enabled = true;


        yield return new WaitForSeconds(8.0f);

        //skipCanvas.SetActive(false);             // 스킵 캔버스 비활성화
        alienBossPattern.enabled = true;         // 보스 패턴 켜기
        playerInputActions.OnInputActions();     // 플레이어 입력 켜기
        dungeonClearTime.enabled = true;         // 클리어 타임 켜기
        OpeningSequencerCamera.SetActive(false); // 시네머신 끄기

        //시네머신 끄기전 활성화 변수 

        mainCamera.fieldOfView = 60f;           // 메인 카메라 FOV값 60으로 설정



        //playerController.enabled = true;
    }
}

