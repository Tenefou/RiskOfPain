using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;


public class BuckAnimator : MonoBehaviour
{
    private Animator animator;

    private ThirdPersonActions controls; // R�f�rence � l'asset g�n�r�
    private Vector2 inputVector;
    private Vector2 smoothInputVector;
    private Vector2 inputVelocity;



    private void Awake()
    {
        controls = new ThirdPersonActions(); // Initialise l'asset g�n�r�
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable(); // Active les actions
        controls.Player.Move.performed += OnMove; // Associe l'�v�nement
        controls.Player.Move.canceled += OnMove; // G�re l'arr�t du mouvement
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove; // D�sassocie l'�v�nement
        controls.Player.Move.canceled -= OnMove;
        controls.Disable(); // D�sactive les actions
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
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
