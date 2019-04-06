using g3;
using System.Collections.Generic;

namespace planter
{
    public class PlateSegment3dGenerator : IntermediateMeshGenerator
    {
        public IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        public PlateGeneratorContext Context;
        public Vector3d InnerBendTopCorner, InnerBendBottomCorner;
        public Vector3d OuterBendUpTopCorner, OuterBendUpBottomCorner;
        public Vector3d OuterBendDownTopCorner, OuterBendDownBottomCorner;

        public PlateSegment3dGenerator(PlateGeneratorContext context)
        {
            Context = context;
        }

        public IntermediateMeshGenerator Generate(int segIdx)
        {
            var angle = Context.Config.SegmentAngle(segIdx);

            var plateSegmentGenerator = Context.Plate2d;

            InnerBendTopCorner = plateSegmentGenerator.InnerBendCorner.rotate2d(angle).to3d(Context.Config.plateHeight / 2);
            OuterBendUpTopCorner = plateSegmentGenerator.OuterUpCornerPoint.rotate2d(angle).to3d(Context.Config.plateHeight / 2);
            OuterBendDownTopCorner = plateSegmentGenerator.OuterDownCornerPoint.rotate2d(angle).to3d(Context.Config.plateHeight / 2);

            InnerBendBottomCorner = plateSegmentGenerator.InnerBendCorner.rotate2d(angle).to3d(-Context.Config.plateHeight / 2);
            OuterBendUpBottomCorner = plateSegmentGenerator.OuterUpCornerPoint.rotate2d(angle).to3d(-Context.Config.plateHeight / 2);
            OuterBendDownBottomCorner = plateSegmentGenerator.OuterDownCornerPoint.rotate2d(angle).to3d(-Context.Config.plateHeight / 2);

            var topSeg = new IntermediateMeshGenerator();
            topSeg.Append(plateSegmentGenerator.Intermediate);
            topSeg.Offset(new Vector3d(0, 0, Context.Config.plateHeight / 2));
            topSeg.Rotate(angle);
            Intermediate.Append(topSeg);

            var bottomSeg = new IntermediateMeshGenerator();
            bottomSeg.Append(plateSegmentGenerator.Intermediate);
            bottomSeg.Offset(new Vector3d(0, 0, -Context.Config.plateHeight / 2));
            bottomSeg.Rotate(angle);
            bottomSeg.ReverseTriagles();
            Intermediate.Append(bottomSeg);

            // build inner surfaces for bends
            var sets = new[]
            {
                plateSegmentGenerator.InnerBendPoints,
                plateSegmentGenerator.OuterUpBendPoints,
                plateSegmentGenerator.OuterDownBendPoints
            };

            foreach (var set in sets)
            {
                var setMesh = new IntermediateMeshGenerator();
                setMesh.Extrude(set, Context.Config.plateHeight);
                setMesh.Rotate(angle);

                Intermediate.Append(setMesh);
            }

            // outer surface


            var outerSurfaceMesh = new IntermediateMeshGenerator();
            outerSurfaceMesh.Extrude(plateSegmentGenerator.OuterSurfaceNearPoints, Context.Config.plateHeight);
            outerSurfaceMesh.Rotate(angle);

            Intermediate.Append(outerSurfaceMesh);

            var upSurfaceMesh = new IntermediateMeshGenerator();
            upSurfaceMesh.Extrude(new List<Vector2d>() { plateSegmentGenerator.OuterBendUpTouchPoint, plateSegmentGenerator.InnerBendUpTouchPoint }, Context.Config.plateHeight);
            upSurfaceMesh.Rotate(angle);

            Intermediate.Append(upSurfaceMesh);

            var downSurfaceMesh = new IntermediateMeshGenerator();
            downSurfaceMesh.Extrude(new List<Vector2d>() { plateSegmentGenerator.InnerBendDownTouchPoint, plateSegmentGenerator.OuterBendDownTouchPoint }, Context.Config.plateHeight);
            downSurfaceMesh.Rotate(angle);

            Intermediate.Append(downSurfaceMesh);

            return Intermediate;
        }
    }
}
