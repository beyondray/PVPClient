using UnityEngine;
using System;
using System.Collections;

namespace KBEngine {
    public class PropsEntity : Entity
   {
        public Vector3 eulerAngles
        {
            get
            {
                return new Vector3(direction.y, direction.z, direction.x);
            }
            set
            {
                direction.x = value.z;
                direction.y = value.x;
                direction.z = value.y;
            }
        }

        public Quaternion rotation
        {
            get
            {
                return Quaternion.Euler(eulerAngles);
            }
            set
            {
                direction = value.eulerAngles;
            }
        }

        public string name
        {
            get
            {
                return (string)getDefinedProperty("name");
            }
        }

        public float speed
        {
            get
            {
                return (float)getDefinedProperty("moveSpeed");
            }
        }

        public virtual void set_HP(object old)
        {
            object v = getDefinedProperty("HP");
            Event.fireOut("set_HP", new object[] { this, v });
        }

        public virtual void set_HP_Max(object old)
        {
            object v = getDefinedProperty("HP_Max");
            Event.fireOut("set_HP_Max", new object[] { this, v });
        }

        public virtual void set_MP(object old)
        {
            object v = getDefinedProperty("MP");
            Event.fireOut("set_MP", new object[] { this, v });
        }

        public virtual void set_MP_Max(object old)
        {
            object v = getDefinedProperty("MP_Max");
            Event.fireOut("set_MP_Max", new object[] { this, v });
        }

        public virtual void set_PP(object old)
        {
            object v = getDefinedProperty("PP");
            Event.fireOut("set_PP", new object[] { this, v });
        }

        public virtual void set_PP_Max(object old)
        {
            object v = getDefinedProperty("PP_Max");
            Event.fireOut("set_PP_Max", new object[] { this, v });
        }

        public virtual void set_moveSpeed(object old)
        {
            Event.fireOut("set_moveSpeed", new object[] { this });
        }

        public virtual void set_level(object old)
        {
            object v = getDefinedProperty("level");
            Event.fireOut("set_level", new object[] { this, v });
        }

        public virtual void set_name(object old)
        {
            Event.fireOut("set_name", new object[] { this });
        }
    }
}