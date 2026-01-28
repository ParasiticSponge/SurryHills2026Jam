using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


public class PlayerBehaviour : MonoBehaviour
{
    private PlayerInput playerInput;
    InputAction admit;
    InputAction decline;

    GameObject interaction;

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
        Actions.collision += Collision;
    }

    void OnDisable()
    {
        admit.performed -= OnAdmit;
        decline.performed -= OnDecline;
        Actions.collision -= Collision;
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
        if (interaction != null)
        {
            interaction.GetComponent<EntityBehaviour>().TriggerAdmit.Invoke(1);
            Debug.Log("Admitted");
        }
        interaction = null;
    }

    private void OnDecline(InputAction.CallbackContext context)
    {
        if (interaction != null)
        {
            interaction.GetComponent<EntityBehaviour>().TriggerAdmit.Invoke(-1);
            Debug.Log("Declined");
        }
        interaction = null;
    }

    void Collision(GameObject collider)
    {
        EntityBehaviour comp = collider.GetComponent<EntityBehaviour>();
        if (comp != null && comp.canInteract == true)
        {
            print("found object");
            interaction = collider;
        }
    }
}
