using g3;
using System.Linq;

namespace planter
{
    public class OuterFarSurfaceGenerator
    {
        public IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        public PlateGeneratorContext Context;

        public OuterFarSurfaceGenerator(PlateGeneratorContext context)
        {
            Context = context;
        }

        public IntermediateMeshGenerator Generate()
        {
            var plateSegmentGenerator = Context.Plate2d;
            var farPoints = plateSegmentGenerator.OuterSurfaceFarPoints;

            for (int segIdx = 0; segIdx < Context.Config.segments; segIdx++)
            {
                var angle = Context.Config.SegmentAngle(segIdx);
                var topOuterPoints = farPoints.Select(p => p.rotate2d(angle).to3d(Context.Config.plateHeight / 2)).ToList();
                var bottomOuterPoints = farPoints.Select(p => p.rotate2d(angle).to3d(-Context.Config.plateHeight / 2)).ToList();

                for (int idx = 0; idx < farPoints.Count - 1; idx++)
                {
                    var thisIdx = idx;
                    var nextIdx = (idx + 1) % farPoints.Count;
                    Intermediate.counterClockwiseSingleSidedFromFront(
                        topOuterPoints[thisIdx], 
                        bottomOuterPoints[nextIdx], 
                        topOuterPoints[nextIdx]);

                    Intermediate.counterClockwiseSingleSidedFromFront(
                        topOuterPoints[thisIdx], 
                        bottomOuterPoints[thisIdx], 
                        bottomOuterPoints[nextIdx]);
                }
            }

            foreach (var set in Context.Surface2d.OuterJointPoints)
            {
                var reversed = set.ToList();
                reversed.Reverse();
                Intermediate.Extrude(reversed, Context.Config.plateHeight);
            }

            return Intermediate;
        }
    }
}
