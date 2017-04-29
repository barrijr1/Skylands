using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {


    public float maxSpeed = 10f;
    private bool facingRight = true;

    Animator anim;

    bool grounded = false;
    public Transform groundCheck;
    float groundRadius = 0.2f;
    public LayerMask whatIsGround;
    private Rigidbody2D body;
    public float jumpForce = 20f;
    public int maxJumpCount = 2;
    private int jumpCount;

    private int minScroll = 4;
    private int maxScroll = 8;

    private GameObject mainCamera;
    private Camera cameraScroller;

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
		if(grounded && (Input.GetKeyDown("space") || Input.GetKeyDown("w")))
        {
            anim.SetBool("Ground", false);
            body.AddForce(new Vector2(0, jumpForce));
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
        {
            if (cameraScroller.orthographicSize >= minScroll)
            {
                cameraScroller.orthographicSize--;
            }
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
        {
            if (cameraScroller.orthographicSize <= maxScroll)
            {
                cameraScroller.orthographicSize++;
            }
        }
    }

    void FixedUpdate()
    {
        float move = Input.GetAxis("Horizontal");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        cameraScroller = mainCamera.GetComponent<Camera>();

        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);
        anim.SetBool("Ground", grounded);

        anim.SetFloat("vSpeed", body.velocity.y);

        anim.SetFloat("Speed", Mathf.Abs(move));

        body.velocity = new Vector2(move * maxSpeed, body.velocity.y);

        if (move > 0 && !facingRight)
        {
            ChangeDirection();
        }
        else if (move < 0 && facingRight)
        {
            ChangeDirection();
        }
    }

    void ChangeDirection()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
