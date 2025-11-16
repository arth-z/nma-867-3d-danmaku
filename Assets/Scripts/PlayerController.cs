using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    // for mouselook
    float sens = 0.1f;
    float h = 0; // horizontal
    float v = 0; // vertical
    InputAction lookAction;
    InputAction clickAction;

    // keyboard movement
    InputAction moveHorizontal;
    InputAction moveVertical;
    InputAction dashAction;
    InputAction focusAction;
    float f = 0; // forward
    float s = 0; // side
    float up = 0; // up/down
    float NORMAL_SPEED = 500f;
    float delta_speed;
    Vector3 accel;

    // dashing
    float dashTimer = 0.3f;
    float dashCoeff = 1f;
    bool dashing = false;

    Rigidbody rb;
    Collider col;

    // gameplay systems
    int health = 10;
    float iFrameTimer = 0f;
    CinemachineImpulseSource impulse;

    float drag;
    float AIR_RES = 5f;
    Vector3 velocity = new Vector3(0, 0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        moveHorizontal = InputSystem.actions.FindAction("MoveHorizontal");
        moveVertical = InputSystem.actions.FindAction("MoveVertical");

        focusAction = InputSystem.actions.FindAction("Focus");
        focusAction.started += ctx => Focus();
        focusAction.canceled += ctx => Unfocus();

        dashAction = InputSystem.actions.FindAction("Dash");
        dashAction.started += ctx => Dash();
        dashAction.canceled += ctx => StopDash();

        clickAction = InputSystem.actions.FindAction("Attack");
        clickAction.performed += ctx => Attack();

        lookAction.Enable();
        moveHorizontal.Enable();
        moveVertical.Enable();
        focusAction.Enable();
        clickAction.Enable();
        dashAction.Enable();

        delta_speed = NORMAL_SPEED;
        drag = AIR_RES;

        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    void MouseLook()
    {
        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        h += lookValue.x * sens;
        v += lookValue.y * sens;

        // Clamp pitch to avoid issues and sign flips at +/- 90
        v = Mathf.Clamp(v, -89, 89);

        // spherical coordinates because euler rotations cause some weeeeeird stuff
        var theta = h * Mathf.Deg2Rad;
        var phi = v * Mathf.Deg2Rad;

        var sinTheta = Mathf.Sin(theta);
        var cosTheta = Mathf.Cos(theta);
        var sinPhi = Mathf.Sin(phi);
        var cosPhi = Mathf.Cos(phi);

        // Convert from spherical to cartesian and directly assign to up. 
        var fwd = new Vector3(cosPhi * sinTheta, sinPhi, cosPhi * cosTheta);
        transform.forward = fwd;

        var up = new Vector3(-sinPhi * sinTheta, cosPhi, -sinPhi * cosTheta);
        transform.rotation = Quaternion.LookRotation(fwd, up);
    }

    void MotionControl()
    {
        Vector2 movementH = moveHorizontal.ReadValue<Vector2>();
        Vector2 movementV = moveVertical.ReadValue<Vector2>();
        f = movementH.y;
        s = movementH.x;
        up = movementV.y;

        // construct acceleration unit vector from inputs and normalise it, then multiply it by delta_speed
        accel = transform.forward * f + transform.right * s + transform.up * up;
        if (accel != Vector3.zero) accel.Normalize(); 
        accel *= delta_speed;

        transform.position += dashCoeff * Time.deltaTime * velocity + (0.5f * Time.deltaTime * Time.deltaTime * accel);

        // change velocity by acceleration
        velocity += accel * Time.deltaTime;
        // air resistance or something
        velocity /= 1 + drag * Time.deltaTime;


    }

    void Focus()
    {
        drag = AIR_RES*4f;
        delta_speed = NORMAL_SPEED * 0.7f;
    }

    void Unfocus()
    {
        drag = AIR_RES;
        delta_speed = NORMAL_SPEED;
    }

    void Dash()
    {
        if (dashTimer >= 0.3f) dashTimer = 0f;
        dashing = true;
    }

    void StopDash()
    {
        dashing = false;
    }

    void DashUpdate()
    {
        if (dashTimer < 0.3f)
        {
            dashTimer += Time.deltaTime;
            dashCoeff = dashCoeff >= 3f ? 3f : dashCoeff + dashTimer * 1500f * Time.deltaTime;
        }
        else
        {
            if (dashCoeff > 1.5f) dashCoeff -= Time.deltaTime;
            if (dashing) {dashCoeff = 1.5f;} else {dashCoeff = 1f;}
        }
    }

    void IFrameUpdate()
    {
        if (iFrameTimer > 0f)
        {
            iFrameTimer -= Time.deltaTime;
            if (iFrameTimer < 0f) iFrameTimer = 0f;
        }
    }

    public Vector3 getVelocity()
    {
        return velocity;
    }

    public Vector3 getAccel()
    {
        return accel;
    }

    public Vector3 getPosition()
    {
        return transform.position;
    }

    public Vector3 getForward()
    {
        return transform.forward;
    }

    public float getDeltaSpeed()
    {
        return delta_speed;
    }

    public float getDashCoeff()
    {
        return dashCoeff;
    }

    public float getDashTimer()
    {
        return dashTimer;
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        DashUpdate();
        MotionControl();
        IFrameUpdate();
    }

    // update is called after all Update functions have been called
    void LateUpdate()
    {

    }

    void FixedUpdate()
    {
    }

    void Attack()
    {
    }

    public void TakeDamage(BulletController bulletOther)
    {
        if (iFrameTimer == 0f)
        {
            impulse.GenerateImpulseAtPositionWithVelocity(transform.position, bulletOther.getVelocity().normalized);
            health -= 1;
            print(health);
            if (health <= 0) Destroy(gameObject);
            iFrameTimer = 2f;
        }
    }

    // hit to handle slow bullets -- see BulletController.cs to see how fast bullets are handled
    public void OnTriggerEnter(Collider other)
    {
        if (iFrameTimer != 0f) return;
        BulletController bulletOther = other.GetComponent<BulletController>();
        if (bulletOther == null) return;
        if (bulletOther.isFriendly()) return;
        TakeDamage(bulletOther);
    }
}