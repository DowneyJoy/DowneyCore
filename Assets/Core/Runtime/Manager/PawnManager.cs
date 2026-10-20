using System.Collections.Generic;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.GamePlay;
using AT.Core.Runtime.Utils;
using Unity.Logging;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Manager
{
    public class PawnManager : AutoSingleton<PawnManager>, ISingleton,IPersistence
    {
        private List<IPawn> _pawns = new List<IPawn>();
        public IPawn GetPawn(string playerId)
        {
            return _pawns.Find(x=>x.pawnId == playerId);
        }

        public void Join(PlayerInstance player)
        {
            string pawnId = player.id;
            string pawnName = player.name;

            if (_pawns.Find(x => x.pawnId.Equals(pawnId)) != null)
            {
                Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Handle Player Join at @scene: {DebugInfo.@scene} at @frame: {DebugInfo.@frame}, State: Failed, Reason: Player Already Joined the World.");

            }

            GameObject prefab =
                AssetsManager.instance.LoadAsset<GameObject>(ContentDefinitions.PAWN_ADDRESS, AssetType.GameObject);
            GameObject vessel = Instantiate(prefab,PlayerStarter.position, PlayerStarter.rotation);
            
            IPawn pawn = vessel.GetComponent<IPawn>();
            pawn.BindPlayer(pawnName, pawnId);
            
            _pawns.Add(pawn);
        }

        public void Single(string id)
        {
            _pawns.Clear();
        }
    }
}