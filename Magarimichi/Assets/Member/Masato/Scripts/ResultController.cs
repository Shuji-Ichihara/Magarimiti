using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.BGMPause();
        SoundManager.instance.PlayBGM(SoundManager.E_BGM.OutGameBGM);
    }
}
