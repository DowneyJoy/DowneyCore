using System.Collections.Generic;
using System.IO;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Definitions;
using AT.Core.Runtime.GamePlay;
using AT.Core.Runtime.Utils;
using Unity.Logging;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace AT.Core.Runtime.Manager
{
    public class InputManager : MonoSingleton<InputManager>,ISingleton,IPersistence
    {
        [SerializeField] private InputActionAsset defaultInputAsset;

        private InputActionAsset _inputAsset;
        private readonly Dictionary<int,InputAction> _actions = new Dictionary<int, InputAction>();

        void ISingleton.Single(string id)
        {
            Load(id);
            InitInput();
            Player.instance.RegisterInput();
        }

        void Load(string id)
        {
            string file = ContentDefinitions.GetInputActionsPath();
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Load Input Settings at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Pending");
            _inputAsset = !File.Exists(file)? CreateDefaultInputAsset(file):ReadInputAsset(file);
        }

        void InitInput()
        {
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Init Input at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Success");
            _inputAsset.Enable();
            _actions.Clear();
        }

        InputActionAsset CreateDefaultInputAsset(string file)
        {
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Created Default Input at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Success");

            string json = defaultInputAsset.ToJson();
            File.WriteAllText(file, json);
            return defaultInputAsset;
        }

        InputActionAsset ReadInputAsset(string file)
        {
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Read Input at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Success");

            string json = File.ReadAllText(file);
            var action = new InputActionAsset();
            action.LoadFromJson(json);
            return action;
        }

        public void RegisterAction(string actionName)
        {
            int hash = actionName.GetHashCode();
            if (_actions.ContainsKey(hash))
            {
                Log.Warning($"Object: {id}, Type: {GetType().FullName}, Action: Register Input Action at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Failed, Reason: Action Exist.");
                return;
            }
            
            InputAction action = _inputAsset.FindAction(actionName);
            if (action == null)
            {
                Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Register Input Action at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}, State: Failed, Reason: Action Exist.");
                return;
            }
            _actions[hash] = action;
        }
        bool Validate(string actionName,out InputAction action)
        {
            int hash = actionName.GetHashCode();
            if (!_actions.TryGetValue(hash,out action))
            {
                Log.Error($"Object: {id}, Type: {GetType().FullName}, Action: Validate Action at @scene: {DebugInfo.scene} at @frame: {DebugInfo.frame}, State: Failed, Reason: Action Exist.");
                return false;
            }
            return true;
        }

        public TValue GetValue<TValue>(string actionName) where TValue : struct
        {
            if (!Validate(actionName,out InputAction action))
            {
                return default;
            }
            return action.ReadValue<TValue>();
        }

        public bool GetButtonDown(string actionName)
        {
            if (!Validate(actionName, out InputAction action))
            {
                return false;
            }

            return action.WasPressedThisFrame();
        }
        
        public bool GetButtonUp(string actionName)
        {
            if (!Validate(actionName, out InputAction action))
            {
                return false;
            }

            return action.WasReleasedThisFrame();
        }
        
        public bool GetButton(string actionName)
        {
            if (!Validate(actionName, out InputAction action))
            {
                return false;
            }

            return action.IsPressed();
        }
        
        public void ChangeBinding(string actionName, string newBinding)
        {
            if (!Validate(actionName, out InputAction action))
            {
                return;
            }

            action.ChangeBindingWithPath(newBinding);
        }

        public void SaveInputSettings()
        {
            string file = ContentDefinitions.GetInputActionsPath();
            File.WriteAllText(file,_inputAsset.ToJson());
            Log.Info($"Object: {id}, Type: {GetType().FullName}, Action: Save Input Settings, State: Success at @scene: {SceneManager.GetActiveScene().name} at @frame: {Time.frameCount}");
        }
    }
}