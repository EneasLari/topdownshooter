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
    public Transform FPSCameraPosition;

    //FPS VARIABLES
    public float mouseSensitivityX = 250;
    public float mouseSensitivityY = 250;
    public float jumpForce = 220;
    public LayerMask groundedMask;
    float verticalLookRotation;
    bool grounded;


    public float moveSpeed = 5;
    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    private Vector3 initialcameraoffset;

    //we override the (virtual) start method of LivingEntity and we call it here with base.Start 
    protected override void Start()
    {
        base.Start();
    }

    void Awake()
    {
        gunController = GetComponent<GunController>();
        controller = GetComponent<PlayerController>();
        SetViewCamera();
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber) {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void SetViewCamera() {
        TopDownCamera.gameObject.SetActive(false);
        FPSCamera.gameObject.SetActive(false);
        if (currentView == ViewMode.TopDown)
        {
            viewCamera = TopDownCamera;//Camera.main;
            //FOR TOPDOWN: the camera follows the player in z and x axis 
            initialcameraoffset = (viewCamera.transform.position - controller.transform.position);
        }
        else if (currentView == ViewMode.FPS)
        {
            UnityEngine.Cursor.lockState = CursorLockMode.Locked;
            viewCamera = FPSCamera;//Camera.main;
            //make player parent of camera
            viewCamera.transform.parent = FPSCameraPosition;
            viewCamera.transform.localPosition = Vector3.zero;

        }
        viewCamera.gameObject.SetActive(true);
        viewCamera.tag = "MainCamera";
    }

    void FixedUpdate()
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
        RaycastHit aimhit;
        //set a default point at 100 meter distance where the ray will hit
        Vector3 point = ray.GetPoint(100);
        if (Physics.Raycast(ray, out aimhit, 100f))
        {
            Debug.DrawLine(ray.origin, aimhit.point, Color.red);
            //place crosshairs at the end of the ray point
            gunController.Aim(aimhit.point);
        }
        else {
            Debug.DrawLine(ray.origin, point, Color.red);      
            gunController.Aim(point);
        }

        crosshairs.SetFPSCrosshair(viewCamera);
        crosshairs.DetectTargets(ray);
        


        // Jump
        if (Input.GetButtonDown("Jump"))
        {
            if (grounded)
            {
                GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            }
        }

        // Grounded check
        Ray groundedray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(groundedray, out hit, 1 + .1f, groundedMask))
        {
            grounded = true;
        }
        else
        {
            grounded = false;
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
        // If player falls he dies
        if (transform.position.y < -10)
        {
            TakeDamage(health);
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

        //make camera follow the player
        float newx = (initialcameraoffset.x) + controller.transform.position.x;
        float newz = (initialcameraoffset.z) + controller.transform.position.z;
        Vector3 newpos = new Vector3(newx, viewCamera.transform.position.y, newz);
        Vector3 newpostionofcamera = Vector3.Lerp(viewCamera.transform.position, newpos, 1 * Time.fixedDeltaTime);
        viewCamera.transform.position = newpostionofcamera;

    }

    public override void Die()
    {
        viewCamera.transform.parent = null;
        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}