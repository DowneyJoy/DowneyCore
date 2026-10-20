using System;
using AT.Core.Runtime.Basic;
using Newtonsoft.Json;
using UnityEngine;

namespace AT.Core.Runtime.Data
{
    [CreateAssetMenu(menuName = "DowneyJoy/Data/Pawn Data")]
    public class PawnData : CloneableObject
    {
        [SerializeField] private int agility = 30;

        public float moveSpeed => 2f + agility / 10f;

        public Data ToData()
        {
            Data data = new Data();
            data.LoadFromPawnData(this);
            return data;
        }
        [Serializable]
        public class Data
        {
            [JsonProperty]public int agility{get;set;}

            public Data()
            {
                agility = 30;
            }

            public Data(int agility)
            {
                this.agility = agility;
            }

            public void LoadFromPawnData(PawnData data)
            {
                agility = data.agility;
            }

            public PawnData ToPawnData()
            {
                PawnData data = CreateInstance<PawnData>();
                data.agility =  agility;
                return data;
            }
        }
    }
}