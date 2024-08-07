using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
public List<Image> images;
bool play = false;
public float sTime = 1f;
bool start = false;
public float eTime = 1f;
bool end = false;
public float lastTime = 5.5f;
private float speed1;
private float speed2;
public float delay;
private void Start()
{
    speed1 = 1 / sTime;
    speed2 = 1 / eTime;
}
void Update()
{
    if (sTime < 0)
    {
        if (!start)
        {
            if (!play)
            {
                
                play = true;
                start = true;
            }

        }
        if (start)
        {
            
            if (lastTime <= 0)
            {
                CancelInvoke();
                if (eTime <= 0)
                {
                    end = true;
                }
                else
                {
                    eTime -= Time.deltaTime;
                    images[0].color = new Color(images[0].color.r, images[0].color.g, images[0].color.b, images[0].color.a - speed2 * Time.deltaTime);
                    images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, images[1].color.a - speed2 * Time.deltaTime);
                }
            }
            else
            {
                lastTime -= Time.deltaTime;
            }
        }
    }
    else
    {
        sTime -= Time.deltaTime;
        images[0].color = new Color(images[0].color.r, images[0].color.g, images[0].color.b, images[0].color.a + speed1 * Time.deltaTime);
        images[1].color = new Color(images[1].color.r, images[1].color.g, images[1].color.b, images[1].color.a + speed1 * Time.deltaTime);
    }
    if (end)
    {
        if(delay <= 0)
        {
            //SoundManager.Instance.MusiclPlayStr("1");
            SceneManager.LoadScene("Start Scene");
        }
        else
        {
            delay -= Time.deltaTime;
        }
    }
}

}
