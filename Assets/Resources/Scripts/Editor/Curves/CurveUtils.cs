using UnityEngine;
using UnityEditor;

namespace Resource.Editor {

    public static class CurveUtils {

        /// <summary>
        /// Draw a curve from the start to the end based on the sides of each rect the line should connect to.
        /// </summary>
        /// <param name="aStartSide">The side in which the line starts from the outgoing Rect</param>
        /// <param name="aEndSide">The side in which the line ends into the incoming Rect</param>
        public static void DrawCurve(Rect aStart, Rect aEnd, SIDE aStartSide = SIDE.right, SIDE aEndSide = SIDE.left) {
            Vector3 startPosition = GetSidePosition(aStart, aStartSide);
            Vector3 endPosition = GetSidePosition(aEnd, aEndSide);

            Vector3 startTangent = GetTangent(startPosition, endPosition, aStartSide);
            Vector3 endTangent = GetTangent(endPosition, startPosition, aEndSide);

            Handles.DrawBezier(startPosition, endPosition, startTangent, endTangent, Color.black, null, 1.5f);
        }

        /// <summary>
        /// Get the position on the side base don 
        /// </summary>
        private static Vector3 GetSidePosition(Rect aRect, SIDE aSide) {
            switch (aSide) {
                case SIDE.left:
                    return new Vector3(aRect.x, aRect.y + (aRect.height / 2.0f), 0);
                case SIDE.top:
                    return new Vector3(aRect.x + (aRect.width / 2.0f), aRect.y, 0);
                case SIDE.right:
                    return new Vector3(aRect.x + aRect.width, aRect.y + (aRect.height / 2.0f), 0);
                case SIDE.bottom:
                    return new Vector3(aRect.x + (aRect.width / 2.0f), aRect.y + aRect.height, 0);
                default:
                    return Vector3.zero;
            }
        }

        /// <summary>
        /// Get the tanget (offset) to curve the line. Based partially on the distance between both points.
        /// </summary>
        private static Vector3 GetTangent(Vector3 aPosition, Vector3 aTarget, SIDE aSide) {
            float distance = Mathf.Clamp(Vector3.Distance(aTarget, aPosition), 0, 50.0f);

            switch (aSide) {
                case SIDE.left:
                    return aPosition + (Vector3.left * distance);
                case SIDE.top:
                    return aPosition + (Vector3.down * distance);
                case SIDE.right:
                    return aPosition + (Vector3.right * distance);
                case SIDE.bottom:
                    return aPosition + (Vector3.up * distance);
                default:
                    return Vector3.zero;
            }
        }

    }

}
