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

        private int currentBGM = (int)BGMType.None;


        /** Properties **/
        public float SfxVolume
        {
            get { return audioSources[(int)SoundType.Sfx].volume; }
            set
            {
                audioSources[(int)SoundType.Sfx].volume = value;
                Managers.Data.BasicSettingData.SfxVolume = value;
            }
        }
        public float BgmVolume
        {
            get { return audioSources[(int)SoundType.Bgm].volume; }
            set
            {
                audioSources[(int)SoundType.Bgm].volume = value;
                Managers.Data.BasicSettingData.BgmVolume = value;
            }
        }
        public int CurrentBGM { get { return currentBGM; } }


        public void Init()
        {
            GameObject root = GameObject.Find("@SoundManager");
            if (root == null)
            {
                root = new GameObject { name = "@SoundManager" };
                UnityEngine.Object.DontDestroyOnLoad(root);

                string[] soundTypeNames = Enum.GetNames(typeof(SoundType));
                for (int i = 0; i < soundTypeNames.Length; i++)
                {
                    GameObject go = new GameObject { name = soundTypeNames[i] };
                    audioSources[i] = go.AddComponent<AudioSource>();
                    go.transform.parent = root.transform;
                }
            }

            audioSources[(int)SoundType.Bgm].loop = true;
        }


        // path들은 Constants에서 관리됩니다.
        public void PlaySfx(SFXType sfxType, float volume = 1f)
        {
			audioSources[(int)SoundType.Sfx].PlayOneShot(
                Resources.Load<AudioClip>(Constants.PATH_SFX + sfxType.ToString()), volume);
		}

        // TODO - 소리 바꿀때 치지직- 이런 효과음 있으면 좋을듯?
        // TODO - BGM 정해야됨
        public void ChangeBGM(bool isNext)
        {
            int count = Enum.GetNames(typeof(BGMType)).Length;
            currentBGM = isNext ? ((currentBGM + 1) % count)
                      : (currentBGM - 1 < 0 ? count - 1 : currentBGM - 1);

            if (currentBGM == (int)BGMType.None)
            {
                audioSources[(int)SoundType.Bgm].Stop();
                return;
            }

            audioSources[(int)SoundType.Bgm].clip = Managers.Resource.GetBGM((BGMType)currentBGM);

            if (!audioSources[(int)SoundType.Bgm].isPlaying)
                audioSources[(int)SoundType.Bgm].Play();
        }

    }
}