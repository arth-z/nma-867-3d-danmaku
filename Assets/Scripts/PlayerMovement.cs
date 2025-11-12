using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    InputAction lookAction;
    InputAction moveAction;
    InputAction clickAction;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        lookAction = InputSystem.actions.FindAction("Look");
        moveAction = InputSystem.actions.FindAction("Move");
        clickAction = InputSystem.actions.FindAction("Click");
        clickAction.performed += ctx => clicked();
        lookAction.Enable();
        moveAction.Enable();
        clickAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void clicked()
    {
        Debug.Log("Clicked");
    }
}
