using UnityEngine;
using UnityEngine.UI;

public class Fade_Controller : MonoBehaviour
{
    //透明度を変更するパネルのイメージ
    Image fadeImage;

    //フェードイン・アウト処理のフラグ
    private bool IsFadeIn = false;
    public static bool IsFadeOut = false;

    //透明度が変わるスピードを管理
    private float fadeSpeed = 0.01f;
    //パネルの色、不透明度を管理
    private float red, green, blue, alfa;

    // Start is called before the first frame update
    void Start()
    {
        //コンポーネント＜イメージ＞を取得
        fadeImage = GetComponent<Image>();
        red = fadeImage.color.r;    //赤色
        green = fadeImage.color.g;  //緑色
        blue = fadeImage.color.b;   //青色
        alfa = fadeImage.color.a;   //透明度

        IsFadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        //IsFadeInがtrueだったら
        if (IsFadeIn)
        {
            //画面を明るくする
            StartFadeIn();
        }

        //IsFadeOutがtrueだったら
        if (IsFadeOut)
        {
            //画面を暗くする
            StartFadeOut();
        }
    }

    //画面を明るくする処理
    void StartFadeIn()
    {
        alfa -= fadeSpeed;              //不透明度を徐々に下げる
        SetAlpha();                     //変更した不透明度パネルに反映する
        if (alfa <= 0)                  //完全に透明になったら処理を抜ける
        {
            IsFadeIn = false;
            fadeImage.enabled = false;   //パネルの表示をオフにする
        }
    }

    //画面を暗くする処理
    void StartFadeOut()
    {
        fadeImage.enabled = true;       //パネルの表示をオンにする 
        alfa += fadeSpeed;              //不透明度を徐々にあげる
        SetAlpha();                     //変更した透明度をパネルに反映する
        if (alfa >= 1)                  //完全に不透明になったら処理を抜ける
        {
            IsFadeOut = false;
        }
    }

    //変更した値を書き換える処理
    void SetAlpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
    }
}
