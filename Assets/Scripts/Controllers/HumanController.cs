using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class HumanController : MonoBehaviour
{
    public Animator anim;
    public Transform cam;
    public NavMeshAgent agent;
    public GameObject avatar;
    public Transform headLookTarget;
    public float rotateSpeed;

    public GameObject displayText;
    public GameObject humanCanvas;

    [SerializeField] private Vector2 verticalLookRange, lookSensitivity;

    private bool canMove = true;
    private PhotonView pview;
    private float pitch;

    private void Awake ()
    {
        pview = GetComponent<PhotonView> ();
    }

    private void Start ()
    {
        if (EventManager.I != null)
            EventManager.I.PlayerSpawned (PlayerSpecies.Human, avatar);

        if (!pview.IsMine)
        {
            Destroy (GetComponentInChildren<Camera> ().gameObject);
            Destroy (this);
            return;
        }
        else
        {
            foreach (SkinnedMeshRenderer renderer in avatar.GetComponentsInChildren<SkinnedMeshRenderer> ())
                renderer.enabled = false;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Player_StaticActions.OnDisableHumanMovement += DisableMovement;
        Player_StaticActions.OnEnableHumanMovement += EnableMovement;
    }

    private void Update ()
    {
        if (!canMove) return;
        Look ();
        Move ();
        PlayerRaycast();
    }

    private void Look ()
    {
        Vector2 lookInput = new Vector2 (Input.GetAxis ("Look X"), Input.GetAxis ("Look Y"));

        float yRot = cam.transform.rotation.eulerAngles.y + lookInput.x * lookSensitivity.x;
        pitch = Mathf.Clamp (pitch - lookInput.y * lookSensitivity.y, verticalLookRange.x, verticalLookRange.y);

        //Vector3 newRot = cam.transform.rotation.eulerAngles;
        //newRot.y = yRot;
        //newRot.x = pitch;

        cam.transform.localRotation = Quaternion.Euler (pitch, 0, 0);
        agent.transform.rotation = Quaternion.Euler (0, yRot, 0);
        headLookTarget.position = cam.transform.position + cam.transform.forward;
    }

    private void Move ()
    {
        Vector3 input = Quaternion.Euler (0, cam.rotation.eulerAngles.y, 0) * 
            new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));

        //if (input.sqrMagnitude > 0.1f)
            agent.SetDestination (agent.transform.position + input.normalized * 0.25f);

        anim.SetFloat ("MoveSpeed", Mathf.Clamp01 (agent.velocity.magnitude / (agent.speed / 3)));
    }

    private void PlayerRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, 5f))
        {
            if (hit.collider.TryGetComponent<IInteractable> (out IInteractable interactable))
            {
                displayText.SetActive(true);
                if (Input.GetKeyDown(KeyCode.E))
                    interactable.Interact();
            }
            else displayText.SetActive(false);
        }
    }

    private void EnableMovement()
    {
        canMove = true;
    }
    
    private void DisableMovement()
    {
        canMove = false;
    }
}