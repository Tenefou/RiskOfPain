using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuckAnimator : MonoBehaviour
{
    private Animator animator;

    private ThirdPersonActions controls;
    private Vector2 inputVector;
    private Vector2 smoothInputVector;
    private Vector2 inputVelocity;

    private void Awake()
    {
        controls = new ThirdPersonActions();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Player.Move.performed += OnMove;
        controls.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove;
        controls.Player.Move.canceled -= OnMove;
        controls.Disable();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();

        // Normalisation pour obtenir des valeurs discr�tes de 1 lorsqu'on appuie sur deux touches
        if (inputVector != Vector2.zero)
        {
            inputVector.Normalize();
            inputVector *= Mathf.Sqrt(2); // Multiplie par la racine carr�e de 2 pour atteindre 1
        }
    }

    private void Update()
    {
        // Lissage des entr�es
        smoothInputVector = Vector2.SmoothDamp(smoothInputVector, inputVector, ref inputVelocity, 0.1f);

        // Mise � jour des param�tres du Blend Tree
        animator.SetFloat("x", smoothInputVector.x);
        animator.SetFloat("z", smoothInputVector.y);
    }
}
