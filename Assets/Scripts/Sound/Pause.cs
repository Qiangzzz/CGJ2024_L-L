using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public void pause(bool p){
        SnakeHead snakeHead = GameObject.Find("SnakeHead").GetComponent<SnakeHead>();
        snakeHead.setSystemPause(p);
    }
}
