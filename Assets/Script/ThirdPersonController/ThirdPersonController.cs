using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonController1 : MonoBehaviour
{
    //input fields
    private ThirdPersonActions thirdPersonActions;

    private InputAction move;
    private InputAction look;
    private InputAction jump;
    private InputAction sprint;
    private InputAction primaryShoot;

    private bool isJumping;
    private bool isGrounded;
    private bool isfalling;
    private Animator animator;

    private Rigidbody rb;

    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float maxSpeed = 5f;
    private Vector3 forceDirection = Vector3.zero;

    [SerializeField]
    private Camera playerCamera;
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    Transform firePoint;

    private Logger logger;

    private void Awake()
    {
        thirdPersonActions = new ThirdPersonActions();
        rb = this.GetComponent<Rigidbody>();
        animator = this.GetComponent<Animator>();

        move = thirdPersonActions.Player.Move;
        look = thirdPersonActions.Player.Look;
        jump = thirdPersonActions.Player.Jump;
        sprint = thirdPersonActions.Player.Sprint;
        primaryShoot = thirdPersonActions.Player.PrimaryShoot;

        Cursor.visible = false;

        logger = new Logger(Debug.unityLogger);
    }

    private void OnEnable()
    {
        jump.started += Jump;
        thirdPersonActions.Player.Enable();
        primaryShoot.started += PrimaryShoot;
    }


    private void OnDisable()
    {
        jump.started -= Jump;
        thirdPersonActions.Player.Disable();

    }

    private void FixedUpdate()
    {
        forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
        forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;

        rb.AddForce(forceDirection, ForceMode.Impulse);
        forceDirection = Vector3.zero;
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        if (horizontalVelocity.magnitude > maxSpeed * maxSpeed)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
            rb.linearVelocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.linearVelocity.y;
        }


        if (rb.linearVelocity.y < -0.1f && !isGrounded)
        {
            isJumping = false;
            isfalling = true;
        }
        else if (isGrounded)
        {
            isfalling = false;
        }

        LookAt();
        AlignToCamera();
        IsGrounded();
        AnimationJumpManager();
    }

    private void LookAt()
    {
        // Récupère l'orientation avant de la caméra
        Vector3 cameraForward = GetCameraForward(playerCamera);

        // Oriente toujours le personnage vers l'avant de la caméra
        if (cameraForward.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;
        return right.normalized;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded && !isJumping)
        {
            forceDirection += Vector3.up * jumpForce;
            isJumping = true;
            isGrounded = false;
            isfalling = false;
        }
    }

    private void PrimaryShoot(InputAction.CallbackContext context)
    {
        GameObject shot = Instantiate(projectile, firePoint.position, transform.rotation);
        shot.GetComponent<Rigidbody>().AddForce(transform.forward * 10, ForceMode.Impulse);
    }

    private void IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        Debug.DrawRay(this.transform.position + Vector3.up * 0.25f, Vector3.down, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
        {
            if (!isGrounded)
            {
                Debug.Log("Player landed");
                isGrounded = true;
                isJumping = false;
                isfalling = false;
                rb.linearDamping = 10f;
            }

        }
        else
        {
            isGrounded = false;

            Vector3 velocity = rb.linearVelocity;
            Debug.Log("rb.linearVelocity.y = " + velocity.y);
            if (velocity.y < -0f)
            {
                rb.linearDamping = 0f;
                isJumping = false;
                isfalling = true;
            }
            else
            {
                rb.linearDamping = 10f;
            }
        }
    }

    private void AlignToCamera()
    {
        Vector3 cameraForward = GetCameraForward(playerCamera);

        if (cameraForward.sqrMagnitude > 0.1f)
        {
            cameraForward.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void AnimationJumpManager()
    {
        animator.SetBool("isJumping", isJumping);
        animator.SetBool("isFalling", isfalling);
        animator.SetBool("isGrounded", isGrounded);
    }
}
