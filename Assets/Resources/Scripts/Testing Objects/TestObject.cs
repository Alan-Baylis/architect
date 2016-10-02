using UnityEngine;
using System.Collections;
using Resource.Properties;
using Resource.Structs;
using Resource.Utils;

namespace Testing {

    public class TestObject : MonoBehaviour {
        public int intValue = 0;

        [LimitedRange(0, 100)]
        public LimitedRange range;

        void Start() {
            Debug.Log(MathUtils.DigitCount(-100));
            Debug.Log(MathUtils.DigitCount(10.0f));
        }
    }

}
