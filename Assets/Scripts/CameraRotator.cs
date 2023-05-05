using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotator : MonoBehaviour
{
    public float rotationSpeed = 1f; // Speed of rotation in degrees per second
    private int currentRotationIndex = 0; // Index of the current rotation
    private float[] cameraRotations;

    void Start()
    {
        cameraRotations = new float[] { 0f, 180f };

        // Set the initial camera rotation based on the first corner
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, cameraRotations[currentRotationIndex], transform.eulerAngles.z);
    }

    void Update()
    {
        // Check for user input to rotate the camera and grid
        if (Input.GetKeyDown(KeyCode.R)) // Change this to your desired input key or button
        {
            currentRotationIndex = (currentRotationIndex + 1) % 2; // Increment rotation index and wrap around to 0 if it exceeds 3
            StartCoroutine(RotateCamera()); // Start the rotation coroutine
        }
    }

    private IEnumerator RotateCamera()
    {
        float initialRotationY = transform.eulerAngles.y; // Store the initial rotation of the camera
        float targetRotationY = cameraRotations[currentRotationIndex]; // Get the target rotation for the current corner
        float t = 0;

        while (t < 1)
        {
            t += Time.deltaTime * rotationSpeed; // Increment the time variable
            float currentRotationY = Mathf.Lerp(initialRotationY, targetRotationY, t); // Calculate the current rotation of the camera using linear interpolation

            // Apply the new rotation
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, currentRotationY, transform.eulerAngles.z);

            yield return null; // Wait for the next frame
        }

        // Snap to the target rotation to avoid floating-point errors
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, targetRotationY, transform.eulerAngles.z);
    }
}
