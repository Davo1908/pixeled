using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private bool applyForce;
    private bool detectPlayer;
    private PlayerController player;

    public bool ofJump;
    public BoxCollider2D platformCollider;
    public BoxCollider2D platformTrigger;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (!ofJump)
            Physics2D.IgnoreCollision(platformCollider, platformTrigger, true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!ofJump)
                Physics2D.IgnoreCollision(platformCollider, player.GetComponent<CapsuleCollider2D>(), true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!ofJump)
                Physics2D.IgnoreCollision(platformCollider, player.GetComponent<CapsuleCollider2D>(), false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectPlayer = true;

            if(ofJump)
                applyForce= true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectPlayer = false;
        }
    }

    private void Update()
    {
        if (ofJump)
        {
            if(player.transform.position.y - 0.8f > transform.position.y)
            {
                platformCollider.isTrigger = false;
            }
            else
            {
                platformCollider.isTrigger = true; 
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 25, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
