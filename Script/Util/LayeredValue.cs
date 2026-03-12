using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Utility
{
    public class LayeredValue<TPriorityEnum,V> where TPriorityEnum : System.Enum
    {
        private V[] mValueArr;
        private bool[] mIsValueEnrolled;
        private V mDefaultValue;

        public LayeredValue() : this(default(V))
        {
        }

        public LayeredValue(V defaultValue)
        {
            mDefaultValue = defaultValue;
            int length = System.Enum.GetValues(typeof(TPriorityEnum)).Length;
            mValueArr = new V[length];
            mIsValueEnrolled = new bool[length];
        }

        public void EnrollValue(TPriorityEnum key, V value)
        {
            int index = Unsafe.As<TPriorityEnum, int>(ref key);
            mValueArr[index] = value;
            mIsValueEnrolled[index] = true;
        }

        public void DisEnrollValue(TPriorityEnum key)
        {
            int index = Unsafe.As<TPriorityEnum, int>(ref key);
            mIsValueEnrolled[index] = false;
        }


        public V GetValue()
        {
            for (int i = 0; i < mIsValueEnrolled.Length; i++)
            {
                if(mIsValueEnrolled[i])
                {
                    return mValueArr[i];
                }
            }

            return mDefaultValue;
        }
    }
}

