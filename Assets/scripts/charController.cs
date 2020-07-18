using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charController : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Movement Settings")]
    public float Speed = 0;
    public float MaxSpeed = 10;
    public float Acceleration = 10;
    public float Deceleration = 10;
    float position;
    float speed;

    [Header("Jump Settings")]
    public float jumpForce;
    public int defaultJumps = 1;
    int morejumps;
    bool jump;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Grounded checker")]
    bool isGrounded = false;
    public Transform isGroundedChecker;
    public float checkGroundRadius;
    public LayerMask groundLayer;
    public float rememberGroundedFor;
    float lastTimeGrounded;


    [Header("Misc")]
    private int buffercounter=0, coybuffer=0;
    public int buffermax = 10, coymax =10;
    bool bufferbool;
    bool midjump;
    Collider2D body;

    public float climbSpeed;
    bool isclimbing = false;
    float gravityScaleAtStart;

    float midair;
    bool walltouch;

    public static bool move;
    private Transform m_currMovingPlatform;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        morejumps = defaultJumps;
        body = rb.GetComponent<Collider2D>();
        gravityScaleAtStart = rb.gravityScale;
        jump = false;
        midair = 0;
    }
    private void FixedUpdate()
    {
        Move();
    }
    // Update is called once per frame
    void Update()
    {
       
        

        Jump();
        BetterJump();
        bufferjump();
        coyetetime();
        climbLadder();
        mid_air();
        if (isGrounded = true && rb.velocity.y == 0)
        {
            midjump = false;
        }
       // Debug.Log(midjump + "midjump");
        //Debug.Log("grounded" + isGrounded);
        CheckIfGrounded();
        Debug.Log(midjump);
        Debug.Log(midair);
        


    }


    void Move()
    {

        if ((Input.GetKey("left")) && (Speed > -MaxSpeed))
        {
            Speed = Speed - Acceleration * Time.deltaTime;
            if (Speed > 0)
            {
                Speed = Speed - Deceleration * Time.deltaTime;
            }
        }
        else if ((Input.GetKey("right")) && (Speed < MaxSpeed))
        {
            Speed = Speed + Acceleration * Time.deltaTime;
            if (Speed < 0)
            {
                Speed = Speed + Deceleration * Time.deltaTime;
            }
        }
            
        else
        {
            if (Speed > Deceleration * Time.deltaTime) Speed = Speed - Deceleration * Time.deltaTime;
            else if (Speed < -Deceleration * Time.deltaTime) Speed = Speed + Deceleration * Time.deltaTime;
            else
                Speed = 0;
        }
        position = transform.position.x + Speed * Time.deltaTime;
        transform.position = new Vector2(position, transform.position.y);
        /*
        float x = Input.GetAxisRaw("Horizontal");

        float moveBy = x * speed;

        rb.velocity = new Vector2(moveBy, rb.velocity.y);*/
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && (isGrounded || morejumps > 0) && walltouch == false)
        {
            midjump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            morejumps--;
            jump = true;
            transform.parent = null;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && (isclimbing ==true))
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jump = true;
            isclimbing = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && (walltouch == true) && isGrounded == false)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce*1.5f);
            Speed = 10;
            jump = true;
            isclimbing = false;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && (walltouch == true) && isGrounded == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }


    }



    void mid_air()
    {
        if(jump==true && rb.velocity.y > 0)
        {
            midair = 1;
        }
        else if(jump == true && rb.velocity.y < 0)
        {
            midair = -1;
        }
        else if (jump == false)
        {
            midair = 0;
        }

    }
    void checkstat()
    {
        if (midjump = true && rb.velocity.y > 0)
        {

        }
        else if (midjump = true && rb.velocity.y < 0)
        {

        }
        else if (isGrounded = true && rb.velocity.y == 0)
        {
            midjump = false;
        }
    }
    void BetterJump()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.velocity += Vector2.up * Physics2D.gravity * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
    void bufferjump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded == false)
        {
            bufferbool = true;
        }
        if (bufferbool == true)
        {
            buffercounter++;
            if (isGrounded)
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                midjump = true;
            }
            if (buffercounter > buffermax)
            {
                bufferbool = false;
                buffercounter = 0;
            }

        }

    }
    void coyetetime()
    {
        if (isGrounded == false)
        {
            if (Input.GetKeyDown(KeyCode.Space) && coybuffer < coymax && midjump==false)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            coybuffer++;
           
        }
        else
        {
            coybuffer = 0;
        }
    }
    void CheckIfGrounded()
    {
        Collider2D colliders = Physics2D.OverlapCircle(isGroundedChecker.position, checkGroundRadius, groundLayer);

        if (colliders != null)
        {
            isGrounded = true;
            jump = false;
            morejumps = defaultJumps;
            
        }
        else
        {
            if (isGrounded)
            {
                lastTimeGrounded = Time.time;
            }
            isGrounded = false;
        }
    }
 

    void climbLadder()
    {

        if (body.IsTouchingLayers(LayerMask.GetMask("ladder")))
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                isclimbing = true;
                Speed = 0;
            }

            if (isclimbing)
            {
                rb.velocity = new Vector2(0, 0);
                rb.gravityScale = 0;
                Debug.Log("2");
                float controlThrow = Input.GetAxisRaw("Vertical");
                Debug.Log("control throw" + controlThrow);
                //rb.velocity= new Vector2(rb.velocity.x, controlThrow * climbSpeed);
                transform.position = new Vector2(transform.position.x, transform.position.y + (controlThrow * Time.deltaTime * climbSpeed));

            }
            else
                rb.gravityScale = 1;

        }
        else
        {
            rb.gravityScale = 1;
            isclimbing = false;
        }
           


    }

    public void ClimbLadder()
    {
        if (!body.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            rb.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = Input.GetAxisRaw("Vertical");
        Vector2 climbVelocity = new Vector2(rb.velocity.x, controlThrow * climbSpeed);
        rb.velocity = climbVelocity;
        rb.gravityScale = 0;

    }
    void OnCollisionEnter2D(Collision2D other)
    {

        if (other.gameObject.tag == "wall")
        {
            walltouch = true;
        }
        if (other.gameObject.tag == "moving")
        {
            m_currMovingPlatform = other.gameObject.transform;
            transform.SetParent(m_currMovingPlatform);
        }
        if (other.gameObject.tag == "bounce")
        {
            rb.velocity = new Vector2(rb.velocity.x, 20);
        }

    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "wall")
        {
            walltouch = false;
        }

        if (other.gameObject.tag == "moving")
        {
            transform.parent = null;
            m_currMovingPlatform = null;

        }

    }

}
