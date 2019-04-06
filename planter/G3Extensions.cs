using g3;
using System;
using System.Collections.Generic;
using System.Text;

namespace planter
{
    public static class G3Extensions
    {
        public static Vector2d rotate2d(this Vector2d v, double angle)
        {
            double cosa = Math.Cos(angle), sina = Math.Sin(angle);

            return new Vector2d(
                v.x * cosa - v.y * sina,
                v.x * sina + v.y * cosa
                );
        }

        public static Vector3d Copy(this Vector3d s)
        {
            return new Vector3d(s.x, s.y, s.z);
        }

        public static Vector3f Copy(this Vector3f s)
        {
            return new Vector3f(s.x, s.y, s.z);
        }

        public static Vector3f to3f(this Vector3d s)
        {
            return new Vector3f(s.x, s.y, s.z);
        }

        public static Vector3d to3d(this Vector2d s, double z = 0)
        {
            return new Vector3d(s.x, s.y, z);
        }

        public static Vector2d to2dXY(this Vector3d s)
        {
            return new Vector2d(s.x, s.y);

        }
        public static Vector2d toCartesian(this Polar point)
        {
            return new Vector2d(
                point.R * Math.Cos(point.Theta),
                point.R * Math.Sin(point.Theta)
                );
        }

        public static Polar toPolar(this Vector2d center, Vector2d point)
        {
            polarSector sector = polarSector.None;
            if (point.x >= center.x)
            {
                if (point.y >= center.y)
                {
                    sector = polarSector.Q1;
                }
                else
                {
                    sector = polarSector.Q4;
                }
            }
            else
            {
                if (point.y >= center.y)
                {
                    sector = polarSector.Q2;
                }
                else
                {
                    sector = polarSector.Q3;
                }
            }

            var dx = point.x - center.x;
            var dy = point.y - center.y;
            var r = Math.Sqrt(dx * dx + dy * dy);
            var theta = Math.Atan(dy / dx);

            switch (sector)
            {
                case polarSector.Q2:
                case polarSector.Q3:
                    theta += Math.PI;
                    break;
                case polarSector.Q4:
                    theta += 2 * Math.PI;
                    break;
            }

            return new Polar()
            {
                R = r,
                Theta = theta
            };
        }
    }
}
