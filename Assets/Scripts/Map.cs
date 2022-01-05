using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public Sprite def;
    public Sprite top;
    public Sprite jgl;
    public Sprite mid;
    public Sprite bot;

    static Image map;
    static Map @this;

    public delegate void mapMethod();


    void Start()
    {
        map = GetComponent<Image>();
        @this = this;
    }

    public static void SetDef()
    {
        map.sprite = @this.def;
    }
    public static void SetTop()
    {
        map.sprite = @this.top;
    }
    public static void SetJgl()
    {
        map.sprite = @this.jgl;
    }
    public static void SetMid()
    {
        map.sprite = @this.mid;
    }
    public static void SetBot()
    {
        map.sprite = @this.bot;
    }

}
