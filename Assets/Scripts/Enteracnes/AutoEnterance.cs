using UnityEngine;

// 충돌 시 자동으로 씬 전환을 시작하는 입장 지점이다.
public class AutoEnterance : Enterance
{
    [SerializeField] 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnterArea(nextArea, EnteranceType.Auto);
        }
    }

}