using UnityEngine;

public class AutoEnterance : Enterance
{
    [SerializeField] 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            EnterArea(nextArea);
        }
    }

}
