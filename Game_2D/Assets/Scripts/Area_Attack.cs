using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area_Attack : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if(collision.name == "Bat")
            {
                collision.GetComponent<Bat>().ReceiveDamage();
            }
            else if(collision.name == "Skeleton")
            {
                collision.GetComponent<Skeleton>().ReceiveDamage();
            }
            else if(collision.name == "Spider")
            {
                collision.GetComponent<Waypoints>().ReceiveDamage();
            }
        }
        else if (collision.CompareTag("Destroyable"))
        {
            collision.GetComponent<Animator>().SetBool("Destroy", true);
        }
    }
}
