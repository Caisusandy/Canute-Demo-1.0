using Canute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Credit : MonoBehaviour
{
    public int count = 0;
    public float time = 0;
    public ScrollRect page;
    public Image blocker;
    public Canute.Module.Motion Motion;
    private Vector3 mouseInput;

    public void Awake()
    {
        StartCoroutine("AwakeFadeOut");
        StartCoroutine("Up");
    }
    public void Start()
    {
    }

    public void OnMouseDown()
    {
        Motion.enabled = false;
        mouseInput = Input.mousePosition;
    }

    public void OnMouseUp()
    {
        Motion.enabled = true;

        if ((mouseInput - Input.mousePosition).magnitude < 0.1)
            count++;
    }


    // Update is called once per frame
    void Update()
    {
        if (count >= 3)
        {
            blocker.gameObject.SetActive(true);
            time += Time.deltaTime;
            var color = blocker.color;
            color.a = time / 3;
            blocker.color = color;
        }
        if (time > 3)
            SceneControl.GotoScene(MainScene.gameStart);
    }

    IEnumerator AwakeFadeOut()
    {
        blocker.gameObject.SetActive(true);
        var color = blocker.color;
        var t = 0f;

        while (t < 0.5)
        {
            t += Time.deltaTime;
            color = blocker.color;
            color.a = 1;
            blocker.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        t = 0;
        while (t < 3)
        {
            t += Time.deltaTime;
            color = blocker.color;
            color.a = 1 - t / 3;
            blocker.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        color = blocker.color;
        color.a = 0;
        blocker.color = color;
        blocker.gameObject.SetActive(false);
        yield return null;
    }

    IEnumerator Up()
    {
        var t = 0f;

        while (t < 0.5)
        {
            t += Time.deltaTime;
            page.content.position = new Vector3(0, -1000, 0);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        yield return null;
    }

    IEnumerator AwakeFadeIn()
    {
        blocker.gameObject.SetActive(true);
        var color = blocker.color;
        var t = 0f;

        while (t < 3)
        {
            t += Time.deltaTime;
            color = blocker.color;
            color.a = t / 3;
            blocker.color = color;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        color = blocker.color;
        color.a = 1;
        blocker.color = color;
        blocker.gameObject.SetActive(false);
        yield return null;
    }
}
