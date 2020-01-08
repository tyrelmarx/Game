using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using DG.Tweening;

public class Movement : MonoBehaviour
{
    private Collision coll;
    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public Rigidbody2D rb;
    public Animator animator;

    [Space]
    [Header("Stats")]
    public float speed = 10f;
    public float jumpForce = 50f;
    public float slideSpeed = 5f;

    public float lowJumpMultiplier = 200f;
    public float fallMultiplier = 200f;


    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;

    public bool wallSlide;

    [Space]

    private bool groundTouch;
    private bool holdJump;
    private bool betterJumpingEnabled;


    public int side = 1;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);

        Walk(dir);
        animator.SetFloat("Speed", Math.Abs(x));

        if (coll.onWall && Input.GetButton("Fire3") && canMove)
        {
            if (side != coll.wallSide)
                //                anim.Flip(side * -1);

                wallGrab = true;
            wallSlide = false;
            betterJumpingEnabled = false;
        }

        if (Input.GetButtonUp("Fire3") || !coll.onWall || !canMove)
        {
            wallGrab = false;
            wallSlide = false;
            betterJumpingEnabled = true;
        }

        if (coll.onGround)
        {
            betterJumpingEnabled = true;
        }

        if (wallGrab)
        {
            rb.gravityScale = 0;
            if (x > .2f || x < -.2f)
                rb.velocity = new Vector2(rb.velocity.x, 0);

            float speedModifier = y > 0 ? .5f : 1;

            rb.velocity = new Vector2(rb.velocity.x, y * (speed * speedModifier));
        }
        else
        {
            rb.gravityScale = 3;
        }

        if (coll.onWall && !coll.onGround)
        {
            if (x != 0 && !wallGrab)
            {
                wallSlide = true;
                WallSlide();
            }
        }

        if (!coll.onWall || coll.onGround)
            wallSlide = false;

        if (Input.GetButtonDown("Jump"))
        {
            //            anim.SetTrigger("jump");

            if (coll.onGround)
                Jump(Vector2.up, false);
        }

        if (Input.GetButton("Jump"))
        {
            holdJump = true;
        }
        else
        {
            holdJump = false;
        }

        if (coll.onGround && !groundTouch)
        {
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            groundTouch = false;
        }

        if (wallGrab || wallSlide || !canMove)
            return;

        if (x > 0)
        {
            side = 1;
            //            anim.Flip(side);
            PlayerFlip(side);

        }
        if (x < 0)
        {
            side = -1;
            //            anim.Flip(side);
            PlayerFlip(side);

        }


    }


    private void WallSlide()
    {
        if (coll.wallSide != side)
            //            anim.Flip(side * -1);

            if (!canMove)
                return;

        bool pushingWall = false;
        if ((rb.velocity.x > 0 && coll.onRightWall) || (rb.velocity.x < 0 && coll.onLeftWall))
        {
            pushingWall = true;
        }
        float push = pushingWall ? 0 : rb.velocity.x;

        rb.velocity = new Vector2(push, -slideSpeed);
    }

    private void Walk(Vector2 dir)
    {
        if (!canMove)
            return;

        if (wallGrab)
            return;


            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);

    }

    private void Jump(Vector2 dir, bool wall)
    {

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

    }

    IEnumerator DisableMovement(float time)
    {
        canMove = false;
        yield return new WaitForSeconds(time);
        canMove = true;
    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

    private void FixedUpdate()
    {
        BetterJumping();
    }

    void BetterJumping()
    {
        if (betterJumpingEnabled)
        {
            if (!holdJump)
            {
                rb.AddForce(new Vector2(0f, Physics2D.gravity.y * lowJumpMultiplier * Time.deltaTime));
            }
            //make the player fall faster
            else if (rb.velocity.y < 0)
            {
                rb.AddForce(new Vector2(0f, Physics2D.gravity.y * fallMultiplier * Time.deltaTime));
            }

        }
    }


    void PlayerFlip(int side)
    {
        if (side == -1)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }
}