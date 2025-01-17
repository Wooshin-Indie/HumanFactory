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
        [SerializeField] private Button oneCycleButton; // 1사이클 실행 버튼

        void Start()
        {
            // 버튼에 이벤트 리스너 추가
            playButton.onClick.AddListener(PlayGame);
            pauseButton.onClick.AddListener(PauseGame);
            speedUp2Button.onClick.AddListener(SpeedUp2Game);
            stopButton.onClick.AddListener(StopGame);
            oneCycleButton.onClick.AddListener(OneCycleGame);

            // 초기 상태 설정
            //Time.timeScale = 1f; // 게임 시작 시 1배속
        }

        private void PlayGame() // 게임 재생
        {
            MapManager.Instance.IsOneCycling = false;
            MapManager.Instance.AddPersonWith1x();
            GameManagerEx.Instance.SetExeType(ExecuteType.Play);
            Debug.Log("게임 재생");
        }

        private void PauseGame() // 게임 일시정지
        {
			GameManagerEx.Instance.SetExeType(ExecuteType.Pause);
        }

        private void SpeedUp2Game() // 게임 2배속
        {
            MapManager.Instance.DoubleCycleTime();
        }

        private void StopGame() // 게임 리셋
        {
            MapManager.Instance.ClearHumans();
            MapManager.Instance.IdxIn = 0;
            MapManager.Instance.IdxOut = 0;
            GameManagerEx.Instance.SetExeType(ExecuteType.None);
        }

        private void OneCycleGame() // 1사이클씩 실행
        {
            MapManager.Instance.IsOneCycling = true;
            Debug.Log("isPersonAdd:" + MapManager.Instance.IsPersonAdd + ", idxIn" + MapManager.Instance.IdxIn);
            MapManager.Instance.AddPersonWithOneCycling();
            GameManagerEx.Instance.SetExeType(ExecuteType.Play);
        }
    }
}