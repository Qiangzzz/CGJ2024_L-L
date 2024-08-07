using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridAnimEvent : MonoBehaviour
{
    public Sprite level0TargetSprite,level1TargetSprite,level2TargetSprite,level3TargetSprite,level4TargetSprite;
    private TileMapGameObjectController tileMapController;
    private void Awake()
    {
        level0TargetSprite = GetComponentInChildren<SpriteRenderer>().sprite;
        tileMapController = FindObjectOfType<TileMapGameObjectController>();
    }

    public void AnimEvent_FlipToSprite1()
    {
        Sprite _targetSprite = null;
        switch (TileMapGameObjectController.CurLevel)
        {
            case 5:
                //todo //��Ϸ����
                _targetSprite = level0TargetSprite;
                TileMapGameObjectController.CurLevel = 1;
                return;
                break;
            case 1:
                _targetSprite = level1TargetSprite;
                break;
            case 2:
                _targetSprite = level2TargetSprite;
                break;
            case 3:
                _targetSprite = level3TargetSprite;
                break;
            case 4:
                _targetSprite = level4TargetSprite;
                break;
            default:
                return;
        }
        GetComponentInChildren<SpriteRenderer>().sprite = _targetSprite;

        //ɾ���ɵ�С���壬�½��µ�С����
        tileMapController.ReGenerateSmallGameObject(transform);
        
    }
     
}
