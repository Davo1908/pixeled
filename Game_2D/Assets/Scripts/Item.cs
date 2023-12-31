using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            AssignItem();
        }
    }

    private void AssignItem()
    {
        if (gameObject.CompareTag("Coin"))
        {
            GameManager.Instance.UpdateCoinCounter();
        }else if (gameObject.CompareTag("PowerUp"))
        {
            GameManager.Instance.player.GiveImmortality();
        }

        Destroy(gameObject);
    }
}
