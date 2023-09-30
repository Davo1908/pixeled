using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.VisualScripting;

public class Bat : MonoBehaviour
{
    private CinemachineVirtualCamera cm;
    private SpriteRenderer sp;
    private PlayerController player;
    private Rigidbody2D rb;
    private bool applyForce;

    public float velocityOfMove = 3;
    public float radiusDetection = 15;
    public LayerMask layerPlayer;
    public int impactForce;

    public Vector2 posisionHead;

    public int lives = 3;
    public string nam;

    private void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = nam;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusDetection);
        Gizmos.color = Color.green;
        Gizmos.DrawCube((Vector2)transform.position + posisionHead, new Vector2(1, 0.5f) * 0.7f);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = player.transform.position - transform.position;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= radiusDetection)
        {
            rb.velocity = direction.normalized * velocityOfMove;
            ChangeView(direction.normalized.x);
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void ChangeView(float directionX)
    {
        if (directionX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (directionX > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (transform.position.y + posisionHead.y < player.transform.position.y - 0.7f)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.forceJump;
                StartCoroutine(ShakeCamera(0.1f));
                Destroy(gameObject, 0.2f);
            }
            else
            {
                player.ReceiveDamage((transform.position - player.transform.position).normalized);
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * impactForce, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    public void ReceiveDamage()
    {
        StartCoroutine(ShakeCamera(0.1f));

        if (lives > 0)
        {
            StartCoroutine(DamageEffect());
            
            applyForce = true;
            lives--;
        }
        else
        {
            Destroy(gameObject, 0.2f);
        }
    }

    private IEnumerator ShakeCamera (float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(time);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    private IEnumerator DamageEffect()
    {
        sp.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        sp.color = Color.white;
    }
}
