using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace planter
{
    public class PlateSegment2dGenerator : IntermediateMeshGenerator
    {
        public IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        public PlateGeneratorContext Context;

        public Vector2d InnerBendCorner, InnerBendUpTouchPoint, InnerBendDownTouchPoint;
        public List<Vector2d> InnerBendPoints;

        public Vector2d OuterUpCornerPoint, OuterDownCornerPoint;
        public Vector2d OuterBendUpTouchPoint, OuterBendUpCenterTouchPoint;
        public List<Vector2d> OuterUpBendPoints;

        public Vector2d OuterBendDownTouchPoint, OuterBendDownCenterTouchPoint;
        public List<Vector2d> OuterDownBendPoints;

        public List<Vector2d> OuterSurfaceNearPoints, OuterSurfaceFarPoints;

        public PlateSegment2dGenerator(PlateGeneratorContext context)
        {
            Context = context;
        }

        public IntermediateMeshGenerator Generate()
        {
            var crossingX = Context.Config.halfPanelWidth / Context.Config.halfSegmentSin;
            var innerBendX = crossingX + Context.Config.bendRadius / Context.Config.halfSegmentSin;

            var innerBendCenter = new Vector2d(innerBendX, 0);

            InnerBendCorner = new Vector2d(crossingX, 0);

            var rot = Vector2d.AxisX.rotate2d(Context.Config.halfSegmentAngle);

            var distToIntersection = Context.Config.bendRadius / Context.Config.halfSegmentSin * Context.Config.halfSegmentCos;
            InnerBendUpTouchPoint = InnerBendCorner + rot * distToIntersection;
            InnerBendDownTouchPoint = new Vector2d(InnerBendUpTouchPoint.x, -InnerBendUpTouchPoint.y);

            InnerBendPoints = Generators.counterClockwiseCircleSegments(Context.Config.pointsInBend, innerBendCenter, InnerBendUpTouchPoint, InnerBendDownTouchPoint);

            for (int i = 0; i < InnerBendPoints.Count - 1; i++)
            {
                Intermediate.counterClockwiseSingleSidedFromFront(
                    InnerBendCorner.to3d(),
                    InnerBendPoints[i + 1].to3d(),
                    InnerBendPoints[i].to3d()
                   );
            }

            // outer corners
            var itxUp = Intersector.CircelLineIntersection(Vector2d.Zero, Context.Config.plateRadius - Context.Config.outerWidth, InnerBendCorner, InnerBendCorner + Vector2d.AxisX.rotate2d(Context.Config.halfSegmentAngle));
            OuterUpCornerPoint = itxUp.Single(p => p.y > 0);

            var itxDown = Intersector.CircelLineIntersection(Vector2d.Zero, Context.Config.plateRadius - Context.Config.outerWidth, InnerBendCorner, InnerBendCorner + Vector2d.AxisX.rotate2d(-Context.Config.halfSegmentAngle));
            OuterDownCornerPoint = itxDown.Single(p => p.y < 0);

            // outer bend
            var outerWidth = new Vector2d(Context.Config.plateRadius - Context.Config.outerWidth, 0);

            var outerBendCenter = new Vector2d(Context.Config.plateRadius - Context.Config.outerWidth - Context.Config.bendRadius, 0);

            // outer up bend
            var outerBendUpRay = Vector2d.AxisX.rotate2d(Context.Config.halfSegmentAngle);
            var outerBendUpCenter = Intersector.CircelLineIntersection(Vector2d.Zero, outerBendCenter.x, innerBendCenter, innerBendCenter + outerBendUpRay).Single(p => p.y > 0);
            var outerBendUpCenterRelToInnerBendCorner = outerBendUpCenter - InnerBendCorner;

            OuterBendUpCenterTouchPoint = Intersector.CircelLineIntersection(Vector2d.Zero, outerWidth.x, Vector2d.Zero, outerBendUpCenter).Single(p => p.y > 0);
            OuterBendUpTouchPoint = InnerBendCorner + outerBendUpRay * Math.Sqrt(outerBendUpCenterRelToInnerBendCorner.Length * outerBendUpCenterRelToInnerBendCorner.Length - Context.Config.bendRadius * Context.Config.bendRadius);
            OuterUpBendPoints = Generators.counterClockwiseCircleSegments(Context.Config.pointsInBend, outerBendUpCenter, OuterBendUpCenterTouchPoint, OuterBendUpTouchPoint);

            for (int i = 0; i < OuterUpBendPoints.Count - 1; i++)
            {
                Intermediate.counterClockwiseSingleSidedFromFront(
                    OuterUpCornerPoint.to3d(),
                    OuterUpBendPoints[i + 1].to3d(),
                    OuterUpBendPoints[i].to3d()
                    );
            }

            // outer down bend
            var outerBendDownRay = Vector2d.AxisX.rotate2d(-Context.Config.halfSegmentAngle);
            var outerBendDownCenter = Intersector.CircelLineIntersection(Vector2d.Zero, outerBendCenter.x, innerBendCenter, innerBendCenter + outerBendDownRay).Single(p => p.y < 0);
            var outerBendDownCenterRelToInnerBendCorner = outerBendDownCenter - InnerBendCorner;

            OuterBendDownCenterTouchPoint = Intersector.CircelLineIntersection(Vector2d.Zero, outerWidth.x, Vector2d.Zero, outerBendDownCenter).Single(p => p.y < 0);
            OuterBendDownTouchPoint = InnerBendCorner + outerBendDownRay * Math.Sqrt(outerBendDownCenterRelToInnerBendCorner.Length * outerBendDownCenterRelToInnerBendCorner.Length - Context.Config.bendRadius * Context.Config.bendRadius);
            OuterDownBendPoints = Generators.counterClockwiseCircleSegments(Context.Config.pointsInBend, outerBendDownCenter, OuterBendDownTouchPoint, OuterBendDownCenterTouchPoint);

            for (int i = 0; i < OuterDownBendPoints.Count - 1; i++)
            {
                Intermediate.counterClockwiseSingleSidedFromFront(
                    OuterDownCornerPoint.to3d(),
                    OuterDownBendPoints[i + 1].to3d(),
                    OuterDownBendPoints[i].to3d()
                    );
            }



            // outer serment surface
            OuterSurfaceNearPoints = Generators.counterClockwiseCircleSegments(
                Context.Config.segmentOuterSurfacePoints,
                Vector2d.Zero,
                OuterBendDownCenterTouchPoint,
                OuterBendUpCenterTouchPoint
                );

            var startPoint = Intersector.CircelLineIntersection(Vector2d.Zero, Context.Config.plateRadius, Vector2d.Zero, OuterBendDownCenterTouchPoint).Single(p => p.y < 0);
            var endPoint = Intersector.CircelLineIntersection(Vector2d.Zero, Context.Config.plateRadius, Vector2d.Zero, OuterBendUpCenterTouchPoint).Single(p => p.y > 0);

            OuterSurfaceFarPoints = Generators.counterClockwiseCircleSegments(
                Context.Config.segmentOuterSurfacePoints,
                Vector2d.Zero,
                startPoint,
                endPoint);

            return Intermediate;
        }
    }
}
