using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using Cinemachine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using static Cinemachine.DocumentationSortingAttribute;

public class PlayerController : MonoBehaviour
{
    private int direcX;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 direction;
    private CinemachineVirtualCamera cm;
    private Vector2 directionMove;
    private Vector2 directionDamage;
    private bool block;
    private GrayCamera gc;
    private SpriteRenderer sprite;

    [Header ("Estadistics")]
    public float velocityMove = 10;
    public float forceJump = 5;
    public float velocityDash = 20;
    public float velocitySlide;
    public int lives = 3;
    public float timeInmortality;

    [Header("Colisions")]
    public LayerMask layerFloor;
    public float radiusCollition;
    public Vector2 down, right, left;

    [Header("Booleanos")]
    public bool canMove = true;
    public bool inFloor = true;
    public bool canDash;
    public bool doingDash;
    public bool touchedFloor;
    public bool doingShake;
    public bool inAttack;
    public bool inWall;
    public bool rightWall;
    public bool leftWall;
    public bool grabOn;
    public bool jumpWall;
    public bool itIsImmortal;
    public bool applyForce;
    public bool finalMap;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cm = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
        gc = Camera.main.GetComponent<GrayCamera>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetBlockTrue()
    {
        block = true;
    }

    public void Dead()
    {
        if (lives > 0)
            return;

        GameManager.Instance.GameOver();
        this.enabled= false;
    }

    public void ReceiveDamage()
    {
        StartCoroutine(ImpactDamage(Vector2.zero));
    }

    public void ReceiveDamage(Vector2 directionDamage)
    {
        StartCoroutine(ImpactDamage(directionDamage));
    }

    private IEnumerator ImpactDamage(Vector2 directionDamage)
    {
        if (!itIsImmortal)
        {
            StartCoroutine(Immortality());
            lives--;
            gc.enabled = true;
            float velocityAuxiliar = velocityMove;
            this.directionDamage = directionDamage;
            applyForce = true;
            Time.timeScale = 0.4f;
            FindObjectOfType<RippleEffect>().Emit(Camera.main.WorldToViewportPoint(transform.position));
            StartCoroutine(ShakeCamera(0.2f));
            yield return new WaitForSeconds(0.2f);
            Time.timeScale = 1;
            gc.enabled = false;

            UpdateLivesUI(1);

            velocityMove = velocityAuxiliar;
            Dead();
        }
    }

    public void GiveImmortality()
    {
        StartCoroutine(Immortality());
    }

    public IEnumerator Immortality()
    {
        itIsImmortal = true;

        float timeElapsed = 0;

        while (timeElapsed < timeInmortality)
        {
            sprite.color = new Color (1, 1, 1, .5f);
            yield return new WaitForSeconds(timeInmortality / 20);
            sprite.color = new Color(1, 1, 1, 1);
            yield return new WaitForSeconds(timeInmortality / 20);
            timeElapsed += timeInmortality / 10;
        }
        itIsImmortal = false;
    }

    public void UpdateLivesUI(int discountedLives)
    {
        int discountedLife = discountedLives;

        for (int i = GameManager.Instance.livesUI.transform.childCount - 1; i >= 0; i--)
        {
            if (GameManager.Instance.livesUI.transform.GetChild(i).gameObject.activeInHierarchy && discountedLife != 0)
            {
                GameManager.Instance.livesUI.transform.GetChild(i).gameObject.SetActive(false);
                discountedLife--;
            }
            else
            {
                if (discountedLife == 0)
                    break;
            }
        }
    }

    private void FixedUpdate()
    {
        if (applyForce)
        {
            velocityMove = 0;
            rb.velocity = Vector2.zero;
            rb.AddForce(-directionDamage * 25, ForceMode2D.Impulse);
            applyForce = false;
        }
    }

