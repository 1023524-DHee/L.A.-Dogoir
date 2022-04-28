using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorToManager : MonoBehaviour
{
    public Cutscene_Manager cutsceneManager;
    
    public void CutsceneFinished()
    {
        cutsceneManager.CutsceneEventFinished();
    }
}
