using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Lucky.Interactive.Grid_
{
    /// <summary>
    /// transform当前位置对应grid最左下角单元的最左下角
    /// 这个Grid基于UI，位置不提供就基于屏幕坐标
    /// 由于理论上我希望世界空间和屏幕空间完全一样(所以以后pos既指屏幕空间, 又指世界空间)
    /// </summary>
    public class Grid : InteractableUI
    {
        public float cellWidth = 100;
        public float cellHeight = 100;
        public int rows = 4;
        public int cols = 4;
        public int[,] map;
        public bool isCellPivotInMiddle = true; // true为中间, false为左下角
        public Vector2 pivotOffset = Vector2.zero;

        public Vector2 CellSize => new Vector2(cellWidth, cellHeight);
        public Vector2 OrigPos => transform.position + (Vector3)pivotOffset + (isCellPivotInMiddle ? -new Vector3(cellWidth, cellHeight) / 2 : Vector3.zero);
        public float TotalWidth => cellWidth * cols;
        public float TotalHeight => cellHeight * rows;
        public static Vector2 halfOne = new Vector2(0.5f, 0.5f);
        public float Left => OrigPos.x;
        public float Right => Left + TotalWidth;
        public float Bottom => OrigPos.y;
        public float Top => Bottom + TotalHeight;

        protected virtual void Awake()
        {
            map = new int[cols, rows];
        }

        public int this[int x, int y]
        {
            get => map[x, y];
            set => map[x, y] = value;
        }

        public Vector2Int GetGridPosByMousePos() => GetGridPosByPos(Input.mousePosition - new Vector3(1920, 1080) / 2);

        public Vector2Int GetGridPosByPos(Vector2 pos) => GetGridPosByPos(pos.x, pos.y);
        public List<Vector2Int> GetGridPosesByWorldPoses(List<Vector2> screenPoses) => screenPoses.Select(GetGridPosByPos).ToList();

        public Vector2Int GetGridPosByPos(float x, float y)
        {
            Vector2 pivot = OrigPos;
            int gridX = Mathf.FloorToInt((x - pivot.x) / cellWidth);
            int gridY = Mathf.FloorToInt((y - pivot.y) / cellHeight);
            return new Vector2Int(gridX, gridY);
        }



        public bool OverlapGridPoint(Vector2 point) => OverlapGridPoint(point.x, point.y);
        public bool OverlapGridPoint(float x, float y) => OverlapGridPoint((int)Math.Floor(x), (int)Math.Floor(y));
        public bool OverlapGridPoint(int x, int y) => x >= 0 && x < cols && y >= 0 && y < rows;
        public bool OverlapWorldPoint(float x, float y) => x >= Left && x < Right && y >= Bottom && y < Top;

        public bool OverlapWorldPoint(Vector2 point) => OverlapWorldPoint(point.x, point.y);
        public bool OverlapWorldPoint(Vector2Int point) => OverlapWorldPoint(point.x, point.y);

        public bool OverlapWorldPointtAll(List<Vector2> points) => points.All(OverlapWorldPoint);
        public bool OverlapWorldPointtAll(List<Vector2Int> points) => points.All(OverlapWorldPoint);

        public bool CheckValidByWorldPoses(bool isRelative, List<Vector2> poses) => CheckValidByWorldPoses(isRelative, poses.ToArray());

        public bool CheckValidByGridPoses(List<Vector2Int> poses) => CheckValidByGridPoses(poses.ToArray());
        public bool CheckValidByGridPoses(params Vector2Int[] poses)
        {
            foreach ((int x, int y) in poses)
            {
                // 如果不在网格内
                if (x < 0 || x >= cols || y < 0 || y >= rows)
                    return false;
                // 如果该格已被占用
                if (map[x, y] == 1)
                    return false;
            }

            return true;
        }

        public bool CheckValidByWorldPoses(bool isRelative, params Vector2[] poses)
        {
            Vector2Int[] gridPoses = new Vector2Int[poses.Length];
            int i = 0;
            foreach ((float x, float y) in poses)
            {
                int nx = isRelative ? GetGridPosByPos(new Vector2(x, y) + OrigPos).x : GetGridPosByPos(x, y).x;
                int ny = isRelative ? GetGridPosByPos(new Vector2(x, y) + OrigPos).y : GetGridPosByPos(x, y).x;
                gridPoses[i++] = new Vector2Int(nx, ny);
            }

            return CheckValidByGridPoses(gridPoses);
        }

        /// <summary>
        /// 用网格坐标
        /// </summary>
        public void Put(int val = 1, params Vector2Int[] poses)
        {
            foreach ((int x, int y) in poses)
                map[x, y] = val;
        }
    }
}