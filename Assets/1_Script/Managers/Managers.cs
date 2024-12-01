using UnityEngine;

namespace HumanFactory.Managers
{
    /*
     * Manager 들을 관리하는 Manager입니다.
     * 사용할 때에는 Managers.[Property].[Func] 처럼 사용하면 됩니다.
     * [Property] 에 해당하는 Manager들은 MonoBehaviour를 상속받을 수 없습니다.
     * 따라서, Editor로부터 값을 받는게 안됩니다.
     * 1. 따로 MonoBehavior를 상속받고 싱글톤을 적용하던지,
     * 2. 해당 클래스에서 값을 받고 사용하시면 됩니다.
     */
    public class Managers : MonoBehaviour
    {
        static Managers s_instance;
        public static Managers Instance { get { return s_instance; } }

        /** Managers **/
        private ResourceManager _resource = new ResourceManager();
        private DataManager _data = new DataManager();

        /** Properties **/
        public static ResourceManager Resource { get { return Instance._resource; } }
        public static DataManager Data { get { return Instance._data; } }

        public void Init()
        {
            if (s_instance == null)
            {
                GameObject go = GameObject.Find("@Managers");
                if (go == null)
                {
                    go = new GameObject { name = "@Managers" };
                    go.AddComponent<Managers>();
                }

                DontDestroyOnLoad(go);
                s_instance = go.GetComponent<Managers>();

            }
            else
            {
                Destroy(this.gameObject);
                return;
            }

            s_instance._resource.Init();
            s_instance._data.Init();
        }


        void Awake()
        {
            Init();
        }

    }
}