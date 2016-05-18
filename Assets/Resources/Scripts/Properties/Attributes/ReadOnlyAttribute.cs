using UnityEngine;

namespace Resources.Properties {

    /// <summary>
    /// Display the given field as a read-only variable. 
    /// Disallows any modifications to take place in the inspector.
    /// </summary>
    public class ReadOnlyAttribute : PropertyAttribute {

        #region Constructor
        public ReadOnlyAttribute() { }
        #endregion

    }

}
