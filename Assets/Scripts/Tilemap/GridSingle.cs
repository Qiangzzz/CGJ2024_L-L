using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSingle : MonoBehaviour
{
    // public GridType gridType;
    // public enum GridType
    // {
    //     grid1,
    //     grid2,
    //     grid3
    // }
    public Directions direction;
    public GameObject food;
    public bool catOn=false;
    public int passNumber = 1;
    public void Init(Sprite _sprite)
    {
        // gridType = _gridType;
        transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = _sprite;
        food = null;
    }

    public void SetTrail(Directions _direct)
    {
        direction = _direct;
    }
    public void addPassNumber()
    {
        if(passNumber == 2){
            passNumber = 1;
            SetTrail(Directions.None);
            if(this.gameObject.transform.GetChild(0).childCount >= 2){
            this.gameObject.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            Destroy(this.gameObject.transform.GetChild(0).GetChild(1).gameObject);
            }
            if( this.gameObject.transform.GetChild(0).childCount >= 1 && this.gameObject.transform.GetChild(0).childCount < 2){
            this.gameObject.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            Destroy(this.gameObject.transform.GetChild(0).GetChild(0).gameObject);
            }
        }
        if(passNumber < 2){passNumber++;}

        Debug.Log("次数" + passNumber);
        Debug.Log("子对象" + this.gameObject.transform.GetChild(0).GetChild(0).gameObject.name);
    }
}
