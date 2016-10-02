using UnityEngine;

namespace Resource.Properties {

    /// <summary>
    /// Require the given property to be set to true (active) for this property to be enabled and modifiable.
    /// If the given property is set to false (inactive) then this property will be disabled.
    /// </summary>
    public class RequirePropertyAttribute : PropertyAttribute {
        private string propertyName;

        #region Getters & Setters
        public string PropertyName {
            get { return propertyName; }
        }
        #endregion

        #region Constructor
        public RequirePropertyAttribute(string aPropertyName) {
            propertyName = aPropertyName;
        }
        #endregion

    }

}
