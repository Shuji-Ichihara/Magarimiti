using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapManager : SingletonMonoBehaviour<MapManager>
{
    #region Refarences
    // 配置するマップチップ
    [SerializeField]
    private MapChip _mapChip = null;
    // プレイヤーオブジェクト
    [SerializeField]
    private GameObject _player = null;
    // 鍵オブジェクト
    [SerializeField]
    private GameObject _key = null;
    // 錠前オブジェくト
    [SerializeField]
    private GameObject _lock = null;
    [SerializeField]
    private Button _button;
    //
    [SerializeField]
    private GameObject _house = null;
    [SerializeField]
    private UnityEngine.UI.Button _resetButton = null;
    #endregion
    #region Sprites
    // マップチップに設定するスプライトの配列
    public List<Sprite> MapChipSprites => _mapChipSprites;
    [SerializeField]
    private List<Sprite> _mapChipSprites = new List<Sprite>();
    // マップチップの属性によって設定するスプライト
    public Sprite StartMapChipSprite => _startMapChipSprite;
    [SerializeField]
    private Sprite _startMapChipSprite = null;
    public Sprite GoalMapChipSprite => _goalMapChipSprite;
    [SerializeField]
    private Sprite _goalMapChipSprite = null;
    public Sprite KeyMapChipSprite => _keyMapChipSprite;
    [SerializeField]
    private Sprite _keyMapChipSprite = null;
    public Sprite LockMapChipSprite => _lockMapChipSprite;
    [SerializeField]
    private Sprite _lockMapChipSprite = null;
    #endregion
    #region Map
    // マップの縦横比
    public Vector2Int MapChipWidthAndHeight => _mapWidthAndHeight;
    [SerializeField]
    private Vector2Int _mapWidthAndHeight = new Vector2Int(4, 4);
    // マップチップ配列
    // 配列のイメージは、[横のマップチップ, 縦のマップチップ]です。
    public MapChip[,] Map => _map;
    private MapChip[,] _map = { };
    #endregion
    #region Others
    // スワイプの始点と終点
    Vector3 _swipeStartPosition = Vector3.zero, _swipeEndPosition = Vector3.zero;
    // 動かすマップチップと移動先のマップチップ
    MapChip _targetMapChip = null, _destinationMapChip = null;
    // 空白のマップチップ
    private MapChip _noneMapChip = null;
    // 選択中のマテリアル
    [SerializeField]
    private Material _activeMaterial = null;
    // 通常時のマテリアル
    [SerializeField]
    private Material _defaultMaterial = null;
    // ゴールのテスト用フラグ
    private bool _isReachedGoal = false;
    private bool _isResetButton = false;
    #endregion

    new private void Awake()
    {
        // null チェック
        if (_mapChip == null)
        {
            GameObject obj = Resources.Load("Prefabs/MapChip") as GameObject;
            _mapChip = obj.GetComponent<MapChip>();
        }
        if (_player == null)
        {
            GameObject obj = Resources.Load("Prefabs/Player") as GameObject;
            _player = obj;
        }
        if (_key == null)
        {
            GameObject obj = Resources.Load("Prefabs/Key") as GameObject;
            _key = obj;
        }
        if (_lock == null)
        {
            GameObject obj = Resources.Load("Prefabs/Lock") as GameObject;
            _lock = obj;
        }
        if(_house == null)
        {
            GameObject obj = Resources.Load("Prefabs/House") as GameObject;
            _house = obj;
        }
        if (_button == null)
        {
            GameObject obj = GameObject.Find("Button");
            _button = obj.GetComponent<Button>();
        }
        if (_resetButton == null)
        {
            GameObject obj = GameObject.Find("ResetButton");
            _resetButton = obj.GetComponent<UnityEngine.UI.Button>();
        }
        // マップ配列の大きさを設定
        _map = new MapChip[_mapWidthAndHeight.y, _mapWidthAndHeight.x];
        GenerateMap();
        GenerateObject();
    }

    private void Start()
    {
        SoundManager.instance.BGMPause();
        SoundManager.instance.PlayBGM(SoundManager.E_BGM.InGameBGM);
    }

    private void Update()
    {
        _isResetButton = false;
        MoveMapChip();
        GoalCheck();
        if (_isReachedGoal == true)
        {
            _button.Noumber = 3;
            _button.SceneMoving();
        }
        // 「やり直し」ボタンを押した時の処理
        _resetButton.onClick.AddListener(
            delegate
            {
                if(_isResetButton == false)
                {
                    DeleteMap();
                    if (_key == null)
                    {
                        GameObject obj = Resources.Load("Prefabs/Key") as GameObject;
                        _key = obj;
                    }
                    if (_lock == null)
                    {
                        GameObject obj = Resources.Load("Prefabs/Lock") as GameObject;
                        _lock = obj;
                    }
                    // マップ再生成
                    GenerateMap();
                    GenerateObject();
                    // プレイヤーのステータスをリセット
                    var player = _player.GetComponent<PlayerManager>();
                    player.enabled = true;
                    player.DisableKey();
                    player.SetKeyImageColor(new Color(1.0f, 1.0f, 1.0f, 0.3f));
                    _player.GetComponent<CapsuleCollider2D>().enabled = true;
                    _key.GetComponent<CapsuleCollider2D>().enabled = true;
                    _lock.GetComponent<BoxCollider2D>().enabled = true;
                    _isResetButton = true;
                }
            });
    }

    /// <summary>
    /// マップ生成
    /// </summary>
    private void GenerateMap()
    {
        int half = 2;
        // 欠けさせるマップチップの座標
        Vector2Int hiddenMapChip = new Vector2Int(_mapWidthAndHeight.y - 1, 0);
        // _map[0,0] の座標
        Vector3 mapChipPosition = new Vector3(
            -(_mapChip.transform.localScale.x * _mapWidthAndHeight.x / half) + _mapChip.transform.localScale.x / half
           , (_mapChip.transform.localScale.y * _mapWidthAndHeight.y / half) - _mapChip.transform.localScale.y / half);
        for (int heightMapChipCount = 0; heightMapChipCount < _mapWidthAndHeight.y; heightMapChipCount++)
        {
            // 一列配置する度に、次に配置するマップチップを 1 個分ずつ下にずらす
            if (heightMapChipCount != 0)
            {
                var dummyObj = _map[heightMapChipCount - 1, 0];
                mapChipPosition = new Vector3(
                    dummyObj.transform.position.x
                  , dummyObj.transform.position.y - _mapChip.transform.localScale.y);
            }
            for (int widthMapChipCount = 0; widthMapChipCount < _mapWidthAndHeight.x; widthMapChipCount++)
            {
                // 配置する度にマップチップを 1 個分ずつ右にずらす
                if (widthMapChipCount != 0)
                    mapChipPosition += Vector3.right * _mapChip.transform.localScale.x;
                GameObject mapChipObj = Instantiate(_mapChip.gameObject, mapChipPosition, Quaternion.identity);
                var mapChip = mapChipObj.GetComponent<MapChip>();
                mapChip.transform.SetParent(transform);
                _map[heightMapChipCount, widthMapChipCount] = mapChip;
            }
        }
        // 空白のマップチップを作成
        SpriteRenderer renderer = _map[hiddenMapChip.x, hiddenMapChip.y].GetComponent<SpriteRenderer>();
        renderer.enabled = false;
    }

    /// <summary>
    /// マップ内に存在するオブジェクトを生成
    /// </summary>
    private void GenerateObject()
    {
        Vector2Int keySpawnPosition = new Vector2Int(),
                   lockSpawnPosition = new Vector2Int(), 
                   houseSpawnPosition = new Vector2Int();
        bool isGeneratedKey = false;
        // _map[1,1] の座標に鍵が生成されないようにする
        // ゲームの詰み防止
        while(isGeneratedKey == false)
        {
            // 鍵が生成されるマップチップのインデックス
            keySpawnPosition = new Vector2Int(
                Random.Range(1, _mapWidthAndHeight.x - 1), Random.Range(1, _mapWidthAndHeight.y - 1));
            if (keySpawnPosition.x == 1 && keySpawnPosition.y == 1)
                continue;
            isGeneratedKey = true;
        }
        // 錠前が生成されるマップチップのインデックス
        lockSpawnPosition = new Vector2Int(_mapWidthAndHeight.x - 1, _mapWidthAndHeight.y - 2);
        //
        houseSpawnPosition = new Vector2Int(_mapWidthAndHeight.x - 1, MapChipWidthAndHeight.y - 1);
        // 生成
        _player = Instantiate(_player, _map[0, 0].transform.position, Quaternion.identity);
        _key = Instantiate(_key, _map[keySpawnPosition.y, keySpawnPosition.x].transform.position, Quaternion.identity);
        _lock = Instantiate(_lock, _map[lockSpawnPosition.y, lockSpawnPosition.x].transform.position, Quaternion.identity);
        _house = Instantiate(_house, _map[houseSpawnPosition.y, houseSpawnPosition.x].transform.position, Quaternion.identity);
    }

    /// <summary>
    /// マップチップの移動
    /// </summary>
    private void MoveMapChip()
    {
        int half = 2;
        // マップチップを動かせるフラグ
        bool isMoveMapChip = false;
        if (Input.GetMouseButtonDown(0) == true)
        {
            var dummySwipeStartPosition = Input.mousePosition;
            _swipeStartPosition = Camera.main.ScreenToWorldPoint(dummySwipeStartPosition);
            // 移動させるマップチップの情報を取得
            _targetMapChip = GetMapChipData(_swipeStartPosition.x, _swipeStartPosition.y);
            // _targetMapChip が null ならばマテリアルを設定しない
            if (_targetMapChip == null)
                return;
            SoundManager.instance.PlaySE(SoundManager.E_SE.ChooseMapChip);
            _targetMapChip.SetMapChipMaterial(_activeMaterial);
        }
        if (Input.GetMouseButtonUp(0) == true)
        {
            var dummySwipeEndPosition = Input.mousePosition;
            _swipeEndPosition = Camera.main.ScreenToWorldPoint(dummySwipeEndPosition);
            // 絶対値に変換
            float xDirection = Mathf.Abs(GetSwipeDirection(_swipeStartPosition.x, _swipeEndPosition.x));
            float yDirection = Mathf.Abs(GetSwipeDirection(_swipeStartPosition.y, _swipeEndPosition.y));
            // スワイプ距離があまりにも短い場合は移動しない
            if (xDirection <= _mapChip.transform.localScale.x / half
                && yDirection <= _mapChip.transform.localScale.y / half)
            {
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                return;
            }
            // プレイヤーが乗っているマップチップは移動しない
            if (_player.transform.position.x >= _targetMapChip.transform.position.x - _player.transform.localScale.x / half
                && _player.transform.position.x <= _targetMapChip.transform.position.x + _player.transform.localScale.x / half)
                if (_player.transform.position.y >= _targetMapChip.transform.position.y - _player.transform.localScale.y / half
                    && _player.transform.position.y <= _targetMapChip.transform.position.y + _player.transform.localScale.y / half)
                {
                    SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                    _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                    return;
                }
            // 移動先のマップチップの情報を取得
            _destinationMapChip = GetMapChipData(_swipeEndPosition.x, _swipeEndPosition.y);
            if (_destinationMapChip == null)
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
            isMoveMapChip = true;
        }
        if (isMoveMapChip == true)
        {
            // いずれかのマップチップの情報が null ならば移動しない
            if (_targetMapChip == null || _destinationMapChip == null)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                return;
            }
            // 移動前と移動後のマップチップの情報が同じならば移動しない
            if (_targetMapChip == _destinationMapChip)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                return;
            }
            // _targetMapChip と NoneMapChip のインデックスを取得
            var indexNoneMapChip = _map.GetIndex(_noneMapChip);
            var indexTargetMapChip = _map.GetIndex(_targetMapChip);
            // 斜めの移動を行わない
            if ((indexTargetMapChip.x == indexNoneMapChip.x || indexTargetMapChip.y == indexNoneMapChip.y) == false)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                return;
            }
            // 空白のマップチップに隣接していない場合は移動しない
            if ((indexTargetMapChip.x == indexNoneMapChip.x - 1 || indexTargetMapChip.x == indexNoneMapChip.x + 1
                || indexTargetMapChip.y == indexNoneMapChip.y - 1 || indexTargetMapChip.y == indexNoneMapChip.y + 1) == false)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                return;
            }
            // スタート、ゴール、鍵、錠前があるマップチップ、空白のマップチップは移動しない
            if (_targetMapChip.MapChipAttribute != MapChipAttribute.Use)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                return;
            }
            // スライドパズルである為、空白以外には移動しない
            if (_destinationMapChip.MapChipAttribute != MapChipAttribute.None)
            {
                SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                _targetMapChip.SetMapChipMaterial(_defaultMaterial);
                return;
            }
            // マップチップを移動させる
            ChangeMapChip(ref _targetMapChip, ref _destinationMapChip);
            SoundManager.instance.PlaySE(SoundManager.E_SE.ReleaseMapChip);
            _targetMapChip.SetMapChipMaterial(_defaultMaterial);
        }
    }

    /// <summary>
    /// マップチップを入れ替える
    /// </summary>
    /// <param name="targetMapChip">入れ替えたいマップチップ</param>
    /// <param name="destinationMapChip">入れ替える先のマップチップ</param>
    private void ChangeMapChip(ref MapChip targetMapChip, ref MapChip destinationMapChip)
    {
        Vector3 targetPosition = targetMapChip.transform.position, destinationPosition = destinationMapChip.transform.position;
        MapChip dummyTargetMapChip = targetMapChip, dummyDestinationMapChip = destinationMapChip;
        for (int heightMapChipCount = 0; heightMapChipCount < _mapWidthAndHeight.y; heightMapChipCount++)
        {
            for (int widthMapChipCount = 0; widthMapChipCount < _mapWidthAndHeight.x; widthMapChipCount++)
            {
                // 配列の要素を上書き
                // これをしないとインデックスを取得する時におかしな値になる
                if (_map[heightMapChipCount, widthMapChipCount] == targetMapChip)
                    _map.SetValue(dummyDestinationMapChip, heightMapChipCount, widthMapChipCount);
                else if (_map[heightMapChipCount, widthMapChipCount] == destinationMapChip)
                    _map.SetValue(dummyTargetMapChip, heightMapChipCount, widthMapChipCount);
            }
        }
        targetMapChip.transform.position = destinationPosition;
        destinationMapChip.transform.position = targetPosition;
    }

    /// <summary>
    /// プレイヤーがゴールしたかを判定
    /// </summary>
    public void GoalCheck()
    {
        int half = 2;
        // プレイヤーの座標、ゴールのマップチップの座標
        Vector3 playerPosition = _player.transform.position,
                mapChipPosition = _map[MapChipWidthAndHeight.y - 1, _mapWidthAndHeight.x - 1].transform.position;
        // プレイヤーオブジェクトのスケール
        float playerXScale = _player.transform.localScale.x, playerYScale = _player.transform.localScale.y;
        // プレイヤーの座標がゴールのマップチップの中心と重なったらゴールする
        if (playerPosition.x <= mapChipPosition.x + playerXScale / half && playerPosition.x >= mapChipPosition.x - playerXScale / half)
            if (playerPosition.y <= mapChipPosition.y + playerYScale / half && playerPosition.y >= mapChipPosition.y - playerYScale / half)
                _isReachedGoal = true;
    }

    /// <summary>
    /// マップ削除
    /// </summary>
    private void DeleteMap()
    {
        Destroy(_player);
        Destroy(_key);
        Destroy(_lock);
        for (int heightMapChipCount = 0; heightMapChipCount < _mapWidthAndHeight.y; heightMapChipCount++)
        {
            for (int widthMapChipCount = 0; widthMapChipCount < _mapWidthAndHeight.x; widthMapChipCount++)
            {
                Destroy(_map[heightMapChipCount, widthMapChipCount].gameObject);
            }
        }
    }

    #region Setter
    /// <summary>
    /// None 属性のマップチップの情報を格納
    /// </summary>
    /// <param name="mapChip"></param>
    public void SetNoneMapChip(MapChip mapChip)
    {
        _noneMapChip = mapChip;
    }
    #endregion
    #region Getter
    /// <summary>
    /// 魏のデータを取得
    /// </summary>
    /// <returns>鍵のゲームオブジェクト</returns>
    public GameObject GetKeyData()
    {
        return _key;
    }

    /// <summary>
    /// 錠前のデータを取得
    /// </summary>
    /// <returns></returns>錠前のゲームオブジェクト
    public GameObject GetLockData()
    {
        return _lock;
    }

    /// <summary>
    /// スワイプした距離の長さを取得
    /// </summary>
    /// <param name="startValue">スワイプの始点</param>
    /// <param name="endValue">スワイプの終点</param>
    /// <returns>始点と終点の距離</returns>
    private float GetSwipeDirection(in float startValue, in float endValue)
    {
        return endValue - startValue;
    }

    /// <summary>
    /// 配列の中のマップチップの情報をワールド座標から取得
    /// </summary>
    /// <param name="xPosition">取得したいマップチップの X 座標</param>
    /// <param name="yPosition">取得したいマップチップの Y 座標</param>
    /// <returns>マップチップの情報</returns>
    public MapChip GetMapChipData(float xPosition, float yPosition)
    {
        int half = 2;
        MapChip mapChip = null;
        foreach (var dummyMapChip in _map)
        {
            float xScale = dummyMapChip.transform.localScale.x;
            float yScale = dummyMapChip.transform.localScale.y;
            Vector3 dummyMapChipPosition = dummyMapChip.transform.position;
            // 引数の値を比較し、条件に合致するマップチップを特定、代入する
            if (xPosition >= dummyMapChipPosition.x - xScale / half && xPosition <= dummyMapChipPosition.x + xScale / half)
            {
                if (yPosition >= dummyMapChipPosition.y - yScale / half && yPosition <= dummyMapChipPosition.y + yScale / half)
                {
                    mapChip = dummyMapChip;
                    break;
                }
            }
        }
        if (mapChip == null)
        {
            Debug.LogError($"{new Vector2(xPosition, yPosition)}にあるマップチップの情報を取得できませんでした。");
            return null;
        }
#if UNITY_EDITOR
        //Debug.Log($"今回取得したマップチップは、MapChip{_map.GetIndex(mapChip)} です。");
        //Debug.Log($"今回取得したマップチップは、MapChip{mapChip.MapChipAttribute} です。");
#endif
        return mapChip;
    }
    #endregion
}
