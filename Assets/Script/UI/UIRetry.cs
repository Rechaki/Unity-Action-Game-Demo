using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRetry : MonoBehaviour
{
    public Image image;


    void Start()
    {
        image.GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EventON()
    {
        var targetSize = new Vector2(400, 300);
        GetComponent<RectTransform>().sizeDelta = targetSize;
    }
    public void EventEnter()
    {
        var targetSize = new Vector2(300,200);
        GetComponent<RectTransform>().sizeDelta = targetSize;
    }


}
