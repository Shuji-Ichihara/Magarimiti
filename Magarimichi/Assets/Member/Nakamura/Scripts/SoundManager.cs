using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;

public class SoundManager : MonoBehaviour
{

    public static SoundManager instance;

    [SerializeField, Range(0.0f, 1.0f), Header("音源設定")]
    private float _bgmVolume = 0.1f;

    [SerializeField, Range(0.0f, 1.0f)]
    private float _seVolume = 0.1f;

    [SerializeField, Header("ミュート設定")]
    private bool _bgmMute = false;

    [SerializeField]
    private bool _seMute = false;

    [SerializeField, Header("BGM・SE音源")]
    private AudioClip[] _bgmClips;

    [SerializeField]
    private AudioClip[] _seClips;

    [SerializeField]
    private AudioSource _bgmSource;

    [SerializeField]
    private AudioSource _seSource;

    /*なんかこれ使うのオススメ！って言われたけどよくわからんからコメントアウト
    [SerializeField]
    private AudioMixerGroup _bgmMixer;

    [SerializeField]
    private AudioMixerGroup _seMixer;
    */

    private List<AudioSource> _seSources = new List<AudioSource>();


    void Awake()
    {
        //シングルトン処理
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        //_bgmSource.outputAudioMixerGroup = _bgmMixer;
        //_seSource.outputAudioMixerGroup = _seMixer;

    }

    /// <summary>
    /// BGMの再生のメソッド
    /// </summary>
    /// <param name="e_BGM">再生するBGMのenum</param>
    public void PlayBGM(E_BGM e_BGM, bool isFade = true, float fadeInTime = 3)
    {

        int index = (int)e_BGM;

        if (index < 0 || index >= _bgmClips.Length)
        {
            Debug.LogError("BGMのindexが範囲外：index=" + index);
            return;
        }

        if (_bgmSource.isPlaying == false)
        {
            Debug.Log("BGM再生");
            _bgmSource.clip = _bgmClips[index];
            _bgmSource.Play();
            if (isFade)
            {
                FadeIn(_bgmSource, fadeInTime).Forget();
            }
        }
        else
        {
            return;
        }

    }

    /// <summary>
    /// SEの再生のメソッド
    /// </summary>
    /// <param name="e_SE">再生するSEのenum</param>
    public void PlaySE(E_SE e_SE)
    {

        AudioSource newAudioSource = gameObject.AddComponent<AudioSource>();
        _seSources.Add(newAudioSource);

        int index = (int)e_SE;

        if (index < 0 || index >= _seClips.Length)
        {
            Debug.LogError("SEのindexが範囲外：index=" + index);
            return;
        }

        newAudioSource.clip = _seClips[index];
        newAudioSource.Play();

        StartCoroutine(WaitAndDestroy(newAudioSource));
    }

    private IEnumerator WaitAndDestroy(AudioSource audioSource)
    {
        while (audioSource.isPlaying)
        {
            yield return null;
        }

        _seSources.Remove(audioSource);
        Destroy(audioSource);
    }

    /// <summary>
    /// BGMの一時停止用のメソッド
    /// </summary>
    public void BGMPause()
    {
        if (_bgmSource.isPlaying)
        {
            _bgmSource.Pause();
        }
        else if (!_bgmSource.isPlaying)
        {
            _bgmSource.UnPause();
        }

    }

    /// <summary>
    /// BGM音量を調整するためのメソッド
    /// </summary>
    /// <param name="volume">セットする音量</param>
    public void SetBGMVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        _bgmVolume = volume;
        _bgmSource.volume = _bgmMute ? 0 : _bgmVolume;
    }

    /// <summary>
    /// SE音量を調整するためのメソッド
    /// </summary>
    /// <param name="volume">セットする音量</param>
    public void SetSEVolume(float volume)
    {
        volume = Mathf.Clamp(volume, 0.0f, 1.0f);
        _seVolume = volume;
        foreach (AudioSource seSource in _seSources)
        {
            seSource.volume = _seMute ? 0 : _seVolume;
        }
    }

    /// <summary>
    /// BGMのミュートを切り替えるためのメソッド
    /// </summary>
    public void ToggleBGMMute()
    {
        _bgmMute = !_bgmMute;
        _bgmSource.volume = _bgmMute ? 0 : _bgmVolume;
    }

    /// <summary>
    /// SEのミュートを切り替えるためのメソッド
    /// </summary>
    public void ToggleSEMute()
    {
        _seMute = !_seMute;
        foreach (AudioSource seSource in _seSources)
        {
            seSource.volume = _seMute ? 0 : _seVolume;
        }
    }

    /// <summary>
    /// フェードインのメソッド
    /// </summary>
    /// <param name="audio">対象のAudioSorce　入ってなかったら再生するAudioSorce</param>
    /// <param name="fadeTime">フェードの時間</param>
    public async UniTask FadeIn(AudioSource audio = null, float fadeTime = 3)
    {
        if (audio == null)
        {
            audio = _bgmSource;
        }

        audio.volume = 0;
        while (audio.volume < 1)
        {
            float deltafadeTime = Time.deltaTime / fadeTime;
            audio.volume = Mathf.Min(audio.volume + deltafadeTime, 1);
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// フェードアウトのメソッド
    /// </summary>
    /// <param name="audio">対象のAudioSorce 入ってなかったら再生中のAudioSorce</param>
    /// <param name="fadeTime">フェード時間</param>
    public async UniTask FadeIOut(AudioSource audio = null, float fadeTime = 3)
    {
        //audioがnullの場合はBGMのAudioSourceを使用する(多分これしか使わないだろ…多分)
        if (audio == null)
        {
            audio = _bgmSource;
        }

        while (audio.volume > 0)
        {
            float deltafadeTime = Time.deltaTime / fadeTime;
            audio.volume = Mathf.Max(audio.volume - deltafadeTime, 0);
            await UniTask.DelayFrame(1);
        }
    }

    // BGM・SEのEnum（これいちいち定義するのめんどくさすぎる）
    public enum E_BGM
    {
        /*
        BGM01,
        BGM02,
        BGM03
        */
        OutGameBGM,
        InGameBGM,
    }

    public enum E_SE
    {
        /*
        SE01,
        SE02,
        SE03
        */
        GetKey,
        MovePlayer,
        ChooseMapChip,
        ReleaseMapChip,
        Cancel,
        TapButton,
    }

}
