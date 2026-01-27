using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerBehaviour : MonoBehaviour
{
    private PlayerInput playerInput;
    InputAction admit;
    InputAction decline;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        admit = playerInput.actions["Admit"];
        decline = playerInput.actions["Decline"];
    }
    void OnEnable()
    {
        admit.performed += OnAdmit;
        decline.performed += OnDecline;
    }

    void OnDisable()
    {
        admit.performed -= OnAdmit;
        decline.performed -= OnDecline;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnAdmit(InputAction.CallbackContext context)
    {
        Debug.Log("Admit triggered");
    }

    private void OnDecline(InputAction.CallbackContext context)
    {
        Debug.Log("Decline triggered");
    }

}
