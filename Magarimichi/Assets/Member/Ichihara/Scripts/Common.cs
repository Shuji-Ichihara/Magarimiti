using UnityEngine;

/// <summary>
/// 拡張メソッドを定義するクラス
/// </summary>
public static class Extensions
{
    /// <summary>
    /// 二次元配列のインデックスを取得する
    /// </summary>
    /// <typeparam name="T">ジェネリック化</typeparam>
    /// <param name="array"> element が格納されている配列</param>
    /// <param name="element">インデックスを取得したい要素</param>
    /// <returns>element が存在する配列のインデックス (二次元配列のインデックスを返す為、Vector2Int を 2 つのインデックスのまとまりとして返す)</returns>
    public static Vector2Int GetIndex<T>(this T[,] array, T element) where T : MapChip
    {
        int line = 0, column = 0;
        for (int i = 0; i < array.GetLength(1); i++)
        {
            for (int j = 0; j < array.GetLength(0); j++)
            {
                if (array[j, i] == element)
                {
                    line = j;
                    column = i;
                }
            }
        }
        return new Vector2Int(line, column);
    }
}

public class Common
{
    // プレイヤーの移動に使用する。 MapChip の CanMovePlayer のキー
    public static readonly string MoveUp = "MoveUp";
    public static readonly string MoveDown = "MoveDown";
    public static readonly string MoveLeft = "MoveLeft";
    public static readonly string MoveRight = "MoveRight";
}