using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour {

    private GameObject player;
    private Vector3 startPosition;
    private Vector3 currentPosition;
    private Vector3 endPosition;

    // This var will determine if the animation is started
    private bool animation_started = false;
    // This var will determine if the animation is finished
    private bool animation_finished = true;

    bool movingForward = false;

    public int currentSpeed = 4;
    public int currentJumpHeight = 10;

	// Use this for initialization
	void Start () {
        player = GameObject.Find("player");
    }

    void movePlayer(string keyPress)
    {

       // GameObject leftLeg = GameObject.Find("player_leg-left");
        //GameObject rightLeg = GameObject.Find("player_leg-right");

        if (keyPress == "w")
        {
            transform.parent.position += Vector3.forward * currentSpeed;
           // rightLeg.GetComponent<Animator>().Play("AnimationMovePlayer");
        }
        if (keyPress == "s")
        {
            transform.parent.position += Vector3.back * currentSpeed;
            //rightLeg.GetComponent<Animator>().Play("AnimationMovePlayer");
        }
        if (keyPress == "a")
        { 
            transform.parent.position += Vector3.left * currentSpeed;
            // rightLeg.GetComponent<Animator>().Play("AnimationMovePlayer");
        }
        if (keyPress == "d")
        {
            transform.parent.position += Vector3.right * currentSpeed;
        }
        if (keyPress == "space")
        {
            //transform.parent.position += Vector3.up * currentJumpHeight;
            //player.GetComponent<Animation>().Play("player-jump");

            player.GetComponent<Rigidbody>().AddForce(Vector3.up * 200);
            //gameObject.rigidbody.AddForce(Vector3.up * jumpForce);
        }

        //animation_started = true;
        //animation_finished = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKey("w"))
        {
            if (!player.GetComponent<Animation>().IsPlaying("player-jump"))
            {
                movePlayer("w");
            }
        }
        if (Input.GetKey("s"))
        {
            if (!player.GetComponent<Animation>().IsPlaying("player-jump"))
            {
                movePlayer("s");
            }
        }
        if(Input.GetKey("a"))
        {
            if (!player.GetComponent<Animation>().IsPlaying("player-jump"))
            {
                movePlayer("a");
            }
        }
        if (Input.GetKey("d"))
        {
            if (!player.GetComponent<Animation>().IsPlaying("player-jump"))
            {
                movePlayer("d");
            }
        }
        if (Input.GetKeyDown("space"))
        {
            movePlayer("space");
        }
    }

    void LateUpdate ()
    {

    }
}
