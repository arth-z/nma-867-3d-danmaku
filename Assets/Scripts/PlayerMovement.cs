using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
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

    float dashTimer = 0.3f;
    float dashCoeff = 1f;

    float friction = 1.2f;
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
        dashAction.performed += ctx => Dash();

        clickAction = InputSystem.actions.FindAction("Attack");
        clickAction.performed += ctx => Attack();

        lookAction.Enable();
        moveHorizontal.Enable();
        moveVertical.Enable();
        focusAction.Enable();
        clickAction.Enable();

        delta_speed = NORMAL_SPEED;
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

    void Move()
    {
        Vector2 movementH = moveHorizontal.ReadValue<Vector2>();
        f = movementH.y;
        s = movementH.x;
        Vector3 accel = new Vector3(0, 0, 0);

        if (f != 0)
        {
            accel += transform.forward * f * Time.deltaTime * delta_speed;
        }

        if (s != 0) 
        {
            accel += transform.right * s * Time.deltaTime * delta_speed;
        }
 

        Vector2 movementV = moveVertical.ReadValue<Vector2>();
        up = movementV.y;

        if (up != 0) 
        {
            accel += transform.up * up * Time.deltaTime * delta_speed;
        }

        accel = accel.normalized * delta_speed;

        velocity += accel * Time.deltaTime;
        velocity /= friction; // air res
        if (velocity.magnitude < 0)
        {
            velocity = new Vector3(0, 0, 0);
        }

        transform.position += velocity * dashCoeff * Time.deltaTime;

    }

    void Focus()
    {
        friction = 1.3f;
        delta_speed = NORMAL_SPEED * 0.7f;
    }

    void Unfocus()
    {
        friction = 1.1f;
        delta_speed = NORMAL_SPEED;
    }

    void Dash()
    {
        if (dashTimer >= 0.3f) dashTimer = 0f;
    }
    
    void DashUpdate()
    {
        if (dashTimer < 0.3f)
        {
            dashTimer += Time.deltaTime;
            dashCoeff = 3f;
        }
        else
        {
            dashCoeff = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        MouseLook();
        DashUpdate();
        Move();
    }

    void FixedUpdate()
    {
        
    }

    void Attack()
    {
    }
}
