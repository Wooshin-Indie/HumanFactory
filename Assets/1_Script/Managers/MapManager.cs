using UnityEngine;

namespace HumanFactory.Managers
{

    public class MapGrid
    {
        private GridType type = 0;
        private PadType padType = 0;
        private BuildingType buildingType = 0;

        public GridType Type {  get => type; set => type = value; }
        public PadType PadType { get => padType; set => padType = value; }
        public BuildingType BuildingType { get => buildingType; set => buildingType = value; }

        public MapGrid()
        {
            type = GridType.Empty;
        }

        public void GetPadParameter(out int dir)
        {
            dir = (int)padType;
        }
    }

    public class MapManager : MonoBehaviour
    {
        #region Singleton
        private static MapManager instance;
        public static MapManager Instance { get { return instance; } }

        private void Init()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
                return;
            }
        }
        #endregion

        private void Awake()
        {
            Init();
        }

        private MapGrid[,] programMap;
        public MapGrid[,] ProgramMap { get => programMap; }

        [SerializeField] private Vector2Int mapSize;

        private void Start()
        {
            programMap = new MapGrid[mapSize.x, mapSize.y];
            for (int i = 0; i < mapSize.x; i++)
            {
                for (int j = 0; j < mapSize.y; j++)
                {
                    programMap[i, j] = new MapGrid();
                }
            }

            // HACK - Temproary testing Pad (direction change)
            programMap[2, 0].Type = GridType.Pad;
            programMap[2, 0].PadType = PadType.DirRight;
            programMap[4, 0].Type = GridType.Pad;
            programMap[4, 0].PadType = PadType.DirLeft;
            programMap[0, 1].Type = GridType.Pad;
            programMap[0, 1].PadType = PadType.DirRight;
            programMap[4, 1].Type = GridType.Pad;
            programMap[4, 1].PadType = PadType.DirUp;
            programMap[2, 2].Type = GridType.Pad;
            programMap[2, 2].PadType = PadType.DirDown;
            programMap[4, 2].Type = GridType.Pad;
            programMap[4, 2].PadType = PadType.DirLeft;
            programMap[0, 4].Type = GridType.Pad;
            programMap[4, 4].Type = GridType.Pad;

        }

    }
}