using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace New
{
    public class LevelConfigData : SerializedScriptableObject
    {
        [NonSerialized, OdinSerialize]
        private Dictionary<int, List<PieceConfig>>  _pieceConfigs = new();
        
        private LevelConfigCsv[] _needConvertData;

        public void ConvertData()
        {
            _pieceConfigs = new();
            
            foreach (var item in _needConvertData)
            {
                var list = new List<PieceConfig>();
                foreach (var pieceConfigCsv in item.pieces)
                {
                    list.Add(pieceConfigCsv.ToPieceConfig());
                }
                
                _pieceConfigs.Add(item.level, list);
            }
        }

        public List<PieceConfig> GetPieceConfigs(int level)
        {
            return _pieceConfigs.TryGetValue(level, out var config) ? config : _pieceConfigs.LastOrDefault().Value;
        }
    }

    [Serializable]
    public struct LevelConfigCsv
    {
        public int level;
        public PieceConfigCsv[]  pieces;
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