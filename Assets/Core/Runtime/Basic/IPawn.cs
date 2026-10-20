using UnityEngine;

namespace AT.Core.Runtime.Basic
{
    public interface IPawn
    {
        public string pawnId { get; set; }
        void MoveInput(Vector2 input);
        void Teleport(Vector3 position);
        void BindPlayer(string name, string id);
    }
}