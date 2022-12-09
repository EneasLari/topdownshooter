using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity
{
    public enum ViewMode { TopDown, FPS, ThirdPerson }
    public ViewMode currentView;

    public Camera TopDownCamera;
    public Camera FPSCamera;

    //FPS VARIABLES
    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    public float jumpForce = 220;
    float verticalLookRotation;


    public float moveSpeed = 5;

    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;
    //we override the (virtual) start method of LivingEntity and we call it here with base.Start 
    protected override void Start()
    {
        base.Start();
        TopDownCamera.gameObject.SetActive(false);
        FPSCamera.gameObject.SetActive(false);

        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        if (currentView == ViewMode.TopDown)
        {
            viewCamera = TopDownCamera;//Camera.main;
        }
        else if (currentView == ViewMode.FPS)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            crosshairs.transform.Rotate(new Vector3(90, 0, 0));
            viewCamera = FPSCamera;//Camera.main;
        }
        viewCamera.gameObject.SetActive(true);
    }

    void Update()
    {
        if (currentView == ViewMode.TopDown)
        {
            TopDownMovement();
        }
        else if (currentView == ViewMode.FPS)
        {
            FPSMovement();
        }



    }

    private void FPSMovement()
    {
        // Look rotation:
        transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime);
        verticalLookRotation += Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -60, 60);
        viewCamera.transform.localEulerAngles = Vector3.left * verticalLookRotation;

        // Calculate movement:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        Vector3 moveVelocity = new Vector3(inputX, 0, inputY).normalized * moveSpeed;
        Vector3 localMove = transform.TransformDirection(moveVelocity);
        controller.Move(localMove);

        // Look input
        Ray ray = viewCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        Vector3 point = ray.GetPoint(20);
        Debug.DrawLine(ray.origin, point, Color.red);
        //place crosshairs at the end of the ray point
        crosshairs.transform.position = point;
        //make crosshair to look at(Z axis points camera) camera
        crosshairs.transform.LookAt(viewCamera.transform);
        crosshairs.DetectTargets(ray);
        gunController.Aim(point);

        // Weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }


    }

    private void TopDownMovement()
    {
        // Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);

        // Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);
            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 2)
            {
                gunController.Aim(point);
            }
        }

        // Weapon input
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
    }
}