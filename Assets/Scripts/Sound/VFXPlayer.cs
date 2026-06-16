using UnityEngine;

// 효과음을 즉시 재생한다.
// StartVFX는 기존 Animation Event와 Inspector 연결 호환을 위해 유지한다.
public class VFXPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip[] vfxClips;
    [SerializeField] private AudioSource audioSource;

    // 기존 Animation Event와 UnityEvent 연결 호환용 메서드다.
    public void StartVFX(int index)
    {
        PlaySFX(index);
    }

    public void PlaySFX(int index)
    {
        if (!CanPlaySFX(index)) return;

        audioSource.Stop();
        audioSource.clip = vfxClips[index];
        audioSource.Play();
    }

    private bool CanPlaySFX(int index)
    {
        if (audioSource == null)
        {
            Debug.LogError("[VFXPlayer] AudioSource가 할당되지 않았습니다.", this);
            return false;
        }

        if (vfxClips == null || vfxClips.Length == 0)
        {
            Debug.LogWarning("[VFXPlayer] SFX Clip이 비어 있습니다.", this);
            return false;
        }

        if (index < 0 || index >= vfxClips.Length)
        {
            Debug.LogWarning($"[VFXPlayer] SFX index가 범위를 벗어났습니다. index: {index}", this);
            return false;
        }

        if (vfxClips[index] == null)
        {
            Debug.LogWarning($"[VFXPlayer] SFX Clip이 비어 있습니다. index: {index}", this);
            return false;
        }

        return true;
    }
}