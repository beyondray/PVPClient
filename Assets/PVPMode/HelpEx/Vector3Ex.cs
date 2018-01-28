using System;
using System.Collections;
using UnityEngine.Internal;

namespace UnityEngine
{
    public class Vector3Ex
    {
        public static Vector3 ToDir(Vector3 eulerAngles)
        {
            return new Vector3(eulerAngles.z, eulerAngles.x, eulerAngles.y);
        }

        public static Vector3 ToEuler(Vector3 direction)
        {
            return new Vector3(direction.y, direction.z, direction.x);
        }
    }
}
