using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    //呼び出し方の例です。
    //ゲームクリア時は変わると思いますが、仕組みは大体一緒なので、大丈夫だと思います。
    //わからなければ、早めの連絡を

    GameObject Scene;

    public int Noumber;

    public void Test()
    {
        Fade_Controller.IsFadeOut = true;
        StartCoroutine("SceneMove");
    }

    IEnumerator SceneMove()
    {
        yield return new WaitForSeconds(3);

        Scene = GameObject.FindGameObjectWithTag("ScneManger");
        switch (Noumber)
        {
            case 0:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.TitleScene);
                break;
            case 1:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.StageSelect);
                break;
            case 2:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.Main1);
                break;
            case 3:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.Main2);
                break;
            case 4:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.Main3);
                break;
            case 5:
                Scene.GetComponent<SceneManger>()._LoadScene(Name.ResultScene);
                break;
        }
    }
}
