using g3;
using System;
using System.Collections.Generic;
using System.Linq;

namespace planter
{
    public class IntermediateMeshGenerator
    {
        public List<Vector3d> Vertices = new List<Vector3d>();
        public List<Vector3f> Normals = new List<Vector3f>();
        public List<Index3i> Triangles = new List<Index3i>();

        public IntermediateMeshGenerator counterClockwiseSingleSidedFromFront(Vector3d v1, Vector3d v2, Vector3d v3)
        {
            var normal = Vector3f.Zero;

            var idx = Vertices.Count;
            Vertices.AddRange(new[] {
                    v1,
                    v2,
                    v3,
                });

            Normals.AddRange(new[] { normal, normal, normal });

            Triangles.Add(new Index3i(idx, idx + 1, idx + 2));

            return this;
        }

        public IntermediateMeshGenerator Append(IntermediateMeshGenerator intermediate)
        {
            var count = Vertices.Count;

            Vertices.AddRange(intermediate.Vertices.Select(v => v.Copy()));
            Normals.AddRange(intermediate.Normals.Select(n => n.Copy()));
            Triangles.AddRange(intermediate.Triangles.Select(t => new Index3i(t.a + count, t.b + count, t.c + count)));

            return this;
        }

        public IntermediateMeshGenerator ReverseTriagles()
        {
            Triangles = Triangles.Select(t => new Index3i(t.a, t.c, t.b)).ToList();

            return this;
        }

        public IntermediateMeshGenerator Extrude(
            List<Vector2d> points, 
            double height,
            bool closed = false
            )
        {
            for (var idx = 0; idx < points.Count - (closed ? 0: 1); idx++)
            {
                var nextIdx = (idx + 1) % points.Count;

                counterClockwiseSingleSidedFromFront(
                    points[idx].to3d(height / 2),
                    points[nextIdx].to3d(height / 2),
                    points[nextIdx].to3d(-height / 2)
                    );

                counterClockwiseSingleSidedFromFront(
                    points[idx].to3d(height / 2),
                    points[nextIdx].to3d(-height / 2),
                    points[idx].to3d(-height / 2)
                    );
            }

            return this;
        }

        public IntermediateMeshGenerator Offset(Vector3d offset)
        {
            for (int vertIdx = 0; vertIdx < Vertices.Count; vertIdx++)
            {
                Vertices[vertIdx] += offset;
            }

            return this;
        }

        public IntermediateMeshGenerator Rotate(double angle)
        {
            for (var idx = 0; idx < Vertices.Count; idx++)
            {
                var tmp = Vertices[idx];
                Vertices[idx] = new Vector2d(tmp.x, tmp.y).rotate2d(angle).to3d(tmp.z);
            }

            return this;
        }

        public IntermediateMeshGenerator Append(MeshGenerator polygonizer)
        {
            var count = Vertices.Count;

            Vertices.AddRange(
                Enumerable
                    .Range(0, polygonizer.vertices.Count)
                    .Select(i => polygonizer.vertices[i]));

            Normals.AddRange(
                Enumerable
                    .Range(0, polygonizer.normals.Count)
                    .Select(i => polygonizer.normals[i]));

            Triangles.AddRange(
                Enumerable
                    .Range(0, polygonizer.triangles.Count)
                    .Select(i =>
                    {
                        var t = polygonizer.triangles[i];
                        return new Index3i(t.a + count, t.b + count, t.c + count);
                    }));

            return this;
        }
    }
}
