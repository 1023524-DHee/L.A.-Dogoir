using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;


public class SceneTransition : MonoBehaviour
{
    public List<GameObject> goToEnable;
    
    private PhotonView _photonView;
    
    private bool isEnabled;
    private bool dogPresent;
    private bool humanPresent;

    private void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        Invoke(nameof(DisableObjects), 2f);
    }

    private void DisableObjects()
    {
        Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        hashtable["DogReady"] = false;
        hashtable["HumanReady"] = false;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        foreach(GameObject go in goToEnable) go.SetActive(false);
    }
    
    private void Update()
    {
        if (!isEnabled) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (dogPresent && PlayerManager.ThisPlayer == PlayerSpecies.Dog)
            {
                Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
                hashtable["DogReady"] = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }
            else if (humanPresent && PlayerManager.ThisPlayer == PlayerSpecies.Human)
            {
                Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
                hashtable["HumanReady"] = true;
                PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            }

            CheckTransition();
        }
    }

    public void SetEnabled()
    {
        _photonView.RPC(nameof(SetEnabled_RPC), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void SetEnabled_RPC()
    {
        foreach(GameObject go in goToEnable) go.SetActive(true);
        isEnabled = true;
    }
    
    private void CheckTransition()
    {
        Hashtable hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        if((bool) hashtable["HumanReady"] && (bool) hashtable["DogReady"])
        {
            Debug.Log("ASDASD");
            SceneManager.LoadScene("Bar_Scene");
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HumanAgent"))
        {
            humanPresent = true;
        }
        else if (other.CompareTag("DogAgent"))
        {
            dogPresent = true;
        }
    }
}
