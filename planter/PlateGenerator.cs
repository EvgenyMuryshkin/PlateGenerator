using g3;
using System.Linq;

namespace planter
{
    public class PlateGenerator : MeshGenerator
    {
        IntermediateMeshGenerator Intermediate = new IntermediateMeshGenerator();
        PlateGeneratorContext Context = new PlateGeneratorContext();

        public PlateGenerator(PlateConfig config)
        {
            Context.Config = config;
        }

        void Complete(IntermediateMeshGenerator intermediate)
        {
            vertices = new VectorArray3d(intermediate.Vertices.Count);
            normals = new VectorArray3f(intermediate.Normals.Count);
            triangles = new IndexArray3i(intermediate.Triangles.Count);

            for (int idx = 0; idx < intermediate.Vertices.Count; idx++)
                vertices[idx] = intermediate.Vertices[idx];
            for (int idx = 0; idx < intermediate.Normals.Count; idx++)
                normals[idx] = intermediate.Normals[idx];
            for (int idx = 0; idx < intermediate.Triangles.Count; idx++)
                triangles[idx] = intermediate.Triangles[idx];
        }

        public override MeshGenerator Generate()
        {
            var plateSegmentGenerator = Context.Plate2d = new PlateSegment2dGenerator(Context);
            plateSegmentGenerator.Generate();

            var segments3d = Enumerable
                .Range(0, Context.Config.segments)
                .Select(segIdx =>
                {
                    var gen = new PlateSegment3dGenerator(Context);
                    return gen.Generate(segIdx);
                }).ToList();

            foreach (var seg in segments3d)
                Intermediate.Append(seg);

            Context.Surface2d = new Surface2dGenerator(Context);
            Context.Surface2d.Generate();

            Intermediate.Append(new IntermediateMeshGenerator().Append(Context.Surface2d.Intermediate).Offset(new Vector3d(0, 0, Context.Config.plateHeight / 2)));
            Intermediate.Append(new IntermediateMeshGenerator().Append(Context.Surface2d.Intermediate).Offset(new Vector3d(0, 0, -Context.Config.plateHeight / 2)).ReverseTriagles());

            // outer surface
            Intermediate.Append(new OuterFarSurfaceGenerator(Context).Generate());

            // spindel surface
            Intermediate.Append(new SpindelSurfaceGenerator(Context).Generate());

            // complete mesh
            Complete(Intermediate);

            return this;
        }
    }
}
