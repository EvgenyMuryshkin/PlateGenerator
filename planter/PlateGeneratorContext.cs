using System.Collections.Generic;
using System.Linq;

namespace planter
{
    public class PlateGeneratorContext
    {
        public PlateConfig Config { get; set; }
        public PlateSegment2dGenerator Plate2d { get; set; }
        public Surface2dGenerator Surface2d { get; set; }
        public IEnumerable<int> SegmentIndexes => Enumerable.Range(0, Config.segments);

        public double halfPlateHeight => Config.plateHeight / 2;
    }
}
