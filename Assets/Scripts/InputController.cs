﻿using UnityEngine;
using XboxCtrlrInput;

/// <summary>
/// The InputController has hardcoded strings corresponding to Buttons and Joystick Axis in
///  the Unity Input Manager. Changes there have to be reflected here...
/// </summary>
public class InputController : MonoBehaviour {

/// <summary>
/// Editor Variables
/// </summary>
    public float PlayerVelocity = 6.0f;
    public float PlayerJumpHeight = 6.0f;
    public float PlayerChargeMultiplier = 3.0f;
    public bool InvertXAxis = false;
    public XboxController XboxInput;

/// <summary>
/// Private Variables
/// </summary>
    private GameStateManager GameState;

    private Player MainPlayer;
    private Rigidbody2D PlayerPhysics;
    private BoxCollider2D PlayerCollider;
    private Animator PlayerAnimator;
    private float VariableVelocity;

    private int PlayerNumber=1;
    private bool IsMoving=false;
    private bool IsInteracting=false;
    // set false or true depending on whether Grandpa or Kid and can jump
    private bool jumpAllowed=true;
    private float TranslationMovement;
    private string _currentDirection = "right";

    private const int KID_PLAYER_NUM = 1;
    private const int GRANDPA_PLAYER_NUM = 2;

    // state int to be set to for changing animation
    private enum AnimStates {
        Idle=0,
        Charge=2,
        Grenade=3,
        Jump=4,
        Scared=5,
        Touch=6,
        Walk=1,
        Whisper=7
    }

	// Use this for initialization
	void Start ()
    {
       GameState = FindObjectOfType<GameStateManager>();

	   if(GameState==null)
       {
           Debug.Log("GameStateManager missing! Abort!");
           this.enabled = false;
           return;
       }
       else
       {
           MainPlayer = gameObject.GetComponent<Player>();
           PlayerPhysics = gameObject.GetComponent<Rigidbody2D>();
           PlayerCollider = gameObject.GetComponent<BoxCollider2D>();
           PlayerAnimator = gameObject.GetComponent<Animator>();
           if(MainPlayer!=null && PlayerPhysics!=null)
           {
               PlayerNumber = (int) XboxInput;
               Debug.Log("Setting up for input Player "+PlayerNumber);
               //No setup anymore!
           }
       }

	}

	// Update is called once per frame
	void Update ()
    {
        if (!GameState.IsGamePaused())
        {
            HandlePlayerInput();
            AnimateWalk();
            ReadDebug();
        }
	}

    // Check and read Input
    private void HandlePlayerInput()
    {
        //Interactions
        IsInteracting = XCI.GetButton(XboxButton.A);
        if(IsInteracting) {
            foreach(GameStateManager.LevelInteraction interaction in MainPlayer.GetLevelInteractionsColliding()) {
                interaction.HasBeenInteracted = true;
                GameState.DoInteraction(interaction);
            }
        }

        float mVelocity = PlayerVelocity;
        if (isCurrentAnimation("Charge")) {
            mVelocity *= PlayerChargeMultiplier;
        }
        //Jumping
        if(IsGrounded())
        {
            VariableVelocity = mVelocity;
            if(XCI.GetButtonDown(XboxButton.X,XboxInput))
            {
                Jump();
            }
        }
        else
        {
            VariableVelocity -= Time.deltaTime*PlayerVelocity/2;
        }

       //Movement (Horizontal only)
        TranslationMovement =  XCI.GetAxis(XboxAxis.LeftStickX,XboxInput);
        if (TranslationMovement >= 0.0f) {
            changeDirection("right");
        } else {
            changeDirection("left");
            TranslationMovement *= -1;
        }
        gameObject.transform.Translate(Time.deltaTime * VariableVelocity * TranslationMovement,0,0);

        // Hint
        if(XCI.GetButton(XboxButton.B,XboxInput)) {
            MainPlayer.ShowHint();
        }

        // Charge
        if(XCI.GetButton(XboxButton.Y,XboxInput)) {
            if (MainPlayer.PlayerNumber == KID_PLAYER_NUM) {
                Charge();
            }
        }

        //DPad - Let the player handle this logic
        if(XCI.GetDPadDown(XboxDPad.Up,XboxInput))
        {
            MainPlayer.OnDPadUp();
        }
        else if( XCI.GetDPadDown(XboxDPad.Down,XboxInput))
        {
            MainPlayer.OnDPadDown();
        }
        else if( XCI.GetDPadDown(XboxDPad.Left,XboxInput))
        {
            MainPlayer.OnDPadLeft();
        }
        else if( XCI.GetDPadDown(XboxDPad.Right,XboxInput))
        {
            MainPlayer.OnDPadRight();
        }

        if(XCI.GetDPadUp(XboxDPad.Up, XboxInput)) {
            MainPlayer.OnDPadUpReleased();
        } else if(XCI.GetDPadUp(XboxDPad.Down, XboxInput)) {
            MainPlayer.OnDPadDownReleased();
        } else if(XCI.GetDPadUp(XboxDPad.Left, XboxInput)) {
            MainPlayer.OnDPadLeftReleased();
        } else if(XCI.GetDPadUp(XboxDPad.Right, XboxInput)) {
            MainPlayer.OnDPadRightReleased();
        }
    }

