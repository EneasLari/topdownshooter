using UnityEngine;
using System;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {
    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("References")]
    [SerializeField] Camera viewCamera;
    [SerializeField] Crosshairs crosshairs;

    PlayerController controller;
    GunController gunController;
    Spawner spawner;

    Vector3 initialCameraOffset;

    protected override void Start() {
        base.Start();
    }

    void Awake() {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();

        if (viewCamera == null) {
            Debug.LogError("View Camera is not assigned on Player.");
            enabled = false;
            return;
        }

        if (crosshairs == null) {
            Debug.LogError("Crosshairs is not assigned on Player.");
            enabled = false;
            return;
        }

        initialCameraOffset = viewCamera.transform.position - transform.position;

        spawner = FindObjectOfType<Spawner>();
        if (spawner != null) {
            spawner.OnNewWave += OnNewWave;
        }
        else {
            Debug.LogWarning("Spawner not found in scene.");
        }
    }

    void OnDestroy() {
        if (spawner != null) {
            spawner.OnNewWave -= OnNewWave;
        }
    }

    void OnNewWave(int waveNumber) {
        health = startingHealth;
        gunController.EquipGun(waveNumber - 1);
    }

    void FixedUpdate() {
        HandleMovement();
        HandleAim();
        HandleWeaponInput();
        FollowPlayerWithCamera();
    }

    void HandleMovement() {
        Vector3 moveInput = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical")
        );

        Vector3 moveVelocity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelocity);
    }

    void HandleAim() {
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);

        if (groundPlane.Raycast(ray, out float rayDistance)) {
            Vector3 point = ray.GetPoint(rayDistance);

            Debug.DrawLine(ray.origin, point, Color.red);
            controller.LookAt(point);

            crosshairs.transform.position = point;
            crosshairs.DetectTargets(ray);

            Vector2 playerPosition = new Vector2(transform.position.x, transform.position.z);
            Vector2 aimPosition = new Vector2(point.x, point.z);

            if ((aimPosition - playerPosition).sqrMagnitude > 2f) {
                gunController.Aim(point);
            }
        }
    }

    void HandleWeaponInput() {
        if (Input.GetMouseButton(0)) {
            gunController.OnTriggerHold();
        }

        if (Input.GetMouseButtonUp(0)) {
            gunController.OnTriggerRelease();
        }
    }

    void FollowPlayerWithCamera() {
        float newX = initialCameraOffset.x + transform.position.x;
        float newZ = initialCameraOffset.z + transform.position.z;

        Vector3 targetPosition = new Vector3(newX, viewCamera.transform.position.y, newZ);
        viewCamera.transform.position = Vector3.Lerp(
            viewCamera.transform.position,
            targetPosition,
            Time.fixedDeltaTime
        );
    }

    public override void Die() {
        if (viewCamera != null) {
            viewCamera.transform.parent = null;
        }

        AudioManager.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}