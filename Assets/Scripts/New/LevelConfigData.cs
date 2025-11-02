using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace New
{
    public class LevelConfigData : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        private Dictionary<int, LevelConfig>  _pieceConfigs = new();
        
        private LevelConfigCsv[] _needConvertData;

        public void ConvertData()
        {
            _pieceConfigs = new();
            
            foreach (var item in _needConvertData)
            {
                _pieceConfigs.Add(item.level, item.ToLevelConfig());
            }
        }

        public LevelConfig GetPieceConfigs(int level)
        {
            return _pieceConfigs.TryGetValue(level, out var config) ? config : _pieceConfigs.LastOrDefault().Value;
        }
    }

    [Serializable]
    public struct LevelConfigCsv
    {
        public int level;
        public int tutId;
        public StickySpotCsv[] stickySpots;
        public PieceConfigCsv[]  pieces;

        public LevelConfig ToLevelConfig()
        {
            var listPieceConfigs = new List<PieceConfig>();
            foreach (var item in pieces)
            {
                listPieceConfigs.Add(item.ToPieceConfig());
            }
            
            var listStickySpots = new List<Vector2Int>();
            foreach (var item in stickySpots)
            {
                listStickySpots.Add(item.GetIndex());
            }

            return new LevelConfig()
            {
                tutId = tutId,
                pieces = listPieceConfigs,
                stickySpots = listStickySpots
            };
        }
    }

    public struct LevelConfig
    {
        public int tutId;
        public List<PieceConfig> pieces;
        public List<Vector2Int> stickySpots;
    }

    [Serializable]
    public struct StickySpotCsv
    {
        public int numberSticky;
        public Letter letterSticky;

        public Vector2Int GetIndex()
        {
            return new Vector2Int(numberSticky - 1, (int)letterSticky);
        }
    }
    
    [Serializable]
    public struct PieceConfigCsv
    {
        public int number;
        public Letter letter;
        public PieceType pieceType;
        public ColorType colorType;

        public PieceConfig ToPieceConfig()
        {
            return new PieceConfig()
            {
                x = number - 1,
                y = (int)letter,
                colorType = colorType,
                pieceType = pieceType
            };
        }
    }
    
    public struct PieceConfig
    {
        public int x;
        public int y;
        public PieceType pieceType;
        public ColorType colorType;
    }

    public enum Letter
    {
        a,
        b,
        c,
        d,
        e,
        f,
        g,
        h
    }
}