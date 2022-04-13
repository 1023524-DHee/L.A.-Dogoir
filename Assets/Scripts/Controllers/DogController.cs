using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class DogController : MonoBehaviour
{
    public Animator anim;
    public NavMeshAgent dog;
    public Transform camPivot;
    public float rotateSpeed;

    private bool canMove = true; 
    private PhotonView pview;
    private float speed;

    private void Awake ()
    {
        pview = GetComponent<PhotonView> ();
        speed = dog.speed;
    }

    private void Start ()
    {
        if (EventManager.I != null)
            EventManager.I.PlayerSpawned (PlayerSpecies.Dog, dog.gameObject);

        if (!pview.IsMine)
        {
            Destroy (GetComponentInChildren<Camera> ().gameObject);
            Destroy (this);
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Player_StaticActions.OnDisableDogMovement += DisableMovement;
        Player_StaticActions.OnEnableDogMovement += EnableMovement;
    }

    private void Update ()
    {
        if (!canMove) return;
        
        Vector3 input = Quaternion.Euler (0, camPivot.rotation.eulerAngles.y, 0) * 
            new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
        float angleDif = (1 - (Vector3.Angle (input, dog.transform.forward) / 180f));

        if (dog.desiredVelocity.magnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.Euler (dog.transform.rotation.eulerAngles.x,
                input.Angle(),//dog.desiredVelocity.Angle (),
                dog.transform.rotation.eulerAngles.z);

            dog.transform.rotation = Quaternion.Lerp (dog.transform.rotation, targetRot, Time.deltaTime * rotateSpeed * (angleDif + 0.2f) / 1.2f);
        }

        dog.speed = Mathf.Lerp (0.1f, speed, angleDif);

        //dog.SetDestination (dog.transform.position + input.normalized);
        dog.SetDestination (dog.transform.position + dog.transform.forward * input.magnitude);
        anim.SetFloat ("MoveSpeed", dog.velocity.magnitude);
        //print (anim.GetFloat ("MoveSpeed"));
        Debug.DrawRay (dog.transform.position + input, Vector3.up, Color.blue);
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