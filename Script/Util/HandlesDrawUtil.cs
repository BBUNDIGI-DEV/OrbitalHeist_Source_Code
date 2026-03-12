#if UNITY_EDITOR
using UnityEngine;

namespace UnityEditor
{
    public static class HandlesDrawUtil
    {
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 centerFrom, float angle, float distance, float thickness = 1.0f)
        {
            Handles.DrawWireArc(center, normal, centerFrom, angle / 2, distance, thickness);
            Handles.DrawWireArc(center, normal, centerFrom, -(angle / 2), distance, thickness);
        }


        public static void DrawCapsule(Vector3 position, float height, float radius, bool drawFromBase = true)
        {
            // Clamp the radius to a half of the capsule's height
            radius = Mathf.Clamp(radius, 0, height * 0.5f);

            Vector3 basePositionOffset = drawFromBase ? Vector3.zero : (Vector3.up * height * 0.5f);
            Vector3 baseArcPosition = position + Vector3.up * radius - basePositionOffset;
            DrawArc(180, 360, baseArcPosition, radius);

            float cylinderHeight = height - radius * 2.0f;
            DrawCylinder(baseArcPosition, cylinderHeight, radius);

            Vector3 topArcPosition = baseArcPosition + Vector3.up * cylinderHeight;

            DrawArc(0, 180, topArcPosition, radius);
        }

        public static void DrawArc(float startAngle, float endAngle, Vector3 position, float radius, bool drawChord = false, bool drawSector = false, int arcSegments = 32)
        {
            float arcSpan = Mathf.DeltaAngle(startAngle, endAngle);

            // Since Mathf.DeltaAngle returns a signed angle of the shortest path between two angles, it 
            // is necessary to offset it by 360.0 degrees to get a positive value
            if (arcSpan <= 0)
            {
                arcSpan += 360.0f;
            }

            // angle step is calculated by dividing the arc span by number of approximation segments
            float angleStep = (arcSpan / arcSegments) * Mathf.Deg2Rad;
            float stepOffset = startAngle * Mathf.Deg2Rad;

            // stepStart, stepEnd, lineStart and lineEnd variables are declared outside of the following for loop
            float stepStart = 0.0f;
            float stepEnd = 0.0f;
            Vector3 lineStart = Vector3.zero;
            Vector3 lineEnd = Vector3.zero;

            // arcStart and arcEnd need to be stored to be able to draw segment chord
            Vector3 arcStart = Vector3.zero;
            Vector3 arcEnd = Vector3.zero;

            // arcOrigin represents an origin of a circle which defines the arc
            Vector3 arcOrigin = position;

            for (int i = 0; i < arcSegments; i++)
            {
                // Calculate approximation segment start and end, and offset them by start angle
                stepStart = angleStep * i + stepOffset;
                stepEnd = angleStep * (i + 1) + stepOffset;

                lineStart.x = Mathf.Cos(stepStart);
                lineStart.y = Mathf.Sin(stepStart);
                lineStart.z = 0.0f;

                lineEnd.x = Mathf.Cos(stepEnd);
                lineEnd.y = Mathf.Sin(stepEnd);
                lineEnd.z = 0.0f;

                // Results are multiplied so they match the desired radius
                lineStart *= radius;
                lineEnd *= radius;

                // Results are multiplied by the orientation quaternion to rotate them 
                // since this operation is not commutative, result needs to be
                // reassigned, instead of using multiplication assignment operator (*=)

                // Results are offset by the desired position/origin 
                lineStart += position;
                lineEnd += position;

                // If this is the first iteration, set the chordStart
                if (i == 0)
                {
                    arcStart = lineStart;
                }

                // If this is the last iteration, set the chordEnd
                if (i == arcSegments - 1)
                {
                    arcEnd = lineEnd;
                }

                Handles.DrawLine(lineStart, lineEnd);
            }

            if (drawChord)
            {
                Handles.DrawLine(arcStart, arcEnd);
            }
            if (drawSector)
            {
                Handles.DrawLine(arcStart, arcOrigin);
                Handles.DrawLine(arcEnd, arcOrigin);
            }
        }

        public static void DrawCylinder(Vector3 position, float height, float radius)
        {
            Vector3 basePosition = position;
            Vector3 topPosition = basePosition + Vector3.up * height;

            DrawCircle(basePosition,Vector3.up ,radius, 32);
            DrawCircle(topPosition, Vector3.up, radius, 32);
        }

        public static void DrawCircle(Vector3 position, Vector3 normal, float radius, int segments)
        {
            if (radius <= 0.0f || segments <= 0)
            {
                return;
            }
            float angleStep = (360.0f / segments);

            // lineStart and lineEnd variables are declared outside of the following for loop
            Vector3 lineStart = Quaternion.LookRotation(normal) * Vector3.right * radius;

            for (int i = 0; i < segments; i++)
            {
                Vector3 lineEnd = Quaternion.AngleAxis(angleStep, normal) * lineStart;
                Handles.DrawLine(lineStart + position, lineEnd + position);
                lineStart = lineEnd;
            }
        }
    }
}
#endif