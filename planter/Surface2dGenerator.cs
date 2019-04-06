using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace planter
{
    public class Surface2dGenerator
    {
        public PlateGeneratorContext Context;
        public IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        public List<List<Vector2d>> OuterJointPoints = new List<List<Vector2d>>();

        public Surface2dGenerator(PlateGeneratorContext context)
        {
            Context = context;
        }

        public IntermediateMeshGenerator Generate()
        {
            var plateSegmentGenerator = Context.Plate2d;

            for (var segIdx = 0; segIdx < Context.Config.segments; segIdx++)
            {
                var angle = Context.Config.SegmentAngle(segIdx);
                var nextAngle = Context.Config.SegmentAngle(segIdx + 1);

                Intermediate.counterClockwiseSingleSidedFromFront(
                    plateSegmentGenerator.InnerBendCorner.rotate2d(angle).to3d(),
                    plateSegmentGenerator.OuterDownCornerPoint.rotate2d(nextAngle).to3d(),
                    plateSegmentGenerator.InnerBendCorner.rotate2d(nextAngle).to3d()
                   );

                Intermediate.counterClockwiseSingleSidedFromFront(
                    plateSegmentGenerator.InnerBendCorner.rotate2d(angle).to3d(),
                    plateSegmentGenerator.OuterUpCornerPoint.rotate2d(angle).to3d(),
                    plateSegmentGenerator.OuterDownCornerPoint.rotate2d(nextAngle).to3d()
                   );

                for (var pointIdx = 0; pointIdx < plateSegmentGenerator.OuterSurfaceNearPoints.Count - 1; pointIdx++)
                {
                    Intermediate.counterClockwiseSingleSidedFromFront(
                        plateSegmentGenerator.OuterSurfaceNearPoints[pointIdx].rotate2d(angle).to3d(),
                        plateSegmentGenerator.OuterSurfaceFarPoints[pointIdx + 1].rotate2d(angle).to3d(),
                        plateSegmentGenerator.OuterSurfaceNearPoints[pointIdx + 1].rotate2d(angle).to3d()
                       );

                    Intermediate.counterClockwiseSingleSidedFromFront(
                        plateSegmentGenerator.OuterSurfaceNearPoints[pointIdx].rotate2d(angle).to3d(),
                        plateSegmentGenerator.OuterSurfaceFarPoints[pointIdx].rotate2d(angle).to3d(),
                        plateSegmentGenerator.OuterSurfaceFarPoints[pointIdx + 1].rotate2d(angle).to3d()
                       );
                }

                // inner segment joint
                var jointCenterPoint = Vector2d.AxisX.rotate2d(angle + Context.Config.halfSegmentAngle) * (Context.Config.plateRadius - Context.Config.halfPanelWidth);
                var jointPoints = new[]
                {
                    plateSegmentGenerator.OuterSurfaceFarPoints.First().rotate2d(nextAngle),
                    plateSegmentGenerator.OuterBendDownCenterTouchPoint.rotate2d(nextAngle),
                    plateSegmentGenerator.OuterDownCornerPoint.rotate2d(nextAngle),
                    plateSegmentGenerator.OuterUpCornerPoint.rotate2d(angle),
                    plateSegmentGenerator.OuterBendUpCenterTouchPoint.rotate2d(angle),
                    plateSegmentGenerator.OuterSurfaceFarPoints.Last().rotate2d(angle)
                };

                // outer segment joint
                for (var idx = 0; idx < jointPoints.Count() - 1; idx++)
                {
                    Intermediate.counterClockwiseSingleSidedFromFront(
                        jointCenterPoint.to3d(),
                        jointPoints[idx].to3d(),
                        jointPoints[idx + 1].to3d()
                       );
                }

                var theta0 = plateSegmentGenerator.OuterSurfaceNearPoints[0].toPolar(Vector2d.Zero).Theta;
                var theta1 = plateSegmentGenerator.OuterSurfaceNearPoints[1].toPolar(Vector2d.Zero).Theta;
                var surfaceSegmentAngle = theta1 - theta0;

                var farTheta0 = plateSegmentGenerator.OuterSurfaceNearPoints.Last().rotate2d(angle).toPolar(Vector2d.Zero).Theta;
                var farTheta1 = plateSegmentGenerator.OuterSurfaceNearPoints.First().rotate2d(nextAngle).toPolar(Vector2d.Zero).Theta;
                var farJointAngle = farTheta1 - farTheta0;
                if (farJointAngle < 0)
                    farJointAngle += 2 * Math.PI;

                var farSegmentPoints = Generators.counterClockwiseCircleSegments(
                    (int)Math.Ceiling(farJointAngle / surfaceSegmentAngle),
                    Vector2d.Zero,
                    plateSegmentGenerator.OuterSurfaceFarPoints.Last().rotate2d(angle),
                    plateSegmentGenerator.OuterSurfaceFarPoints.First().rotate2d(nextAngle)
                    );

                for (var idx = 0; idx < farSegmentPoints.Count() - 1; idx++)
                {
                    Intermediate.counterClockwiseSingleSidedFromFront(
                        jointCenterPoint.to3d(),
                        farSegmentPoints[idx].to3d(),
                        farSegmentPoints[idx + 1].to3d()
                       );
                }

                OuterJointPoints.Add(farSegmentPoints);
            }

            return Intermediate;
        }
    }
}
