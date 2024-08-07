using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class TileMapGameObjectController : MonoBehaviour
{
    [Header("��Ƭ")]
    public Tilemap tilemap; // ��Ҫ�ֶ���Tilemap������ק�����ֶ�
    public Sprite[] TileSprites; // ��������Tile��GameObjectԤ�Ƽ�����
    public GameObject TilePrefab;
    public Dictionary<Vector2, GameObject> TileMapData = new Dictionary<Vector2, GameObject>();
    public HashSet<Vector2Int> occupiedTiles = new HashSet<Vector2Int>();


    private List<GameObject> generatedObjects = new List<GameObject>(); // �洢���ɵ�GameObject

    public Transform gridParentTransf; //
    [Header("С����")]
    public GameObject[] smallObjectsPrefabsLevel1;
    public GameObject[] smallObjectLevel2;
    public GameObject[] smallObjectLevel3; // ��������С�����Ԥ�Ƽ�����
    public GameObject[] smallObjectLevel4;
    public GameObject[] smallObjectLevel5;

    public float smallObjectProbability = 0.2f; // С�������ɵĸ���

    public Vector2Int FilpStartPos;
    [FoldoutGroup("�������")]
    public float noiseScale1 = 0.1f; // ����1������
    [FoldoutGroup("�������")]
    public float noiseScale2 = 0.05f; // ����2������
    [FoldoutGroup("�������")]
    public float noiseWeight1 = 0.5f; // ����1��Ȩ��
    [FoldoutGroup("�������")]
    public float noiseWeight2 = 0.5f; // ����2��Ȩ��

    private Vector2 randomOffset1;
    private Vector2 randomOffset2;


    public static int CurLevel = 1;
    public static bool isLoadLevel = false;

    [Header("ʳ��")]
    public GameObject[] foodPrefabs; // ��������ʳ���Ԥ�Ƽ�����
    public GameObject[] EasterEggFoodPrefabs;
    public List<GameObject> foodObjects = new List<GameObject>(); // �洢���ɵ�ʳ�����
    public float foodGenerationProbability = 0.1f; // ʳ�����ɸ���
    private List<GameObject> currentFoods = new List<GameObject>(); // ��ǰ���ɵ�ʳ���б�
    public float foodGenerationInterval = 10.0f; // ʳ�����ɼ��

    public GameObject snakeCopy;
    ScrollManager scrollManager;
    private bool levelIsEnd = false;
    void Start()
    {
        InstorageTileMapData();
        GenerateFood();
        StartCoroutine(GenerateFoodPeriodically());
        scrollManager= GameObject.Find("ScrollCup").GetComponent<ScrollManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("mainScene");
        }
    }

    IEnumerator GenerateFoodPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(foodGenerationInterval);
            GenerateFood();
        }
    }


    public void LevelReGenerate()
    {
        //清除
        Transform snakeTransf = GameObject.Find("SnakeParent(Clone)").transform;
        snakeTransf.gameObject.SetActive(false);
        Destroy(snakeTransf.gameObject);
        var newSnake = GameObject.Instantiate(snakeCopy);
        newSnake.SetActive(true);
        currentFoods.Clear();
        foodCount = 0;
        levelIsEnd = false;
        //地块归位
        foreach (var _tileMap in TileMapData)
        {
            GridSingle _grid = _tileMap.Value.GetComponentInChildren<GridSingle>();
            _grid.direction = Directions.None;
            _grid.catOn = false;
            _grid.GetComponentInChildren<SpriteRenderer>().color = Color.white;
            _grid.passNumber = 0;
            /*if( _grid.transform.GetChild(0).childCount >= 2){
            _grid.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
            Destroy(_grid.transform.GetChild(0).GetChild(1).gameObject);
            }
            if( _grid.transform.GetChild(0).childCount >= 1 && _grid.transform.GetChild(0).childCount < 2){
            _grid.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            Destroy(_grid.transform.GetChild(0).GetChild(0).gameObject);
            }*/
        }
    }
    void GenerateFood()
    {
        if (currentFoods.Count >= 2) return;

        foreach (Transform _gridTransf in gridParentTransf)
        {
            Vector2Int gridPos = new Vector2Int(
                Mathf.FloorToInt(_gridTransf.position.x),
                Mathf.FloorToInt(_gridTransf.position.y)
            );

            if (occupiedTiles.Contains(gridPos)) continue;
            if (GetTileObject(gridPos).GetComponent<GridSingle>().catOn ) continue;
            
            if (Random.value < foodGenerationProbability)
            {
                float randomNumber = Random.Range(0f,1f);
                GameObject foodPrefab = foodPrefabs[0];
                int foodIndex = 0;
                if(randomNumber < 0.05f){
                    foodIndex = Random.Range(0, EasterEggFoodPrefabs.Length);
                    foodPrefab = EasterEggFoodPrefabs[foodIndex];
                }
                else if(randomNumber >= 0.05f){
                    foodIndex = Random.Range(0, foodPrefabs.Length);
                    foodPrefab = foodPrefabs[foodIndex];
                }
                Vector3 foodLocalPosition = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0);
                GameObject foodObject = Instantiate(foodPrefab, Vector3.zero, Quaternion.identity, _gridTransf);
                foodObject.transform.localPosition = foodLocalPosition;
                foodObject.transform.SetParent(_gridTransf.Find("Sprite"));
                foodObjects.Add(foodObject);
                _gridTransf.GetComponent<GridSingle>().food = foodObject;


                // ����ʳ������
                Food foodComponent = foodObject.GetComponent<Food>();
                if(randomNumber < 0.05f){
                    foodComponent.foodType = (FoodType)foodIndex+6;
                }
                 else if(randomNumber >= 0.05f){
                    foodComponent.foodType = (FoodType)foodIndex;
                }

                // ���ӵ���ռ�ø��Ӽ�����
                occupiedTiles.Add(gridPos);

                // ���ӵ���ǰ���ɵ�ʳ���б�
                currentFoods.Add(foodObject);


                break; // ����һ��ʳ����˳�ѭ��
            }
        }
    }

    private int foodCount = 0;
    /// <summary>
    /// ��Ҫ���߳Զ��ӵ�ʱ�򱻵���
    /// </summary>
    /// <param name="gridPos"></param>
    public void RemoveFood(Vector2Int gridPos)
    {
        foodCount++;
        if (foodCount >= 10 && !levelIsEnd )
        {
            endLevel();
            levelIsEnd = true;
            //FilpAllTile(FilpStartPos);
        }
        
        GridSingle _grid = GetTileObject(gridPos).GetComponent<GridSingle>();
        if (occupiedTiles.Contains(gridPos))
        {
            occupiedTiles.Remove(gridPos);
        }
        GameObject foodObject = _grid.food;
        if (currentFoods.Contains(foodObject))
        {
            currentFoods.Remove(foodObject);
        }
        if ((int)foodObject.GetComponent<Food>().foodType < 6){
            SoundManager.Instance.ExtraEffectPlayStr("20");
        }
        if ((int)foodObject.GetComponent<Food>().foodType >= 6){
            SoundManager.Instance.ExtraEffectPlayStr("21");
        }
        Destroy(foodObject);
        scrollManager.setBallIndex(foodObject.GetComponent<Food>());
        _grid.food = null;
        GenerateFood();
        scrollManager.StartCoroutine("CatBottom"); 
    }




    #region Tile����
    void InstorageTileMapData()
    {
        foreach (Transform _gridTransf in gridParentTransf)
        {
            string[] _indexs = _gridTransf.name.Split("_");
            int _index1 = int.Parse(_indexs[1]);
            int _index2 = int.Parse(_indexs[2]);
            TileMapData[new Vector2(_index1, _index2)] = _gridTransf.gameObject;
        }
    }
    /// <summary>
    /// ���ݶ�ά�����ȡ��Ӧ����
    /// </summary>
    /// <param name="_pos"></param>
    /// <returns></returns>
    public GameObject GetTileObject(Vector2Int _pos)
    {
        if (TileMapData.ContainsKey(_pos))
        {
            return TileMapData[_pos];
        }

        return null;
    }
    #endregion

    #region �����л�
    public void FilpAllTile(Vector2Int _startPos)
    {
        StartCoroutine(FilpAllTileIEnum(_startPos));
    }
    public float flipTimeSplit = 0.5f;

    // ���ϵ��·�ת��ͼ  4-- -6
    //IEnumerator FilpFromUpToBottom()
    //{
    //    Dictionary<int, List<Animator>> animDic = new Dictionary<int, List<Animator>>();
    //    foreach (Transform _gridTransf in gridParentTransf)
    //    {
    //        int _index = int.Parse(_gridTransf.name.Split("_")[1]);
    //        if (!animDic.ContainsKey(_index))
    //        {
    //            animDic[_index] = new List<Animator>();
    //        }
    //        animDic[_index].Add(_gridTransf.GetComponent<Animator>());
    //    }
    //    for (int i = 4; i >= -6; i--)
    //    {
    //        List<Animator> _animList = animDic[i];
    //        foreach (var _anim in _animList)
    //        {
    //            _anim.Play("Grid_Filp1");
    //        }
    //        yield return new WaitForSeconds(flipTimeSplit);
    //    }
    //}


    IEnumerator FilpAllTileIEnum(Vector2Int _startPos)
    {
        isLoadLevel = true;

        List<Vector2Int> flipedSprite = new List<Vector2Int>();
        //һ��
        List<Vector2Int> _stack = new List<Vector2Int> { _startPos };
        while (_stack.Count > 0)
        {
            List<Vector2Int> _tempStack = new List<Vector2Int>();
            foreach (var _vector2 in _stack)
            {
                if (TileMapData.ContainsKey(_vector2))
                {
                    flipedSprite.Add(_vector2);
                    //�и����ݲ��һ�δ��ת��
                    TileMapData[_vector2].GetComponent<Animator>().Play("Grid_Filp1");

                    var _aroundGrids = GridManager.getAroundGrids(_vector2);
                    foreach (var _aroundGrid in _aroundGrids)
                    {
                        if (TileMapData.ContainsKey(_aroundGrid) && !flipedSprite.Contains(_aroundGrid))
                        {
                            _tempStack.Add(_aroundGrid);
                        }
                    }
                }
            }
            _stack = _tempStack;


            yield return new WaitForSeconds(flipTimeSplit);
        }

        CurLevel++;
        isLoadLevel = false;
        LevelReGenerate();
        //todo: �ؿ����ؽ�����������ҽ���
    }
    #endregion

    #region �ؿ�����
    [Button("��ʼ��Grid")]
    public void InitGenerateGridGO()
    {
        ClearGeneratedObjects();
        GenerateOffsetsAndNoise();
        GenerateGameObjectsFromTiles();

        // �����ռ�ø��Ӽ���
        occupiedTiles.Clear();
        GenerateFood();
    }

    private string spriteName = "Sprite";
    [Button("��ȡ��ͼ����")]
    public void LoadGridData()
    {
        Transform level1Transf = GameObject.Find("Level1").transform;
        Transform level2Transf = GameObject.Find("Level2").transform;
        Transform level3Transf = GameObject.Find("Level3").transform;
        Transform level4Transf = GameObject.Find("Level4").transform;
        Transform level5Transf = GameObject.Find("Level5").transform;

        foreach (Transform _transf in gridParentTransf)
        {
            string _name = _transf.name;
            _transf.Find("Sprite").GetComponent<SpriteRenderer>().sprite = level1Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level1TargetSprite = level2Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level2TargetSprite = level3Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level3TargetSprite = level4Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;
            _transf.GetComponentInChildren<GridAnimEvent>().level4TargetSprite = level5Transf.Find(_name).GetComponent<SpriteRenderer>().sprite;

            Transform _transfChild = _transf.Find(spriteName);
            for (int i = _transfChild.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(_transfChild.GetChild(i).gameObject);
            }
            // var _smallGo = _transf.Find(smallObjectName);
            // if (_smallGo != null)
            // {
            //     DestroyImmediate(_smallGo.gameObject);
            // }

            GameObject[] _smallGoPrefabs = null;
            if (CurLevel == 1)
            {
                _smallGoPrefabs = smallObjectsPrefabsLevel1;
            }
            else if (CurLevel == 2)
            {
                _smallGoPrefabs = smallObjectLevel2;
            }
            else if (CurLevel == 3)
            {
                _smallGoPrefabs = smallObjectLevel3;
            }
            else if (CurLevel == 4)
            {
                _smallGoPrefabs = smallObjectLevel4;
            }
            else if (CurLevel == 5)
            {
                _smallGoPrefabs = smallObjectLevel5;
            }
            //����С����
            if (Random.value < smallObjectProbability)
            {
                int smallObjectIndex = Random.Range(0, _smallGoPrefabs.Length);
                GameObject smallObjectPrefab = _smallGoPrefabs[smallObjectIndex];

                Vector3 ObjectLocalPosition = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
                GameObject smallObject = Instantiate(smallObjectPrefab, Vector3.zero, Quaternion.identity, _transf);
                smallObject.transform.localPosition = ObjectLocalPosition;
                smallObject.transform.SetParent(_transf.Find("Sprite"));
                //smallObject.name = "SmallObject_" + localPlace.x + "_" + localPlace.y;
                smallObject.name = smallObjectName;
            }
        }
    }

    void GenerateOffsetsAndNoise()
    {
        // ��ʼ�����ƫ��
        randomOffset1 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));
        randomOffset2 = new Vector2(Random.Range(0f, 1000f), Random.Range(0f, 1000f));

        // ��̬�����������������ӱ仯
        noiseScale1 = Random.Range(0.01f, 0.2f);
        noiseScale2 = Random.Range(0.01f, 0.2f);
        noiseWeight1 = Random.Range(0.3f, 0.7f);
        noiseWeight2 = 1 - noiseWeight1;
    }

    void GenerateGameObjectsFromTiles()
    {
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    Vector3Int localPlace = new Vector3Int(x + bounds.xMin, y + bounds.yMin, 0);
                    Vector3 place = tilemap.CellToWorld(localPlace);

                    // ʹ������Perlin�������ɴ��и���仯�ĵ��Σ����������ƫ��
                    float noiseValue1 = Mathf.PerlinNoise((x + randomOffset1.x) * noiseScale1, (y + randomOffset1.y) * noiseScale1);
                    float noiseValue2 = Mathf.PerlinNoise((x + randomOffset2.x) * noiseScale2, (y + randomOffset2.y) * noiseScale2);
                    float combinedNoiseValue = noiseValue1 * noiseWeight1 + noiseValue2 * noiseWeight2;

                    int prefabIndex = Mathf.FloorToInt(combinedNoiseValue * TileSprites.Length);
                    if (prefabIndex >= TileSprites.Length)
                    {
                        prefabIndex = TileSprites.Length - 1;
                    }

                    GameObject tileGO = Instantiate(TilePrefab, place, Quaternion.identity, gridParentTransf);
                    tileGO.GetComponent<GridSingle>().Init(TileSprites[prefabIndex]);
                    tileGO.name = "Tile_" + localPlace.x + "_" + localPlace.y;

                    generatedObjects.Add(tileGO); // �����ɵ�GameObject���ӵ��б���
                }
            }
        }
    }

    string smallObjectName = "SmallObject";
    public void ReGenerateSmallGameObject(Transform _transf)
    {
        //����ɵ�С���������
        Transform _transfChild = _transf.Find(spriteName);
        for (int i = _transfChild.childCount - 1; i >= 0; i--)
        {
            Destroy(_transfChild.GetChild(i).gameObject);
        }


        //�����µ�С����
        if (Random.value < smallObjectProbability)
        {
            GameObject[] _smallGoPrefabs = null;
            if (CurLevel == 1)
            {
                _smallGoPrefabs = smallObjectLevel2;
            }
            else if (CurLevel == 2)
            {
                _smallGoPrefabs = smallObjectLevel3;
            }
            else if (CurLevel == 3)
            {
                _smallGoPrefabs = smallObjectLevel4;
            }
            else if (CurLevel == 4)
            {
                _smallGoPrefabs = smallObjectLevel5;
            }
            else
            {
                //Debug.Log("");
                return;
            }
            //else if (CurLevel == 3)
            //{
            //    _smallGoPrefabs = smallObjectLevel3;
            //}



            int smallObjectIndex = Random.Range(0, _smallGoPrefabs.Length);
            GameObject smallObjectPrefab = _smallGoPrefabs[smallObjectIndex];
            Vector3 smallObjectLocalPosition = new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.25f, 0.25f), 0);
            GameObject smallObject = Instantiate(smallObjectPrefab, Vector3.zero, Quaternion.identity, _transf);
            smallObject.transform.localPosition = smallObjectLocalPosition;
            smallObject.transform.SetParent(_transf.Find("Sprite"));
            smallObject.transform.localRotation = Quaternion.identity;
            //smallObject.name = "SmallObject_" + localPlace.x + "_" + localPlace.y;
        }
    }

    void ClearGeneratedObjects()
    {
        for (int i = gridParentTransf.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParentTransf.GetChild(i).gameObject);
        }

        generatedObjects.Clear(); // ����б�
    }
    void endLevel(){
        var firstGrid = TileMapData[FilpStartPos];
        GridSingle grid = firstGrid.GetComponent<GridSingle>();
        SoundManager.Instance.ees1PlayStr("17");
        grid.GetComponent<Animator>().Play("Grid_Flip3");
    }
    #endregion
}