    public bool IsGrounded()
    {
        return PlayerPhysics.velocity.y < 0.001f && PlayerPhysics.velocity.y > -0.001f;
    }


    private void AnimateWalk()
    {
        if(Mathf.Abs(TranslationMovement) < 0.1f)
        {
            PlayerAnimator.SetInteger("state", (int) AnimStates.Idle);
        }
        else
        {
            PlayerAnimator.SetInteger("state", (int) AnimStates.Walk);
        }
    }

    public bool Interacted() {
        return IsInteracting;
    }

    private void Jump()
    {
       if (!jumpAllowed) {
           return;
       }

        PlayerPhysics.AddForce(
            new Vector2(0, PlayerPhysics.mass * PlayerJumpHeight ),
            ForceMode2D.Impulse
        );

        PlayerAnimator.Play(getAnimationName("Jump"), 0);
        // XXX So the following should work, but it doesn't. Obviously. Because Unity.
        // PlayerAnimator.SetInteger("state", (int)AnimStates.Walk);
    }

    private void Charge() {
        if (isCurrentAnimation(getAnimationName("Charge"))) {
            return;
        }
        PlayerAnimator.Play(getAnimationName("Charge"), 0);
    }

    private bool isCurrentAnimation(string name) {
        return PlayerAnimator.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private string getAnimationName(string animationName) {
        string anim_name = "Grandpa" + animationName;
        if (MainPlayer.PlayerNumber == KID_PLAYER_NUM) {
            anim_name = "Kid" + animationName;
        }
        return anim_name;
    }

    private void changeDirection(string direction) {
        if (_currentDirection != direction) {
            if (direction == "right")
             {
             gameObject.transform.Rotate(0, 180, 0);
             _currentDirection = "right";
             }
             else if (direction == "left")
             {
             gameObject.transform.Rotate(0, -180, 0);
             _currentDirection = "left";
             }
        }
    }

    //We want our game to support only Xbox Gamepad input.
    // However, we need hardcoded keyboard input for now...
    private void ReadDebug()
    {
        if(XboxInput == XboxController.First)
        {
            DebugP1();
        }
        else
        {
            DebugP2();
        }
    }

    private void DebugP1()
    {
        IsInteracting = Input.GetKeyDown(KeyCode.E);
        if(IsInteracting) {
            foreach(GameStateManager.LevelInteraction interaction in MainPlayer.GetLevelInteractionsColliding()) {
                interaction.HasBeenInteracted = true;
                GameState.DoInteraction(interaction);
            }
        }

        if(IsGrounded())
        {
            VariableVelocity = PlayerVelocity;

            if(Input.GetKeyDown(KeyCode.W))
            {
                Jump();
            }
        }
        else
        {
            VariableVelocity -= Time.deltaTime*PlayerVelocity/2;
        }


        if(Input.GetKey(KeyCode.A))
        {
            TranslationMovement = -1.0f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            TranslationMovement = 1.0f;
        }
        else
        {
            TranslationMovement = Mathf.Lerp(TranslationMovement,0.0f,Time.deltaTime);
        }

        gameObject.transform.Translate(Time.deltaTime * VariableVelocity * TranslationMovement,0,0);


        if(Input.GetKeyDown(KeyCode.Q)) {
            MainPlayer.ShowHint();
        }
    }

    private void DebugP2()
    {
        IsInteracting = Input.GetKeyDown(KeyCode.RightShift);
        if(IsInteracting) {
            foreach(GameStateManager.LevelInteraction interaction in MainPlayer.GetLevelInteractionsColliding()) {
                interaction.HasBeenInteracted = true;
                GameState.DoInteraction(interaction);
            }
        }

        if(IsGrounded())
        {
            VariableVelocity = PlayerVelocity;

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            VariableVelocity -= Time.deltaTime*PlayerVelocity/2;
        }

        if(Input.GetKey(KeyCode.LeftArrow))
        {
            TranslationMovement = -1.0f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            TranslationMovement = 1.0f;
        }
        else
        {
            TranslationMovement = Mathf.Lerp(TranslationMovement,0.0f,Time.deltaTime);
        }

        gameObject.transform.Translate(Time.deltaTime * VariableVelocity * TranslationMovement,0,0);

        if(Input.GetKeyDown(KeyCode.Return)) {
            MainPlayer.ShowHint();
        }
    }
}
