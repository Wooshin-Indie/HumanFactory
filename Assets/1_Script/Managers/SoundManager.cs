using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

		private float masterVolume = 1f;
		private float sfxVolume = 1f;
		private float bgmVolume = 1f;


		/** Properties **/
		public float SfxVolume
        {
            get { return sfxVolume; }
            set
            {
				sfxVolume = value;
                Managers.Data.BasicSettingData.sfxVolume = value;
            }
        }
        public float BgmVolume
        {
            get { return bgmVolume; }
            set
            {
				bgmVolume = value;
				audioSources[(int)SoundType.Bgm].volume = masterVolume * bgmVolume;
				audioSources[(int)SoundType.Noise].volume = masterVolume * bgmVolume;
                Managers.Data.BasicSettingData.bgmVolume = value;
            }
        }
        public float MasterVolume { 
			get => masterVolume; 
			set 
			{
				masterVolume = value;
				audioSources[(int)SoundType.Bgm].volume = masterVolume * bgmVolume;
				audioSources[(int)SoundType.Noise].volume = masterVolume * bgmVolume;
				Managers.Data.BasicSettingData.masterVolume = value;
			} 
		}
        public int CurrentBGM { get { return currentBGM; } }

        private GameObject root;

		public void Init()
        {
            root = GameObject.Find("@SoundManager");
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
			audioSources[(int)SoundType.Noise].loop = true;
			audioSources[(int)SoundType.Noise].clip = Resources.Load<AudioClip>("Sounds/Whitenoise/Humming");
			audioSources[(int)SoundType.Noise].Play();
            InitPool();
        }
        

		#region Object Pooling

		private Transform poolRoot;
		private Stack<AudioSource> poolingAudios = new Stack<AudioSource>();

		private void InitPool(int cnt = 10)
		{
			poolRoot = new GameObject { name = "_poolRoot" }.transform;
			poolRoot.parent = root.transform;

			for (int i = 0; i < cnt; i++)
			{
				poolingAudios.Push(Create());
			}
		}

		private AudioSource Create()
		{
            GameObject go = new GameObject { name = "PoolableAudio" };
			go.AddComponent<AudioSource>();
			go.transform.parent = poolRoot;
			go.gameObject.SetActive(false);
			return go.GetComponent<AudioSource>();
		}
		private void Push(AudioSource source)
		{
			source.gameObject.SetActive(false);
			poolingAudios.Push(source);
		}
		private AudioSource Pop()
		{
			AudioSource source;
			if (poolingAudios.Count == 0) source = Create();
			else source = poolingAudios.Pop();

			source.gameObject.SetActive(true);
			return source;
		}

		private async Task PushAfterDelay(AudioSource source, float delay)
		{
            await Task.Delay((int)(delay * 1000));
			Push(source);
		}


		#endregion

		public void PlaySfx(SFXType sfxType)
		{
            PlaySfx(sfxType, 1f);
		}

		public void PlaySfx(SFXType sfxType, float volume)
        {
			audioSources[(int)SoundType.Sfx].PlayOneShot(GetAudioClip(sfxType) , volume * sfxVolume * masterVolume);
		}

        public AudioSource PlaySfx(SFXType sfxType, float volume, float pitch)
		{
			AudioSource audioSource = Pop();
            audioSource.clip = GetAudioClip(sfxType);
            audioSource.pitch = pitch;
			audioSource.volume = volume * masterVolume * sfxVolume;
			audioSource.Play();
            PushAfterDelay(audioSource, audioSource.clip.length);
			return audioSource;
		}

        public AudioSource PlaySfx(SFXType sfxType, float volume, float pitch, float duration)
		{
			AudioSource audioSource = Pop();
			audioSource.clip = GetAudioClip(sfxType);
			audioSource.pitch = pitch;
			audioSource.volume = volume * masterVolume * sfxVolume;
			audioSource.Play();
			PushAfterDelay(audioSource, Mathf.Min(audioSource.clip.length, duration));
            return audioSource;
		}

        private AudioClip GetAudioClip(SFXType type)
        {
            return Resources.Load<AudioClip>(Constants.PATH_SFX + type.ToString());
		}

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