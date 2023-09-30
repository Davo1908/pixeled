using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class Waypoints : MonoBehaviour
{
    private Vector3 direction;
    private PlayerController player;
    private CinemachineVirtualCamera cm;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private int currentIndex = 0;
    private bool applyForce;

    public int lives = 3;
    public Vector2 positionHead;
    public float displacementSpeed;
    public List<Transform> points = new List<Transform>();
    public bool expecting;
    public float waitTime;

    public void Awake()
    {
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        sp = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void Start()
    {
        if (gameObject.CompareTag("Enemy"))
            gameObject.name = "Spider";
    }

    private void FixedUpdate()
    {
        WaypointsMovement();
        if (gameObject.CompareTag("Enemy"))
        {
            ChangeEnemyScale();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Enemy"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y + positionHead.y)
            {
                player.GetComponent<Rigidbody2D>().velocity = Vector2.up * player.forceJump;

                Destroy(this.gameObject, 0.2f);
            }
            else
            {
                player.ReceiveDamage(-(player.transform.position - transform.position).normalized);
            }
        }else if(collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Platform"))
        {
            if(player.transform.position.y - 0.7f > transform.position.y)
            {
                player.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && gameObject.CompareTag("Platform"))
        {
            player.transform.parent = null;
        }
    }

    private void ChangeEnemyScale()
    {
        if (direction.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (direction.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    private void WaypointsMovement()
    {
        direction = (points[currentIndex].position- transform.position).normalized;

        if(!expecting)
            transform.position = (Vector2.MoveTowards(transform.position, points[currentIndex].position, displacementSpeed * Time.deltaTime));

        if(Vector2.Distance(transform.position, points[currentIndex].position) <= 0.7f)
        {
            if (!expecting)
            {
                StartCoroutine(Wait());
            }
        }
    }

    private IEnumerator Wait()
    {
        expecting = true;
        yield return new WaitForSeconds(waitTime);
        expecting = false;

        currentIndex++;

        if(currentIndex >= points.Count)
            currentIndex = 0;
    }

    public void ReceiveDamage()
    {
        if (lives > 0)
        {
            StartCoroutine(DamageEffect());
            StartCoroutine(ShakeCamera(0.1f));
            applyForce = true;
            lives--;
        }
        else
        {
            StartCoroutine(ShakeCamera(0.1f));
            displacementSpeed = 0;
            rb.velocity = Vector2.zero;
            Destroy(this.gameObject, 0.2f);
        }
    }

    private IEnumerator ShakeCamera(float time)
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
