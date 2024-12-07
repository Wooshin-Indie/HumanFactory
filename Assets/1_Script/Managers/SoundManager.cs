
using System;
using UnityEngine;

namespace HumanFactory.Manager
{
    /// <summary>
    /// 소리를 내는 매니저입니다.
    /// 게임 실행 시 동적으로 AudioSource를 생성합니다.
    /// audioClip 은 ResourceManager에서 동적으로 로드합니다.
    /// </summary>
    public class SoundManager
    {
        private AudioSource[] audioSources = new AudioSource[(int)Enum.GetNames(typeof(SoundType)).Length];
        
        public void init()
        {
            GameObject root = GameObject.Find("@SoundManager");
            if (root == null)
            {
                root = new GameObject { name = "@SoundManager" };
                UnityEngine.Object.DontDestroyOnLoad(root);

                string[] soundTypeNames = Enum.GetNames(typeof(SoundType));
                for(int i=0; i<soundTypeNames.Length; i++)
                {
                    GameObject go = new GameObject { name = soundTypeNames[i] };
                    audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = root.transform;
                }
            }

            audioSources[(int)SoundType.Bgm].loop = true;
        }

        // path들은 Constants에서 관리됩니다.
        public void PlaySound(SoundType type, string path)
        {
            // TODO - 사운드 재생
            // PlayOneShot 사용
        }

        public void ChangeBGM(string path)
        {

        }

    }
}