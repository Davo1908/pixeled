using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Skeleton : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D rb;
    private SpriteRenderer sp;
    private Animator anim;
    private CinemachineVirtualCamera cm;
    private bool applyForce;

    public float distanceDetectionPlayer = 17;
    public float distanceDetectionArrow = 11;
    public GameObject arrow;
    public float forceLaunch = 5;
    public float velocityMov;
    public int lives = 3;
    public bool launchArrow;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        sp = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }


    void Start()
    {
        gameObject.name = "Skeleton";
    }

    void Update()
    {
        Vector2 direc = (player.transform.position - transform.position).normalized * distanceDetectionArrow;
        Debug.DrawRay(transform.position, direc, Color.red);

        float distanceCurrent = Vector2.Distance(transform.position, player.transform.position);

        if (distanceCurrent <= distanceDetectionArrow)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("Walking", false);

            Vector2 direcNormalized = direc.normalized;
            ChangeView(direcNormalized.x);
            if (!launchArrow)
                StartCoroutine(ShootingArrow(direc, distanceCurrent));
        }
        else if (distanceCurrent <= distanceDetectionPlayer)
        {
            Vector2 move = new Vector2(direc.x, 0);
            move = move.normalized;
            //rb.velocity = move * velocityMov;
            rb.velocity = new Vector2(move.x * velocityMov, rb.velocity.y);
            anim.SetBool("Walking", true);
            ChangeView(move.x);
        }
        else
        {
            anim.SetBool("Walking", false);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceDetectionPlayer);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanceDetectionArrow);
    }

    private IEnumerator ShootingArrow(Vector2 directionArrow, float distance)
    {
        launchArrow = true;
        anim.SetBool("Shooting", true);
        yield return new WaitForSeconds(1.42f);
        anim.SetBool("Shooting", false);
        directionArrow = directionArrow.normalized;

        GameObject arrowGO = Instantiate(arrow, transform.position, Quaternion.identity);
        arrowGO.transform.GetComponent<Arrow>().directionArrow = directionArrow;
        arrowGO.transform.GetComponent<Arrow>().skeleton = this.gameObject;

        arrowGO.transform.GetComponent<Rigidbody2D>().velocity = directionArrow * forceLaunch;
        launchArrow = false;
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
            velocityMov = 0;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.ReceiveDamage((transform.position - player.transform.position).normalized);
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            rb.AddForce((transform.position - player.transform.position).normalized * 100, ForceMode2D.Impulse);
            applyForce = false;
        }
    }
}
