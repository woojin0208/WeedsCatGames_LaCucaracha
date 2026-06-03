using UnityEngine;

// 무기 렌더링과 애니메이션을 담당한다.
public class WeaponRenderer : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnDestruction()
    {
        if (audioSource != null)
            audioSource.Play();

        if (animator != null && animator.enabled)
        {
            animator.SetTrigger("Destruction");
            Destroy(gameObject, 1f);
            return;
        }
        Destroy(gameObject, 0.1f);
        
    }
}