using g3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace planter
{
    class Generators
    {
        public static List<Vector2d> counterClockwiseCirclePoints(
            int pointsInCircle,
            Vector2d center,
            double radius)
        {
            var polar = new Polar() { R = radius };
            var delta = 2 * Math.PI / pointsInCircle;

            return Enumerable.Range(0, pointsInCircle).Select(idx =>
            {
                return center + new Polar() { R = radius, Theta = idx * delta }.toCartesian();
            }).ToList();
        }

        public static List<Vector2d> counterClockwiseCircleSegments(
            int pointsInBend,
            Vector2d center,
            Vector2d start,
            Vector2d end)
        {
            if (pointsInBend < 0)
                throw new ArgumentException("Points count is negative", nameof(pointsInBend));

            var startPolar = center.toPolar(start);
            var endPolar = center.toPolar(end);

            var bendStep = startPolar.Theta < endPolar.Theta
                ? (endPolar.Theta - startPolar.Theta) / (pointsInBend + 1)
                : (endPolar.Theta + 2 * Math.PI - startPolar.Theta) / (pointsInBend + 1);

            var bendPoints = new List<Vector2d>();
            bendPoints.Add(start);

            bendPoints.AddRange(Enumerable.Range(1, pointsInBend).Select(idx =>
            {
                return new Polar() { R = startPolar.R, Theta = startPolar.Theta + bendStep * idx }.toCartesian() + center;
            }));

            bendPoints.Add(end);

            return bendPoints;
        }
    }
}
