using UnityEngine;
using UnityEngine.UI;

// 설정 UI의 볼륨 슬라이더와 AudioSettingsManager를 연결한다.
public class AudioSetting : MonoBehaviour
{
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider bgmVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private bool isPauseInputSubscribed;

    private void OnEnable()
    {
        SetInputEventSubscription(true);

        AudioSettingsManager manager = AudioSettingsManager.Instance;
        if (manager == null)
        {
            Debug.LogError("[AudioSetting] AudioSettingsManager를 찾을 수 없습니다.", this);
            return;
        }

        manager.ApplySavedVolumes();

        SetSliderValueWithoutNotify(masterVolumeSlider, manager.MasterVolume);
        SetSliderValueWithoutNotify(bgmVolumeSlider, manager.BGMVolume);
        SetSliderValueWithoutNotify(sfxVolumeSlider, manager.SFXVolume);
    }
    private void OnDisable()
    {
        SetInputEventSubscription(false);
        AudioSettingsManager.Instance?.SaveVolumes();
    }
    private void SetInputEventSubscription(bool isSubscribe)
    {
        InputStateManager manager = InputStateManager.Instance;
        if (manager == null) return;

        if (isSubscribe)
        {
            manager.PauseToggleRequested -= HandlePauseToggleRequested;
            manager.PauseToggleRequested += HandlePauseToggleRequested;
            isPauseInputSubscribed = true;
        }
        else
        {
            manager.PauseToggleRequested -= HandlePauseToggleRequested;
            isPauseInputSubscribed = false;
        }
    }

    private void HandlePauseToggleRequested()
    {
        if (!isPauseInputSubscribed || !gameObject.activeInHierarchy) return;
        ExitAudioSetting();
    }

    public void SetMasterVolume(float volume)
    {
        AudioSettingsManager.Instance?.SetMasterVolume(volume);
    }

    public void SetBGMVolume(float volume)
    {
        AudioSettingsManager.Instance?.SetBGMVolume(volume);
    }

    public void SetSFXVolume(float volume)
    {
        AudioSettingsManager.Instance?.SetSFXVolume(volume);
    }

    private void SetSliderValueWithoutNotify(Slider slider, float value)
    {
        if (slider == null) return;
        slider.SetValueWithoutNotify(value);
    }

    public void ExitAudioSetting()
    {
        AudioSettingsManager.Instance?.SaveVolumes();

        UIManager.Instance.OnPausePanel();

        gameObject.SetActive(false);
    }
}