    public void FinalMapMove(int direcX)
    {
        finalMap = true;
        this.direcX = direcX;
        anim.SetBool("IsWalk", true);

        if (this.direcX < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (this.direcX > 0 && transform.localScale.x < 0)
        {

            transform.localScale = new Vector3(Mathf.Abs(-transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!finalMap)
        {
            Move();
            Grips();
        }
        else
        {
            rb.velocity = (new Vector2(direcX * velocityMove, rb.velocity.y));
        }

    }

    private void Attack(Vector2 direction)
    {
        if(Input.GetKeyDown(KeyCode.B))
        {
            if (!inAttack && !doingDash)
            {
                inAttack = true;

                anim.SetFloat("Attack_X", direction.x);
                anim.SetFloat("Attack_Y", direction.y);

                anim.SetBool("IsAttack", true);
            }
        }
    }

    public void FinishAttack()
    {
        anim.SetBool("IsAttack", false);
        block = false;
        inAttack = false;
    }

    private Vector2 DirectionAttack(Vector2 directionMove, Vector2 direction)
    {
        if (rb.velocity.x == 0 && direction.y != 0)
            return new Vector2(0, direction.y);

        return new Vector2(directionMove.x, direction.y);
    }

    private IEnumerator ShakeCamera()
    {
        doingShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(0.3f);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        doingShake = false;
    }
    
    private IEnumerator ShakeCamera(float time)
    {
        doingShake = true;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = cm.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 5;
        yield return new WaitForSeconds(time);
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
        doingShake = false;
    }

    private void Dash(float x, float y)
    {
        anim.SetBool("Dash", true);
        Vector3 positionPlayer = Camera.main.WorldToViewportPoint(transform.position);
        Camera.main.GetComponent<RippleEffect>().Emit(positionPlayer);
        StartCoroutine(ShakeCamera());

        canDash = true;
        rb.velocity = Vector2.zero;
        rb.velocity += new Vector2(x, y).normalized * velocityDash;
        StartCoroutine(PrepareDash());
    }

    private IEnumerator PrepareDash()
    {
        StartCoroutine(DashFloor());

        rb.gravityScale = 0;
        doingDash = true;

        yield return new WaitForSeconds(0.3f);

        rb.gravityScale = 3;
        doingDash = false;
        FinishDash();
    }

    private IEnumerator DashFloor()
    {
        yield return new WaitForSeconds(0.15f);
        if (inFloor)
            canDash = false;

    }

    public void FinishDash()
    {
        anim.SetBool("Dash", false);
    }

    private void TapFloor()
    {
        canDash = false;
        doingDash = false;
        anim.SetBool("IsJump", false);
    }

    private void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");

        direction = new Vector2(x, y);
        Vector2 directionRaw = new Vector2(xRaw, yRaw);

        Walk();
        Attack(DirectionAttack(directionMove, directionRaw));

        if(inFloor && !doingDash)
        {
            jumpWall = false;
        }

        grabOn = inWall && Input.GetKey(KeyCode.LeftShift);

        if (grabOn && !inFloor)
        {
            anim.SetBool("IsClimb", true);

            if (rb.velocity == Vector2.zero)
            {
                anim.SetFloat("Velocity", 0);
            }
            else
            {
                anim.SetFloat("Velocity", 1);
            }
        }
        else
        {
            anim.SetBool("IsClimb", false);
            anim.SetFloat("Velocity", 0);
        }

        if(grabOn && !doingDash)
        {
            rb.gravityScale = 0;
            if(x > 0.2f || x < 0.2)
                rb.velocity = new Vector2(rb.velocity.x, 0);
            
            float speedModifier = y > 0 ? 0.5f : 1;
            rb.velocity = new Vector2(rb.velocity.x, y * (velocityMove * speedModifier));

            if(leftWall && transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
            else if(rightWall && transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
        else
        {
            rb.gravityScale = 3;
        }

        if(inWall && !inFloor)
        {
            anim.SetBool("IsClimb", true);

            if (x != 0 && !grabOn)
                SlideWall();
        }

        ImproveJump();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(inFloor)
            {
                anim.SetBool("IsJump", true);
                Jump();
            }

            if(inWall && !inFloor)
            {
                anim.SetBool("IsClimb", false);
                anim.SetBool("IsJump", true);
                JumpFromWall();
            }
        }

        if (Input.GetKeyDown(KeyCode.V) && !doingDash && !canDash)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (inFloor && !touchedFloor)
        {
            anim.SetBool("IsClimb", false);

            TapFloor();
            touchedFloor = true;
        }

        if (!inFloor && touchedFloor)
            touchedFloor = false;

        float speed;
        if (rb.velocity.y > 0)
            speed = 1;
        else
            speed = -1;


        if (!inFloor)
        {
            anim.SetFloat("VelocityVertical", speed);
        }
        else
        {
            if(speed == -1)
                FinishJump();
        }
    }

    private void SlideWall()
    {
        if (canMove)
            rb.velocity = new Vector2(rb.velocity.x, -velocitySlide);
    }

    private void JumpFromWall()
    {
        StopCoroutine(DisableMotion(0));
        StartCoroutine(DisableMotion(0.1f));

        Vector2 wallDirection = rightWall ? Vector2.left : Vector2.right;

        if(wallDirection.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3 (-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if(wallDirection.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        anim.SetBool("Jump", true);
        anim.SetBool("Climb", false);
        Jump((Vector2.up + wallDirection), true);

        jumpWall = true;
    }

    private IEnumerator DisableMotion(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    public void FinishJump()
    {
        anim.SetBool("IsJump", false);
    }

    private void ImproveJump()
    {
        if(rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.5f - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (2.0f - 1) * Time.deltaTime;
        }
    }
    private void Grips()
    {
        inFloor = Physics2D.OverlapCircle((Vector2)transform.position + down, radiusCollition, layerFloor);

        Collider2D rightCollision = Physics2D.OverlapCircle((Vector2)transform.position + right, radiusCollition, layerFloor);
        Collider2D leftCollision = Physics2D.OverlapCircle((Vector2)transform.position + left, radiusCollition, layerFloor);
        
        if(rightCollision != null)
        {
            inWall = !rightCollision.CompareTag("Platform");
        }else if(leftCollision != null)
        {
            inWall= !leftCollision.CompareTag("Platform");
        }
        else
        {
            inWall = false;
        }

        rightWall = Physics2D.OverlapCircle((Vector2)transform.position + right, radiusCollition, layerFloor);
        leftWall = Physics2D.OverlapCircle((Vector2)transform.position + left, radiusCollition, layerFloor); 
    }

    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += Vector2.up * forceJump;
    }
    
    private void Jump(Vector2 directionJump, bool wall)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += directionJump * forceJump;
    }

    private void Walk()
    {
        if (canMove && !doingDash && !inAttack)
        {
            if (jumpWall)
            {
                rb.velocity = Vector2.Lerp(rb.velocity,
                    (new Vector2(direction.x * velocityMove, rb.velocity.y)), Time.deltaTime / 2);
            }
            else
            {
                if (direction != Vector2.zero && !grabOn)
                {
                    if (!inFloor)
                    {
                        anim.SetBool("IsJump", true);
                    }
                    else
                    {
                        anim.SetBool("IsWalk", true);
                    }

                    rb.velocity = (new Vector2(direction.x * velocityMove, rb.velocity.y));

                    if (direction.x < 0 && transform.localScale.x > 0)
                    {
                        directionMove = DirectionAttack(Vector2.left, direction);

                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                    }
                    else if (direction.x > 0 && transform.localScale.x < 0)
                    {
                        directionMove = DirectionAttack(Vector2.right, direction);

                        transform.localScale = new Vector3(Mathf.Abs(-transform.localScale.x), transform.localScale.y, transform.localScale.z);
                    }
                }
                else
                {
                    if (direction.y > 0 && direction.x == 0)
                    {
                        directionMove = DirectionAttack(direction, Vector2.up);
                    }

                    anim.SetBool("IsWalk", false);
                }
            }
            
        }
        else
        {
            if (block)
            {
                FinishAttack();
            }
        }
    }
}
