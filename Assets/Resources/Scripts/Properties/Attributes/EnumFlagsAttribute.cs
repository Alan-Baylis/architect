using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Allow the enum to be selected like a bit mask
    /// Requires enum to bit valued (1, 2, 4, 8, 16, 32)
    /// </summary>
    public class EnumFlagsAttribute : PropertyAttribute {
        public EnumFlagsAttribute() { }
    }

}
