using UnityEngine;
using System.Collections;

namespace Resource.Editor {

    public class Curve {
        private Rect originRect;
        private Rect targetRect;
        private SIDE originSide = SIDE.right;
        private SIDE targetSide = SIDE.left;
        private Color curveColor = Color.black;

        #region Getters & Setters
        public Rect OriginRect {
            get { return originRect; }
        }

        public Rect TargetRect {
            get { return targetRect; }
        }

        public Color CurveColor {
            get { return curveColor; }
        }

        public SIDE OriginSide {
            get { return originSide; }
        }

        public SIDE TargetSide {
            get { return targetSide; }
        }
        #endregion

        #region Constructors
        public Curve(Rect aStartRect, Rect aEndRect) {
            originRect = aStartRect;
            targetRect = aEndRect;
        }

        public Curve(Rect aStartRect, Rect aEndRect, Color aCurveColor) {
            originRect = aStartRect;
            targetRect = aEndRect;
            curveColor = aCurveColor;
        }

        public Curve(Rect aStartRect, Rect aEndRect, Color aCurveColor, SIDE aOriginSide, SIDE aTargetSide) {
            originRect = aStartRect;
            targetRect = aEndRect;
            curveColor = aCurveColor;
            originSide = aOriginSide;
            targetSide = aTargetSide;
        }
        #endregion

        #region Utility Functions
        public void UpdateCurve(Rect aTargetRect) {
            targetRect = aTargetRect;
        }

        public void UpdateCurve(Color aCurveColor) {
            curveColor = aCurveColor;
        }

        public void UpdateCurve(SIDE aOriginSide, SIDE aTargetSide) {
            originSide = aOriginSide;
            targetSide = aTargetSide;
        }
        #endregion

    }

}
