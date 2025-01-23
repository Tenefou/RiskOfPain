using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;


public class BuckAnimator : MonoBehaviour
{
    private Animator animator;

    private ThirdPersonActions controls; // Référence à l'asset généré
    private Vector2 inputVector;
    private Vector2 smoothInputVector;
    private Vector2 inputVelocity;



    private void Awake()
    {
        controls = new ThirdPersonActions(); // Initialise l'asset généré
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        controls.Enable(); // Active les actions
        controls.Player.Move.performed += OnMove; // Associe l'événement
        controls.Player.Move.canceled += OnMove; // Gère l'arrêt du mouvement
    }

    private void OnDisable()
    {
        controls.Player.Move.performed -= OnMove; // Désassocie l'événement
        controls.Player.Move.canceled -= OnMove;
        controls.Disable(); // Désactive les actions
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        inputVector = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        // Lissage des entrées
        smoothInputVector = Vector2.SmoothDamp(smoothInputVector, inputVector, ref inputVelocity, 0.1f);

        // Mise à jour des paramètres du Blend Tree
        animator.SetFloat("x", smoothInputVector.x);
        animator.SetFloat("z", smoothInputVector.y);
    }
}
