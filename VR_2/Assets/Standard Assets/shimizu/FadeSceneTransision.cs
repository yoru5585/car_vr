using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public static class FadeSceneTransition
{

    public static IEnumerator StartFadeOut(string SceneName, Image fadeImage, float fadeSpeed = 0.01f, float waitTime = 0.01f)
    {
        float red, green, blue, alfa;
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;
        //Debug.Log("alfa:" + alfa);

        while (alfa < 1)
        {
            alfa += fadeSpeed;
            //Debug.Log("alfa:" + alfa);
            fadeImage.color = new Color(red, green, blue, alfa);
            yield return new WaitForSeconds(waitTime);
        }
        SceneManager.LoadScene(SceneName);
        //Debug.Log("fade");
    }
}

