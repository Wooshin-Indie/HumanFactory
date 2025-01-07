using HumanFactory.Manager;
using UnityEngine;
using UnityEngine.UI;

namespace HumanFactory
{
    public class SpeedControlManager : MonoBehaviour
    {

        [SerializeField] private Button playButton; // 재생 버튼
        [SerializeField] private Button pauseButton; // 일시정지 버튼
        [SerializeField] private Button speedUp2Button; // 배속 버튼
        [SerializeField] private Button stopButton; // 정지 버튼

        void Start()
        {
            // 버튼에 이벤트 리스너 추가
            playButton.onClick.AddListener(PlayGame);
            pauseButton.onClick.AddListener(PauseGame);
            speedUp2Button.onClick.AddListener(SpeedUp2Game);
            stopButton.onClick.AddListener(StopGame);

            // 초기 상태 설정
            //Time.timeScale = 1f; // 게임 시작 시 1배속
        }

        private void PlayGame()
        {
            MapManager.Instance.AddPerson();
            GameManagerEx.Instance.SetExeType(ExecuteType.Play);
            Debug.Log("게임 재생");
        }

        private void PauseGame()
        {
			GameManagerEx.Instance.SetExeType(ExecuteType.Pause);
			Debug.Log("게임 일시정지");
        }

        private void SpeedUp2Game()
        {
            MapManager.Instance.DoubleCycleTime();
			Debug.Log("게임 배속");
        }

        private void StopGame()
        {
            GameManagerEx.Instance.SetExeType(ExecuteType.None);
            Debug.Log("게임 정지");
        }
    }
}