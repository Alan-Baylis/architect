using UnityEngine;
using System;

namespace Resources.Properties {

    /// <summary>
    /// 
    /// </summary>
    public class ComponentPopupAttribute : PropertyAttribute {
        private Type type;

        #region Getters & Setters
        public Type Type {
            get { return type; }
        }
        #endregion

        #region Constructor
        public ComponentPopupAttribute(Type aType) {
            type = aType;
        }
        #endregion

    }

}
