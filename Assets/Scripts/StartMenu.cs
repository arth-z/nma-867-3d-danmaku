using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{

    InputAction enterAction;
    InputAction escapeAction;

    void Start()
    {
        enterAction = new InputAction("Enter", binding: "<Keyboard>/enter");
        enterAction.performed += ctx => Entered();
        enterAction.Enable();

        escapeAction = new InputAction("Escape", binding: "<Keyboard>/escape");
        escapeAction.performed += ctx => Escaped();
        escapeAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void Entered()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    void Escaped()
    {
        Application.Quit();
    }
}
