
using UnityEngine;

namespace HumanFactory.Managers
{
    public class InputManager
    {
        public void OnUpdate()
        {
            OnGameScene();
        }

        private Vector3 worldPos;
        private Vector2Int curMousePos;
        private void OnGameScene()
        {
            if (GameManagerEx.Instance.CurrentCamType != CameraType.Game) return;

            if (Input.GetMouseButtonDown(0))
            {
                worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                curMousePos = new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.y));

                MapManager.Instance.OnClickMapGrid(curMousePos.x, curMousePos.y);
            }
        }

    }

}