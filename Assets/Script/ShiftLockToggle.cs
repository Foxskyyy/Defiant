using UnityEngine;

public class ShiftLockToggle : MonoBehaviour
{
    public Transform target;
    public float distance = 4f;
    public float height = 2f;
    public float mouseSensitivity = 1f;    // Sensitivitas dikurangi
    public float offsetLerpSpeed = 10f;

    public Vector3 lockedShoulderOffset = new Vector3(0.6f, 1.6f, -2.5f); // Bahu belakang kanan

    private bool isLocked = false;
    private float yaw = 0f;
    private float pitch = 15f;

    private Quaternion targetRotation;
    private Vector3 targetPosition;

    private bool isFreeLooking = false;

    public bool IsShiftLocked => isLocked;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        targetRotation = transform.rotation;
        targetPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isLocked = !isLocked;

            if (isLocked)
            {
                yaw = target.eulerAngles.y;
                pitch = 15f;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        if (isLocked)
        {
            LockedCamera();
        }
        else
        {
            // Aktifkan free look saat klik kanan ditekan
            if (Input.GetMouseButtonDown(1))
                isFreeLooking = true;
            if (Input.GetMouseButtonUp(1))
                isFreeLooking = false;

            if (isFreeLooking)
            {
                yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
                pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
                pitch = Mathf.Clamp(pitch, -10f, 60f);
            }

            FreeFollowCamera();
        }
    }

    void FreeFollowCamera()
    {
        Quaternion desiredRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 desiredPosition = target.position
                                - (desiredRotation * Vector3.forward * distance)
                                + Vector3.up * height;

        targetPosition = Vector3.Lerp(targetPosition, desiredPosition, Time.deltaTime * offsetLerpSpeed);

        Vector3 lookAtPoint = target.position + Vector3.up * 1.5f;
        targetRotation = Quaternion.Lerp(
            targetRotation,
            Quaternion.LookRotation(lookAtPoint - targetPosition),
            Time.deltaTime * offsetLerpSpeed);

        ApplyTransform();
    }

    void LockedCamera()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, -10f, 60f);

        Quaternion cameraRotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = cameraRotation * lockedShoulderOffset;

        Vector3 desiredPosition = target.position + offset;
        Quaternion desiredRotation = cameraRotation;

        targetPosition = Vector3.Lerp(targetPosition, desiredPosition, Time.deltaTime * offsetLerpSpeed);
        targetRotation = Quaternion.Lerp(targetRotation, desiredRotation, Time.deltaTime * offsetLerpSpeed);

        ApplyTransform();
    }

    void ApplyTransform()
    {
        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }

    public Vector3 GetCameraForwardOnPlane()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    public Vector3 GetCameraRightOnPlane()
    {
        Vector3 right = transform.right;
        right.y = 0;
        return right.normalized;
    }
}
