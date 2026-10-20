using System;
using System.Collections.Generic;
using System.IO;
using AT.Core.Runtime.Basic;
using AT.Core.Runtime.Data;
using AT.Core.Runtime.Definitions;
using Newtonsoft.Json;
using Unity.Logging;
using UnityEngine;

namespace AT
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Pawn : BasicBehaviour,IPawn,IPersistence
    {
        private List<int> gos;
        [SerializeField] private PawnData _data;
        private Rigidbody2D _rb;
        private RigidbodyType2D _rbType;
        private string _id;
        
        private Vector2 _moveInput;
        private Vector2 _velocity;
        private Vector2 _acceleration;
        private Vector2 _actualVelocity;
        
        private float _moveSpeed;
        private void OnEnable()
        {
            OnInit();
        }

        private void Awake()
        {
            InitComponents();
            InitDatas();
        }

        private void Update()
        {
            UpdateDatas();
        }

        void FixedUpdate()
        {
            Movement(out float accelX, out float accelY);
            UpdatePhysicsData(accelX, accelY);
        }
        #region AWAKE
        void InitComponents()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void InitDatas()
        {
            InitRigidbody(_rb);
            InitPawn(out _data);
            return;
            void InitRigidbody(Rigidbody2D rb)
            {
                rb.velocity = Vector2.zero;
                rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.interpolation = RigidbodyInterpolation2D.Interpolate;
                rb.drag = PhysicsData.Friction;
                rb.angularDrag = 0;
                rb.gravityScale = 0;
                rb.inertia = 1f;

                _rbType = rb.bodyType;
                if (rb.IsSleeping())
                {
                    rb.WakeUp();
                }
            }

            void InitPawn(out PawnData dat)
            {
                string file = ContentDefinitions.GetPawnDataPath();
                if (!File.Exists(file))
                {                
                    dat = _data.Clone() as PawnData;
                    string json = JsonConvert.SerializeObject(_data.ToData(), Formatting.Indented);
                    File.WriteAllText(file, json);
                }
                else
                {
                    string json = File.ReadAllText(file);
                    PawnData.Data data = JsonConvert.DeserializeObject<PawnData.Data>(json);
                    dat = data.ToPawnData();
                } 
            }
        }
        #endregion
        #region Update

        void UpdateDatas()
        {
            _velocity = _rb.velocity;
            _moveSpeed = _data.moveSpeed;
        }
        #endregion
        #region FixedUpdate

        void Movement(out float accelX, out float accelY)
        {
            float current_vel_x = _velocity.x;
            float current_vel_y = _velocity.y;
            
            float horziontal = _moveInput.x > 0 ? 1f : _moveInput.x < 0 ? -1f : 0f;
            float vertical = _moveInput.y > 0 ? 1f : _moveInput.y < 0 ? -1f : 0f;

            float wish_vel_x = horziontal * _moveSpeed;
            float wish_vel_y = vertical * _moveSpeed;
            
            float dif_vel_x = wish_vel_x - current_vel_x;
            float dif_vel_y = wish_vel_y - current_vel_y;
            
            // v = v0 + at, a = (v - v0) / t = delta v / delta t;
            float accel_x = dif_vel_x / Time.deltaTime;
            float accel_y = dif_vel_y / Time.deltaTime;
            
            bool accel_on_x = Mathf.Abs(wish_vel_x) >  Mathf.Abs(current_vel_x);
            bool accel_on_y = Mathf.Abs(wish_vel_y) >  Mathf.Abs(current_vel_y);

            float deceleration_x = horziontal * PhysicsData.Friction;
            float deceleration_y = vertical * PhysicsData.Friction;

            // 有输入，应用加速度
            if (horziontal != 0)
            {
                _actualVelocity.x = accel_on_x ? current_vel_x + accel_x * Time.fixedDeltaTime : wish_vel_x;
            }
            // 无输入，应用减速度
            else
            {
                _actualVelocity.x = Mathf.MoveTowards(_actualVelocity.x,0,PhysicsData.Friction * Time.fixedDeltaTime);
            }
            // 有输入，应用加速度
            if (vertical != 0)
            {
                _actualVelocity.y = accel_on_y ? current_vel_y + accel_y * Time.fixedDeltaTime : wish_vel_y;
            }
            // 无输入，应用减速度
            else
            {
                _actualVelocity.y = Mathf.MoveTowards(_actualVelocity.y, 0, PhysicsData.Friction * Time.fixedDeltaTime);
            }

            accelX = accel_x;
            accelY = accel_y;
            _rb.velocity = new Vector2(_actualVelocity.x, _actualVelocity.y);
        }

        void UpdatePhysicsData(float accelX, float accelY)
        {
            _velocity = _rb.velocity;
            _acceleration = new Vector2(accelX, accelY);
        }
        #endregion
        
        #region Implementation
        
        public string pawnId { get; set; }
        public void MoveInput(Vector2 input)
        {
            _moveInput = input;
        }

        public void Teleport(Vector3 position)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            
            transform.position = position;
            _rb.bodyType = _rbType;

            string obj = $"Object: {id}('{name}')";
            string join = $"Teleported to";
            string coordStr = $"{position.x:0.000} {position.y:0.000} {position.z:0.000}";
            
            Log.Info($"{obj} {join} {coordStr}");
        }

        public void BindPlayer(string name, string id)
        {
            gameObject.name = $"{name}_{id}";
            pawnId = id;
        }

        #endregion

    }
}