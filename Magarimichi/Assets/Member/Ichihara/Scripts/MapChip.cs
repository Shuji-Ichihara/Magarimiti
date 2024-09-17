using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum MapChipAttribute
{
    None  = 0,
    Use   = 1 << 0,
    Start = 1 << 1,
    Goal  = 1 << 2,
    Key   = 1 << 3,
    Lock  = 1 << 4,
}

[RequireComponent(typeof(SpriteRenderer))]
public class MapChip : MonoBehaviour
{
    // このスクリプトが設定されているプレハブのレンダラー
    private SpriteRenderer _renderer = null;

    // マップチップの属性
    public MapChipAttribute MapChipAttribute { get => _mapChipAttribute; }
    private MapChipAttribute _mapChipAttribute = MapChipAttribute.Use;

    // プレイヤーが現在存在するマップチップから移動可能な方向
    public Dictionary<string, bool> CanMovePlayer => _canMovePlayer;
    private Dictionary<string, bool> _canMovePlayer = new Dictionary<string, bool>()
    {
        {Common.MoveUp,    false },
        {Common.MoveDown,  false },
        {Common.MoveLeft,  false },
        {Common.MoveRight, false },
    };

    // Start is called before the first frame update
    void Start()
    {
        // リクエストしている為、null チェックは不要
        _renderer = GetComponent<SpriteRenderer>();
        // 背景である為、一番下に配置されるようにする
        _renderer.sortingOrder = -99;
        SetUpMapChipAtrribute();
        // 「Use」属性マップチップのみ、ランダムにスプライトを設定
        var random = Random.Range(0, MapManager.Instance.MapChipSprites.Count);
        if (_mapChipAttribute == MapChipAttribute.Use)
            SetMapChipSprite(MapManager.Instance.MapChipSprites[random]);
        // 空白のマップチップである場合、MapManager に情報を格納
        if (_mapChipAttribute == MapChipAttribute.None)
            MapManager.Instance.SetNoneMapChip(this);
        SetUpMoveDirection();
    }

    #region SetUp
    /// <summary>
    /// マップチップの属性を設定
    /// </summary>
    private void SetUpMapChipAtrribute()
    {
        var map = MapManager.Instance.Map;
        // [0, 0] はスタート固定
        if (this == map[0, 0])
        {
            _renderer.sprite = MapManager.Instance.StartMapChipSprite;
            _mapChipAttribute = _mapChipAttribute | MapChipAttribute.Start;
            return;

        }
        // 右下はゴール固定
        else if (this == map[MapManager.Instance.MapChipWidthAndHeight.y - 1, MapManager.Instance.MapChipWidthAndHeight.x - 1])
        {
            _renderer.sprite = MapManager.Instance.GoalMapChipSprite;
            _mapChipAttribute = _mapChipAttribute | MapChipAttribute.Goal;
            return;
        }
        // ゲーム開始時、左下は欠けている
        else if (this == map[MapManager.Instance.MapChipWidthAndHeight.y - 1, 0])
        {
            _mapChipAttribute = _mapChipAttribute & ~MapChipAttribute.Use;
            return;
        }
        // 鍵のあるマップチップは移動できない
        else if (transform.position == MapManager.Instance.GetKeyData().transform.position)
        {
            _renderer.sprite = MapManager.Instance.KeyMapChipSprite;
            _mapChipAttribute = _mapChipAttribute | MapChipAttribute.Key;
            return;
        }
        // 錠前はゴールの一つ上に存在
        else if (this == map[MapManager.Instance.MapChipWidthAndHeight.y - 2, MapManager.Instance.MapChipWidthAndHeight.x - 1])
        {
            _renderer.sprite = MapManager.Instance.LockMapChipSprite;
            _mapChipAttribute = _mapChipAttribute | MapChipAttribute.Lock;
            return;
        }
    }

    /// <summary>
    /// プレイヤーが移動できる方向を設定する
    /// </summary>
    private void SetUpMoveDirection()
    {
        Sprite mapChipSprite = _renderer.sprite;
        string mapChipSpriteName = mapChipSprite.name;
        // プレイヤーがこのマップチップからどこに移動できるかを設定
        // 隣接しているマップチップと道が接続されているかを比較する為に使用
        if (mapChipSpriteName.Contains("_Up") == true)
            _canMovePlayer[Common.MoveUp] = true;
        if (mapChipSpriteName.Contains("_Down") == true)
            _canMovePlayer[Common.MoveDown] = true;
        if (mapChipSpriteName.Contains("_Left") == true)
            _canMovePlayer[Common.MoveLeft] = true;
        if (mapChipSpriteName.Contains("_Right") == true)
            _canMovePlayer[Common.MoveRight] = true;

    }
    #endregion

    /// <summary>
    /// 鍵の属性を除去する (プレイヤー側から呼び出す)
    /// </summary>
    public void RemoveKeyAttribute()
    {
        if (_mapChipAttribute == (MapChipAttribute.Key | MapChipAttribute.Use))
            _mapChipAttribute = _mapChipAttribute & ~MapChipAttribute.Key;
    }

    #region Setter
    /// <summary>
    /// スプライトを設定
    /// </summary>
    /// <param name="sprite">設定するスプライト画像</param>
    private void SetMapChipSprite(Sprite sprite)
    {
        _renderer.sprite = sprite;
    }

    /// <summary>
    /// マテリアルを設定
    /// </summary>
    /// <param name="material">設定するマテリアル</param>
    public void SetMapChipMaterial(Material material)
    {
        _renderer.material = material;
    }
    #endregion
}
