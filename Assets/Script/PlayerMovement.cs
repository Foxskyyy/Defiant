using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody rb;
    private Animator animator;

    private ShiftLockToggle cameraController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation; // supaya tidak jatuh miring

        animator = GetComponentInChildren<Animator>();

        cameraController = Camera.main.GetComponent<ShiftLockToggle>();
        if (cameraController == null)
        {
            Debug.LogError("ShiftLockToggle script not found on Main Camera!");
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 moveDirection = Vector3.zero;

        if (cameraController != null)
        {
            Vector3 camForward = cameraController.GetCameraForwardOnPlane();
            Vector3 camRight = cameraController.GetCameraRightOnPlane();

            moveDirection = (camForward * v + camRight * h).normalized;

            if (cameraController.IsShiftLocked && moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
            }
            else if (!cameraController.IsShiftLocked && moveDirection != Vector3.zero)
            {
                // Saat free, rotasi juga (optional)
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f));
            }
        }

        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);

        if (animator != null)
        {
            animator.SetFloat("Speed", moveDirection.magnitude);
        }
    }
}
