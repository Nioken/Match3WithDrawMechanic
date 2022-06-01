using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public struct TileInfo
    {
        public int X;
        public int Y;
        public bool IsBarried;
        public TileInfo(int X, int Y, bool IsBarried)
        {
            this.X = X;
            this.Y = Y;
            this.IsBarried = IsBarried;
        }
    }
    public struct BarrierInfo
    {
        public int heal;
        public int X;
        public int Y;
        public Barrier.BarrierType barrierType;
        public BarrierInfo(int X,int Y,int heal, Barrier.BarrierType barrierType)
        {
            this.X = X;
            this.Y = Y;
            this.heal = heal;
            this.barrierType = barrierType;
        }
    }
    public bool isConfigured;
    public int Steps;
    public int X;
    public int Y;
    public int ScoreQuest;
    public int ItemQuest;
    public int BarrierQuest;
    public BarrierInfo[,] AllBariers;
    public TileInfo[,] AllTiles;
}
