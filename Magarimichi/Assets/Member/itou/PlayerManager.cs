using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private Image _Keyimage;
    [SerializeField] private float _playerMoveMax;
    [SerializeField] private float _movestartMax;
    private SpriteRenderer _mapSprite;
    private readonly KeyCode _movedUpKey = KeyCode.W;
    private readonly KeyCode _movedDownKey = KeyCode.S;
    private readonly KeyCode _movedLeftKey = KeyCode.A;
    private readonly KeyCode _movedRightKey = KeyCode.D;
    private MapChip _mapChip = null;
    private float _playerMove;
    private float _movestart;
    private float colorrock = 1.0f;
    private float MoveSpeed = 2.0f;
    private int direction = 1;
    public bool _UpStart;
    public bool _DownStart;
    public bool _LeftStart;
    public bool _RightStart;
    private bool _GetKey;

    private bool _isMove = false;

    private void Start()
    {
        _Keyimage = GameObject.Find("Key").GetComponent<Image>();
        _Keyimage.color = new Color(colorrock, colorrock, colorrock, 0.3f);
    }

    private void FixedUpdate()
    {
        /*
        if (_UpStart)
        {
            _playerMove++;
            direction = 1;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + MoveSpeed * Time.fixedDeltaTime * direction, this.transform.position.z);
        }
        else if (_DownStart)
        {
            _playerMove++;
            direction = -1;
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + MoveSpeed * Time.fixedDeltaTime * direction, this.transform.position.z);
        }
        else if (_RightStart)
        {
            _playerMove++;
            direction = 1;
            this.transform.position = new Vector3(this.transform.position.x + MoveSpeed * Time.fixedDeltaTime * direction, this.transform.position.y, this.transform.position.z);
        }
        else if (_LeftStart)
        {
            _playerMove++;
            direction = -1;
            this.transform.position = new Vector3(this.transform.position.x + MoveSpeed * Time.fixedDeltaTime * direction, this.transform.position.y, this.transform.position.z);
        }
        */
    }

    void Update()
    {
        MapChip mapChipData = MapManager.Instance.GetMapChipData(transform.position.x, transform.position.y);
        if (mapChipData != null)
            SetMapChipData(mapChipData);
        if (_mapChip == null)
            return;
        var mapData = MapManager.Instance.Map;
        Vector2Int mapChipIndex = mapData.GetIndex(_mapChip);
        /*
        _movestart++;
        if (_playerMoveMax < _playerMove)
        {
            _UpStart = false;
            _DownStart = false;
            _LeftStart = false;
            _RightStart = false; 
            _isMove = false;
            _playerMove = 0;
        }
        */

        if (_isMove == false)
        {
            if (Input.GetKeyDown(_movedRightKey) == true)
            {
                MapChip targetMapChip = mapData[mapChipIndex.x, mapChipIndex.y];
                MapChip destitaionMapChip = mapData[mapChipIndex.x, mapChipIndex.y + 1];
                // プレイヤーが現在いるマップチップと移動先のマップチップを比較する
                if (targetMapChip.CanMovePlayer[Common.MoveRight] == true && destitaionMapChip.CanMovePlayer[Common.MoveLeft] == true)
                {
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.Use | MapChipAttribute.Lock) && _GetKey == false)
                    {
                        SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                        return;
                    }
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.None))
                        return;
                    //_RightStart = true;
                    //_movestart = 0;
                    _isMove = true;
                    StartCoroutine(MovePlayerCoroutine(Vector3.right));
                }
            }
            if (Input.GetKeyDown(_movedDownKey) == true)
            {
                MapChip targetMapChip = mapData[mapChipIndex.x, mapChipIndex.y];
                MapChip destitaionMapChip = mapData[mapChipIndex.x + 1, mapChipIndex.y];
                // プレイヤーが現在いるマップチップと移動先のマップチップを比較する
                if (targetMapChip.CanMovePlayer[Common.MoveDown] == true && destitaionMapChip.CanMovePlayer[Common.MoveUp] == true)
                {
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.Use | MapChipAttribute.Lock) && _GetKey == false)
                    {
                        SoundManager.instance.PlaySE(SoundManager.E_SE.Cancel);
                        return;
                    }
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.None))
                        return;
                    //_DownStart = true;
                    //_movestart = 0;
                    _isMove = true;
                    StartCoroutine(MovePlayerCoroutine(Vector3.down));
                }
            }
            if (Input.GetKeyDown(_movedLeftKey) == true)
            {
                MapChip targetMapChip = mapData[mapChipIndex.x, mapChipIndex.y];
                MapChip destitaionMapChip = mapData[mapChipIndex.x, mapChipIndex.y - 1];
                // プレイヤーが現在いるマップチップと移動先のマップチップを比較する
                if (targetMapChip.CanMovePlayer[Common.MoveLeft] == true && destitaionMapChip.CanMovePlayer[Common.MoveRight] == true)
                {
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.None))
                        return;
                    //_LeftStart = true;
                    //_movestart = 0;
                    _isMove = true;
                    StartCoroutine(MovePlayerCoroutine(Vector3.left));
                }
            }
            if (Input.GetKeyDown(_movedUpKey) == true)
            {
                MapChip targetMapChip = mapData[mapChipIndex.x, mapChipIndex.y];
                MapChip destitaionMapChip = mapData[mapChipIndex.x - 1, mapChipIndex.y];
                // プレイヤーが現在いるマップチップと移動先のマップチップを比較する
                if (targetMapChip.CanMovePlayer[Common.MoveUp] == true && destitaionMapChip.CanMovePlayer[Common.MoveDown] == true)
                {
                    if (destitaionMapChip.MapChipAttribute == (MapChipAttribute.None))
                        return;
                    //_UpStart = true;
                    //_movestart = 0;
                    _isMove = true;
                    StartCoroutine(MovePlayerCoroutine(Vector3.up));
                }
            }
        }
    }

    #region Setter
    public void DisableKey()
    {
        _GetKey = false;
    }

    public void SetKeyImageColor(Color color)
    {
        _Keyimage.color = color;
    }

    private void SetMapChipData(MapChip mapChip)
    {
        _mapChip = mapChip;
    }
    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            _GetKey = true;
            _mapChip.RemoveKeyAttribute();
            _Keyimage.color = new Color(colorrock, colorrock, colorrock, colorrock);
            SoundManager.instance.PlaySE(SoundManager.E_SE.GetKey);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("Lock") && _GetKey == true)
        {
            _GetKey = false;
            _Keyimage.color = new Color(colorrock, colorrock, colorrock, 0.3f);
            SoundManager.instance.PlaySE(SoundManager.E_SE.GetKey);
            Destroy(other.gameObject);
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Map"))
        {
            _mapSprite = other.gameObject.GetComponent<SpriteRenderer>();
        }
    }

    private void LockMove(MapChip mapChip)
    {
        if (mapChip.MapChipAttribute == (MapChipAttribute.Use | MapChipAttribute.Lock))
        {
            _DownStart = true;
            _movestart = 0;
        }
    }

    /// <summary>
    /// 1プレイヤーの移動ルーチン
    /// </summary>
    /// <param name="direction">移動方向</param>
    /// <param name="waitForSecends">待機時間</param>
    /// <returns></returns>
    private IEnumerator MovePlayerCoroutine(Vector3 direction, float waitForSecends = 0.5f)
    {
        float movePlayerDistance = 0.0f;
        if (direction == Vector3.up || direction == Vector3.down)
        {
            while (_isMove == true)
            {
                if (movePlayerDistance >= _mapChip.gameObject.transform.localScale.y / 2)
                {
                    var mapChip = MapManager.Instance.GetMapChipData(transform.position.x, transform.position.y);
                    transform.position = mapChip.transform.position;
                    _isMove = false;
                    continue;
                }
                yield return new WaitForSeconds(waitForSecends);
                SoundManager.instance.PlaySE(SoundManager.E_SE.MovePlayer);
                transform.position += direction * _mapChip.gameObject.transform.localScale.y *  waitForSecends;
                movePlayerDistance += waitForSecends;

            }
        }
        else if (direction == Vector3.right || direction == Vector3.left)
        {
            while (_isMove == true)
            {
                if (movePlayerDistance >= _mapChip.gameObject.transform.localScale.x / 2)
                {
                    var mapChip = MapManager.Instance.GetMapChipData(transform.position.x, transform.position.y);
                    transform.position = mapChip.transform.position;
                    _isMove = false;
                    continue;
                }
                yield return new WaitForSeconds(waitForSecends);
                SoundManager.instance.PlaySE(SoundManager.E_SE.MovePlayer);
                transform.position += direction * _mapChip.gameObject.transform.localScale.x *  waitForSecends;
                movePlayerDistance += waitForSecends;
            }
        }
    }
}
