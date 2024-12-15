using System.Collections;
using System.Collections.Generic;
using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

public class SpeedControlManager : MonoBehaviour
{
    //public Button PlayButton;   // 재생 버튼
    //public Button PauseButton;  // 일시정지 버튼
    //public Button SpeedUp2Button; // 배속 버튼
    [SerializeField] private GameObject PlayButton; // 재생 버튼
    [SerializeField] private GameObject PauseButton; // 일시정지 버튼
    [SerializeField] private GameObject SpeedUp2Button; // 배속 버튼

    public static float currentScale;    // 현재 배속
    private float defaultTimeScale = 1f; // 기본 배속
    private float speedUpScale2 = 2f;    // 배속 값

    void Start()
    {
        // 버튼에 이벤트 리스너 추가
        PlayButton.GetComponent<Button>().onClick.AddListener(PlayGame);
        PauseButton.GetComponent<Button>().onClick.AddListener(PauseGame);
        SpeedUp2Button.GetComponent<Button>().onClick.AddListener(SpeedUp2Game);

        // 초기 상태 설정
        //Time.timeScale = 1f; // 게임 시작 시 1배속
        currentScale = 0f; //초기 값 0
    }

    public void PlayGame()
    {
        Time.timeScale = defaultTimeScale; // 기본 속도로 재생
        Debug.Log("게임 재생");
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // 시간 정지
        Debug.Log("게임 일시정지");
    }

    public void SpeedUp2Game()
    {
        Time.timeScale = speedUpScale2; // 배속 설정
        Debug.Log($"게임 배속: {speedUpScale2}x");
    }
}
