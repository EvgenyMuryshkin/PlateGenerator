using System;

namespace planter
{
    public class PlateConfig
    {
        public int segments = 6;
        public double panelWidth = 10;
        public double bendRadius = 5;
        public double outerWidth = 10;
        public double plateRadius = 150;
        public int pointsInBend = 16;
        public double plateHeight = 5;
        public int segmentOuterSurfacePoints = 32;

        public double halfPanelWidth => panelWidth / 2;
        public double segmentAngle => 2 * Math.PI / segments;
        public double halfSegmentAngle => segmentAngle / 2;

        public double halfSegmentCos => Math.Cos(halfSegmentAngle);
        public double halfSegmentSin => Math.Sin(halfSegmentAngle);

        public int pointsInOuterSurface => segmentOuterSurfacePoints * segments;

        public double SegmentAngle(int seg) => seg * segmentAngle;

        public int spindelPoints = 64;
        public double spindelRadius = 2.5;
        public double spindelCut = 0.5;
    }
}
