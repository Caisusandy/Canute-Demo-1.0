using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterPanel : MonoBehaviour
{
    public float a = 0;
    public Material gussian;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        a += Time.deltaTime * 5;
        Show();
    }

    public void Show()
    {
        gussian.SetFloat("_Size", a);
    }
}
