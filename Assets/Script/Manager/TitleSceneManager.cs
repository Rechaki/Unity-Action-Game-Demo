using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneManager : MonoBehaviour
{
    //パネルのオブジェクト
    public Image PanelImage;
    //暗転するスピード
    [SerializeField] private float alphaSpeed;
    private bool isSceneChange;
    private bool isChangeEnd;
    private float alphaValue;
    private Color PanelColor;
    // 点滅させる対象
    [SerializeField] private Image _target;

    // 点滅周期[s]
    [SerializeField] private float _cycle = 1;

    private float _time;

    public void StartGame()
    {
        //パネルイメージを有効にする
        PanelImage.enabled = true;
        //シーンチェンジ起動
        isSceneChange = true;
    }
    public void EndGame()
    {
        Application.Quit();
    }
    // Start is called before the first frame update
    void Start()
    {
        isSceneChange = false;
        isChangeEnd = false;
        alphaValue = 0f;
        PanelColor = PanelImage.color;
    }

    // Update is called once per frame
    private void Update()
    {
        //シーンチェンジが起動しているとき
        //暗転機能
        if (isSceneChange)
        {
            alphaValue += alphaSpeed * Time.deltaTime;
            PanelImage.color = new Color(PanelColor.r, PanelColor.g, PanelColor.b, alphaValue);
            if (alphaValue >= 1)
            {
                isSceneChange = false;
                //シーン切り替えフラグ
                isChangeEnd = true;
            }
        }
        //シーン切り替え
        if (isChangeEnd)
        {
            SceneManager.LoadScene("Game");
        }
        // 内部時刻を経過させる
        _time += Time.deltaTime;

        // 周期cycleで繰り返す波のアルファ値計算
        var alpha = Mathf.Cos(2 * Mathf.PI * _time / _cycle) * 0.5f + 0.5f;

        // 内部時刻timeにおけるアルファ値を反映
        var color = _target.color;
        color.a = alpha;
        _target.color = color;
    }
}
