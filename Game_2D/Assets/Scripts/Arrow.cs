using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D bc;

    public LayerMask layerFloor;
    public GameObject skeleton;
    public Vector2 directionArrow;
    public float collisionRadius;
    public bool hitTheGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().ReceiveDamage(-(collision.transform.position - skeleton.transform.position).normalized);
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        hitTheGround = Physics2D.OverlapCircle((Vector2)transform.position, collisionRadius, layerFloor);
        if (hitTheGround)
        {
            rb.bodyType = RigidbodyType2D.Static;
            bc.enabled = false;
            this.enabled = false;
        }

        float angule = Mathf.Atan2(directionArrow.y, directionArrow.x) * Mathf.Rad2Deg;

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.y, transform.localEulerAngles.x, angule);
    }
}
