using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class NPCCharacterController : MonoBehaviour
{
    private CharacterController characterController;

    //Since this is an NPC, we'll make it as generic as possible.
    public float movementSpeed = 4f;
    public float rotationSpeed = 10f;

    public enum characterBehavior { Friendly, Hostile };
    public enum characterMovement { Idle, Wandering, Chasing };

    [Header("Behaviors")]
    public float sightRange = 10.0f;
    public characterBehavior activeBehavior;
    public characterMovement currentMovement;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        
    }
}
