using System;
using UnityEngine;

namespace AT.Core.Runtime.Basic
{
    public class CloneableObject : BasicObject, ICloneable
    {
        public object Clone()
        {
            return Instantiate(this);
        }
    }
}