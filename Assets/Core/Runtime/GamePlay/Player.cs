using System;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Data;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.Manager;
using AT.Core.Runtime.Utils;
using Newtonsoft.Json;
using UnityEngine;

namespace AT.Core.Runtime.GamePlay
{
    public class Player : AutoSingleton<Player>,ISingleton,IPersistence
    {
        private PlayerInfo _info;
        private PlayerInstance _player;
        
        public PlayerInstance GetPlayerInstance() => _player;
        void ISingleton.Single(string id)
        {
            LoadInformation();
            CreateInstance();
            //RegisterInput();
        }
        
        void LoadInformation()
        {
            string json = PlayerPrefs.GetString(ContentDefinitions.PLAYER_INFO_KEY);

            if (string.IsNullOrEmpty(json))
            {
                NewInformation(ContentDefinitions.PLAYER_INFO_KEY);
                return;
            }
            _info = JsonConvert.DeserializeObject<PlayerInfo>(json);
        }

        void NewInformation(string key)
        {
            _info = new PlayerInfo(PlayerInfo.GenerateID(), "Player");
            string json = JsonConvert.SerializeObject(_info);
            PlayerPrefs.SetString(key,json);
        }

        void CreateInstance()
        {
            _player = new PlayerInstance(_info.id, _info.name,PawnManager.instance.GetPawn(_info.id));
        }

        public void RegisterInput()
        {
            InputManager.instance.RegisterAction(InputDefinitions.MOVE_INPUT);
            InputManager.instance.RegisterAction(InputDefinitions.LOOK_INPUT);
        }

        private void Update()
        {
            ProcessInput();
        }

        void ProcessInput()
        {
            Get(out Vector2 move,out Vector2 look);
            Process(move, look);
            void Get(out Vector2 move, out Vector2 look)
            {
                move = InputManager.instance.GetValue<Vector2>("move");
                look = InputManager.instance.GetValue<Vector2>("look");
            }

            void Process(Vector2 move, Vector2 look)
            {
                _player.pawn?.MoveInput(move);
            }
            
        }

        public void Join()
        {
            PawnManager.instance.Join(_player);
            
            PlayerInstance player = _player;
            player.pawn = PawnManager.instance.GetPawn(_player.id);
            _player = player;
        }
    }
}