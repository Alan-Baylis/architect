using UnityEditor;
using System.Reflection;
using System;

namespace Resource.Utils {

    public static class Reflection {

        #region Property Values
        public static T GetPrivatePropertyValue<T>(this object aObject, string aPropertyName) {
            if (aObject == null) {
                throw new ArgumentNullException("aObject");
            }

            PropertyInfo propertyInfo = aObject.GetType().GetProperty(aPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (propertyInfo == null) {
                throw new ArgumentOutOfRangeException(string.Format("{0} was not found in Type {1}", aPropertyName, aObject.GetType().FullName));
            }

            return (T) propertyInfo.GetValue(aObject, null);
        }

        public static void SetPrivatePropertyValue<T>(this object aObject, string aPropertyName, T aValue) {
            Type type = aObject.GetType();
            if (type.GetProperty(aPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance) == null) {
                throw new ArgumentOutOfRangeException(string.Format("{0} was not found in Type {1}", aPropertyName, aObject.GetType().FullName));
            }

            type.InvokeMember(aPropertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetProperty | BindingFlags.Instance, null, aObject, new object[] { aValue });
        }
        #endregion

        #region Field Values
        public static T GetPrivateFieldValue<T>(this object aObject, string aFieldName) {
            if (aObject == null) {
                throw new ArgumentNullException("aObject");
            }

            Type type = aObject.GetType();
            FieldInfo field = null;

            while (field == null && type != null) {
                field = type.GetField(aFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
            }

            if (field == null) {
                throw new ArgumentOutOfRangeException(string.Format("{0} was not found in Type {1}", aFieldName, aObject.GetType().FullName));
            }

            return (T) field.GetValue(aObject);
        }

        public static void SetPrivateFieldValue<T>(this object aObject, string aFieldName, T aValue) {
            if (aObject == null) {
                throw new ArgumentNullException("aObject");
            }

            Type type = aObject.GetType();
            FieldInfo field = null;

            while (field == null && type != null) {
                field = type.GetField(aFieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                type = type.BaseType;
            }

            if (field == null) {
                throw new ArgumentOutOfRangeException(string.Format("{0} was not found in Type {1}", aFieldName, aObject.GetType().FullName));
            }

            field.SetValue(aObject, aValue);
        }
        #endregion
    }

}
