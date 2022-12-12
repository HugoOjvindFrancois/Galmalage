using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    // Constant attributs

    private const string IS_WALKING_ANIMATOR = "isWalking";

    // Public attributs

    public float Speed = 5f;
    public float JumpHeight = 2f;
    public float Gravity = -9.81f;
    public float GroundDistance = 0.2f;
    public float DashDistance = 5f;
    public LayerMask Ground;
    public GameObject Respawn;
    public int RespawnDelay = 5;
    public int InvicibilityDelay = 15;

    public int jump = 2;

    // Private attributs

    private int _remainingJump = 0;
    private PlayerInput _input;
    private CharacterController _controller;
    private Animator _animator;
    private Vector3 _velocity;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    private Vector3 _forwardDirection;
    private Vector3 _moveDirection = Vector3.zero;
    private bool _isWalking;

    // Start is called before the first frame update
    void Start() {
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _groundChecker = transform.GetChild(0);
        _input = new PlayerInput();
        _remainingJump = jump;
        _input.Player.Enable();
        _input.Player.Move.performed += OnMove;
        _input.Player.Move.canceled += OnMoveStop;
        _input.Player.Jump.performed += OnJump;
        _isWalking = false;
        if (Respawn == null)
            Respawn = GameObject.FindGameObjectsWithTag("Respawn")[0];
    }

    // Update is called once per frame
    void Update() {
        //_isGrounded = Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore);
        _isGrounded = _controller.isGrounded;
        if (_isGrounded && _velocity.y < 0) {
            _velocity.y = 0f;
            _remainingJump = jump;
        } else {
            _velocity.y += Gravity * Time.deltaTime;
        }
        Mouvement();
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context) {
        _moveDirection.x = context.ReadValue<Vector2>().x;
        if (!_isWalking) {
            _animator.SetBool(IS_WALKING_ANIMATOR, true);
        }
        _isWalking = true;
        
    }

    public void OnMoveStop(InputAction.CallbackContext context) {
        _moveDirection.x = 0;
        if (_isWalking) {
            _animator.SetBool(IS_WALKING_ANIMATOR, false);
        }
        _isWalking = false;
    }

    public void OnJump(InputAction.CallbackContext context) {
        if (_isGrounded || _remainingJump > 0)
            Jump();
    }

    private void Mouvement() {        
        Vector3 move = _moveDirection;
        _controller.Move(move * Time.deltaTime * Speed);
        if (move != Vector3.zero && _isGrounded)
            transform.forward = move;
    }

    private void Jump () {
        _remainingJump = _remainingJump - 1;
        _velocity.y = 0;
        _velocity.y += Mathf.Sqrt(JumpHeight * -2f * Gravity);
        _forwardDirection.x = transform.forward.x;
    }

    public void Respawning() {
        this.gameObject.SetActive(false);
        this._input.Disable();
        this.transform.position = Respawn.transform.position;
        this.transform.rotation = Respawn.transform.rotation;
        this._moveDirection = Vector3.zero;
        this._velocity = Vector3.zero;
        this.gameObject.SetActive(true);
        StartCoroutine(StartRespawnDelay());
        StartCoroutine(StartInvibilityDelay());
    }

    IEnumerator StartRespawnDelay(){
        Debug.Log("Started respawning delay at timestamp : " + Time.time);

        yield return new WaitForSeconds(RespawnDelay);
        this._input.Enable();

        Debug.Log("Finished respawning delay at timestamp : " + Time.time);
    }

    IEnumerator StartInvibilityDelay() {
        Debug.Log("Started invicibility delay at timestamp : " + Time.time);

        yield return new WaitForSeconds(InvicibilityDelay);

        Debug.Log("Finished invicibility delay at timestamp : " + Time.time);
    }
}
