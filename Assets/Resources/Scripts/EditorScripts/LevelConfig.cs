using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public bool IsConfigured;
    public int Steps;
    public int X;
    public int Y;
    public bool ScoreQuest;
    public bool ItemQuest;
    public bool BarrierQuest;
    public BarrierInfo[,] AllBariers;
    public TileInfo[,] AllTiles;

    public struct TileInfo
    {
        public int X;
        public int Y;
        public bool IsBarried;
        public TileInfo(int x, int y, bool isBarried)
        {
            X = x;
            Y = y;
            IsBarried = isBarried;
        }
    }
    public struct BarrierInfo
    {
        public int Heal;
        public int X;
        public int Y;
        public Barrier.BarrierType Type;
        public BarrierInfo(int x, int y, int heal, Barrier.BarrierType type)
        {
            X = x;
            Y = y;
            Heal = heal;
            Type = type;
        }
    }
    public struct LevelInfo
    {
        public int Steps;
        public int X;
        public int Y;
        public bool ScoreQuest;
        public bool ItemQuest;
        public bool BarrierQuest;
        public LevelInfo(int steps, int x, int y, bool scoreQuest, bool itemQuest, bool barrierQuest)
        {
            Steps = steps;
            X = x;
            Y = y;
            ScoreQuest = scoreQuest;
            ItemQuest = itemQuest;
            BarrierQuest = barrierQuest;
        }
    }
}
