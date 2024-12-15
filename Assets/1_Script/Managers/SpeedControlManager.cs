using UnityEngine;
using UnityEngine.UI;

public class SpeedControlManager : MonoBehaviour
{

    [SerializeField] private Button playButton; // 재생 버튼
    [SerializeField] private Button pauseButton; // 일시정지 버튼
    [SerializeField] private Button speedUp2Button; // 배속 버튼

    private float defaultTimeScale = 1f; // 기본 배속
    private float speedUpScale2 = 2f;    // 배속 값

    void Start()
    {
        // 버튼에 이벤트 리스너 추가
        playButton.onClick.AddListener(PlayGame);
        pauseButton.onClick.AddListener(PauseGame);
        speedUp2Button.onClick.AddListener(SpeedUp2Game);

        // 초기 상태 설정
        //Time.timeScale = 1f; // 게임 시작 시 1배속
    }

    private void PlayGame()
    {
        Time.timeScale = defaultTimeScale; // 기본 속도로 재생
        Debug.Log("게임 재생");
    }

    private void PauseGame()
    {
        Time.timeScale = 0f; // 시간 정지
        Debug.Log("게임 일시정지");
    }

    private void SpeedUp2Game()
    {
        Time.timeScale = speedUpScale2; // 배속 설정
        Debug.Log($"게임 배속: {speedUpScale2}x");
    }
}
