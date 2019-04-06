using g3;
using System;
using System.Collections.Generic;
using System.Text;

namespace planter
{
    class Intersector
    {
        public static List<Vector2d> CircelLineIntersection(
            Vector2d circleCenter,
            double r,
            Vector2d line1,
            Vector2d line2
            )
        {
            line1 -= circleCenter;
            line2 -= circleCenter;

            var result = new List<Vector2d>();

            var dx = line2.x - line1.x;
            var dy = line2.y - line1.y;
            var dr = Math.Sqrt(dx * dx + dy * dy);
            var D = line1.x * line2.y - line2.x * line1.y;

            var discriminant = r * r * dr * dr - D * D;
            if (discriminant < 0)
                return result;

            var drSquared = dr * dr;
            if (discriminant < 1e-6)
            {
                // tangent
                result.Add(new Vector2d(D * dy / (dr * dr), -D * dx / (drSquared)) + circleCenter);
            }
            else
            {
                var discriminantSqrt = Math.Sqrt(discriminant);

                // intersection
                var dySign = Math.Sign(dy);

                result.Add(new Vector2d(
                    (D * dy + dySign * dx * discriminantSqrt) / (drSquared), 
                    (-D * dx + Math.Abs(dy) * discriminantSqrt) / (drSquared)) + circleCenter);

                result.Add(new Vector2d(
                    (D * dy - dySign * dx * discriminantSqrt) / (drSquared),
                    (-D * dx - Math.Abs(dy) * discriminantSqrt) / (drSquared)) + circleCenter);
            }

            return result;
        }
    }
}
