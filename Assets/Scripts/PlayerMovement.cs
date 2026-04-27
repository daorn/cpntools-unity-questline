using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float rotationSpeed;

    private Rigidbody rb;
    private Vector2 movementValue;
    private float lookValue;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    private void OnMove(InputValue value)
    {
        movementValue = value.Get<Vector2>() * speed;
    }

    private void OnLook(InputValue value)
    {
        lookValue = value.Get<Vector2>().x * rotationSpeed;
    }

    private void Update()
    {
        transform.Translate(movementValue.x * Time.deltaTime, 0, movementValue.y * Time.deltaTime);
        transform.Rotate(0, lookValue * Time.deltaTime, 0);
    }
}
