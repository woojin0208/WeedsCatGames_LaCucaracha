using UnityEngine;
using UnityEngine.Audio;

// AudioMixer 볼륨 설정을 전역으로 관리한다.
// 슬라이더 값은 0~1 범위로 받고, AudioMixer에는 dB 값으로 변환해서 전달한다.
public class AudioSettingsManager : MonoBehaviour
{
    private const string MasterVolumeKey = "Audio_MasterVolume";
    private const string BGMVolumeKey = "Audio_BGMVolume";
    private const string SFXVolumeKey = "Audio_SFXVolume";

    private const string MasterVolumeParam = "MasterVolume";
    private const string BGMVolumeParam = "BGMVolume";
    private const string SFXVolumeParam = "SFXVolume";

    private const float MinVolume = 0.0001f;
    private const float MinDecibel = -80f;

    private static AudioSettingsManager instance;
    public static AudioSettingsManager Instance => instance;

    [SerializeField] private AudioMixer audioMixer;

    public float MasterVolume { get; private set; } = 1f;
    public float BGMVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        LoadVolumes();
        ApplyAllVolumes();
    }

    private void Start()
    {
        ApplyAllVolumes();
    }
    public void ApplySavedVolumes()
    {
        LoadVolumes();
        ApplyAllVolumes();
    }

    public void SetMasterVolume(float volume)
    {
        MasterVolume = Mathf.Clamp01(volume);
        ApplyVolume(MasterVolumeParam, MasterVolume);
        PlayerPrefs.SetFloat(MasterVolumeKey, MasterVolume);
    }

    public void SetBGMVolume(float volume)
    {
        BGMVolume = Mathf.Clamp01(volume);
        ApplyVolume(BGMVolumeParam, BGMVolume);
        PlayerPrefs.SetFloat(BGMVolumeKey, BGMVolume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        ApplyVolume(SFXVolumeParam, SFXVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, SFXVolume);
    }

    public void SaveVolumes()
    {
        PlayerPrefs.Save();
    }

    private void LoadVolumes()
    {
        MasterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        BGMVolume = PlayerPrefs.GetFloat(BGMVolumeKey, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
    }

    private void ApplyAllVolumes()
    {
        ApplyVolume(MasterVolumeParam, MasterVolume);
        ApplyVolume(BGMVolumeParam, BGMVolume);
        ApplyVolume(SFXVolumeParam, SFXVolume);
    }

    private void ApplyVolume(string parameterName, float volume)
    {
        if (audioMixer == null)
        {
            Debug.LogError("[AudioSettingsManager] AudioMixer가 할당되지 않았습니다.", this);
            return;
        }

        float decibel = volume <= MinVolume ? MinDecibel : Mathf.Log10(volume) * 20f;
        audioMixer.SetFloat(parameterName, decibel);
    }
}