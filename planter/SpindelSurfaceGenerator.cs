using g3;
using System.Collections.Generic;
using System.Linq;

namespace planter
{
    public class SpindelSurfaceGenerator
    {
        public IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        public PlateGeneratorContext Context;

        public SpindelSurfaceGenerator(PlateGeneratorContext context)
        {
            Context = context;
        }

        public IntermediateMeshGenerator Generate()
        {
            var plateSegmentGenerator = Context.Plate2d;

            var cornerPoints = Context
                .SegmentIndexes
                .Select(idx => plateSegmentGenerator.InnerBendCorner.rotate2d(Context.Config.SegmentAngle(idx)))
                .ToList();

            if (Context.Config.spindelRadius == 0)
            {
                for (var idx = 0; idx < cornerPoints.Count; idx++)
                {
                    var nextIdx = (idx + 1) % cornerPoints.Count;

                    Intermediate.counterClockwiseSingleSidedFromFront(
                        Vector2d.Zero.to3d(Context.halfPlateHeight),
                        cornerPoints[idx].to3d(Context.halfPlateHeight),
                        cornerPoints[nextIdx].to3d(Context.halfPlateHeight)
                       );

                    Intermediate.counterClockwiseSingleSidedFromFront(
                        Vector2d.Zero.to3d(-Context.halfPlateHeight),
                        cornerPoints[nextIdx].to3d(-Context.halfPlateHeight),
                        cornerPoints[idx].to3d(-Context.halfPlateHeight)
                       );
                }

                return Intermediate;
            }

            List<Vector2d> spindelPoints = new List<Vector2d>();

            if (Context.Config.spindelCut == 0 )
            {
                spindelPoints = Generators.counterClockwiseCirclePoints(
                    Context.Config.spindelPoints,
                    Vector2d.Zero,
                    Context.Config.spindelRadius
                    );
                /*
                Intermediate.Extrude(spindelPoints, Context.Config.plateHeight, true);

                var nearestDistanceIndexes = cornerPoints.Select(point =>
                {
                    var distances = spindelPoints.Select(p => p.Distance(point)).ToList();
                    var nearestPointIndex = distances.FindIndex(d => d == distances.Min());
                    return nearestPointIndex;
                }).ToList();

                for (var idx = 0; idx < cornerPoints.Count; idx++)
                {
                    var nearestPointIndex = nearestDistanceIndexes[idx];

                    var nextIdx = (idx + 1) % cornerPoints.Count;
                    var nextNearestPointIndex = nearestDistanceIndexes[nextIdx];

                    var polygonPoints = new List<Vector2d>();
                    polygonPoints.AddRange(
                        new []
                        {
                            cornerPoints[idx],
                            cornerPoints[nextIdx]
                        });

                    var spindelPart = new List<Vector2d>();
                    var pointsToTake = nextNearestPointIndex < nearestPointIndex
                        ? nextNearestPointIndex + spindelPoints.Count - nearestPointIndex + 1
                        : nextNearestPointIndex - nearestPointIndex + 1;

                    spindelPart = Enumerable.Range(nearestPointIndex, pointsToTake)
                        .Select(i => spindelPoints[i % spindelPoints.Count])
                        .Reverse()
                        .ToList();

                    polygonPoints.AddRange(spindelPart);

                    var polygonizer = new TriangulatedPolygonGenerator()
                    {
                        Polygon = new GeneralPolygon2d( new Polygon2d(polygonPoints) )
                    };
                    polygonizer.Generate();

                    var polygonMesh = new IntermediateMeshGenerator();
                    polygonMesh.Append(polygonizer);

                    Intermediate.Append(new IntermediateMeshGenerator().Append(polygonMesh).Offset(new Vector3d(0, 0, Context.halfPlateHeight)));
                    Intermediate.Append(new IntermediateMeshGenerator().Append(polygonMesh).Offset(new Vector3d(0, 0, -Context.halfPlateHeight)).ReverseTriagles());
                }
                */
            }
            else
            {
                var spindelCutX = Context.Config.spindelRadius - Context.Config.spindelCut;
                var cutItx = Intersector.CircelLineIntersection(
                    Vector2d.Zero,
                    Context.Config.spindelRadius,
                    new Vector2d(spindelCutX, 0),
                    new Vector2d(spindelCutX, 1)
                    );

                spindelPoints = Generators.counterClockwiseCircleSegments(
                    Context.Config.spindelPoints,
                    Vector2d.Zero,
                    cutItx.Single(p => p.y > 0),
                    cutItx.Single(p => p.y < 0)
                    );

                //Intermediate.Extrude(spindelSegments, Context.Config.plateHeight, true);
            }


            Intermediate.Extrude(spindelPoints, Context.Config.plateHeight, true);

            var nearestDistanceIndexes = cornerPoints.Select(point =>
            {
                var distances = spindelPoints.Select(p => p.Distance(point)).ToList();
                var nearestPointIndex = distances.FindIndex(d => d == distances.Min());
                return nearestPointIndex;
            }).ToList();

            for (var idx = 0; idx < cornerPoints.Count; idx++)
            {
                var nearestPointIndex = nearestDistanceIndexes[idx];

                var nextIdx = (idx + 1) % cornerPoints.Count;
                var nextNearestPointIndex = nearestDistanceIndexes[nextIdx];

                var polygonPoints = new List<Vector2d>();
                polygonPoints.AddRange(
                    new[]
                    {
                            cornerPoints[idx],
                            cornerPoints[nextIdx]
                    });

                var spindelPart = new List<Vector2d>();
                var pointsToTake = nextNearestPointIndex < nearestPointIndex
                    ? nextNearestPointIndex + spindelPoints.Count - nearestPointIndex + 1
                    : nextNearestPointIndex - nearestPointIndex + 1;

                spindelPart = Enumerable.Range(nearestPointIndex, pointsToTake)
                    .Select(i => spindelPoints[i % spindelPoints.Count])
                    .Reverse()
                    .ToList();

                polygonPoints.AddRange(spindelPart);

                var polygonizer = new TriangulatedPolygonGenerator()
                {
                    Polygon = new GeneralPolygon2d(new Polygon2d(polygonPoints))
                };
                polygonizer.Generate();

                var polygonMesh = new IntermediateMeshGenerator();
                polygonMesh.Append(polygonizer);

                Intermediate.Append(new IntermediateMeshGenerator().Append(polygonMesh).Offset(new Vector3d(0, 0, Context.halfPlateHeight)));
                Intermediate.Append(new IntermediateMeshGenerator().Append(polygonMesh).Offset(new Vector3d(0, 0, -Context.halfPlateHeight)).ReverseTriagles());
            }

            return Intermediate;
        }
    }
}
