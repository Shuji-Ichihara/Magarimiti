using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;


public class SoundTest : MonoBehaviour
{
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U))
        { 
            //SoundManager.instance.PlayBGM(SoundManager.E_BGM.InGameBGM);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            SoundManager.instance.FadeIOut(null,3).Forget();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            //SoundManager.instance.PlaySE(SoundManager.E_SE.GetKey);
        }
            
    }
}