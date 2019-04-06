using g3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace planter
{
    [TestClass]
    public class TestGenerators
    {
        public static string SolutionFolder(string start = null)
        {
            if (start == null)
                start = Environment.CurrentDirectory;

            if (!Directory.Exists(start))
                return null;

            if (Directory.EnumerateFiles(start, "*.sln").Any())
                return start;

            return SolutionFolder(Path.GetDirectoryName(start));
        }

        void SaveMesh(PlateConfig config, [CallerMemberName]string fileName = null)
        {
            var output = Path.Combine(SolutionFolder(), "output");
            if (!Directory.Exists(output))
                Directory.CreateDirectory(output);

            var plate = new PlateGenerator(config);

            var meshGenerator = plate.Generate();
            var mesh = meshGenerator.MakeSimpleMesh();

            fileName = fileName != null ? $"{fileName}.stl" : $"output.stl";

            IOWriteResult result = StandardMeshWriter.WriteFile(
                Path.Combine(output, fileName),
                new List<WriteMesh>() { new WriteMesh(mesh) },
                WriteOptions.Defaults);
        }

        [TestMethod]
        public void SEG6xRAD75xSPD0()
        {
            var config = new PlateConfig()
            {
                plateRadius = 75,
                spindelRadius = 0
            };

            SaveMesh(config);
        }

        [TestMethod]
        public void SEG6xRAD75xSPD3xCUT0()
        {
            var config = new PlateConfig()
            {
                plateRadius = 75,
                spindelRadius = 3,
                spindelCut = 0,                
            };

            SaveMesh(config);
        }


        [TestMethod]
        public void SEG8xRAD90xBND10_SPD5_CUT1()
        {
            var config = new PlateConfig()
            {
                segments = 8,
                plateRadius = 90,
                bendRadius = 10,
                spindelRadius = 5,
                spindelCut = 1
            };

            SaveMesh(config);
        }

        [TestMethod]
        public void SEG5xRAD60xPWDT5_SPD3_CUT05()
        {
            var config = new PlateConfig()
            {
                segments = 5,
                plateRadius = 60,
                panelWidth = 5,
                spindelRadius = 3,
                spindelCut = 0.5
            };

            SaveMesh(config);
        }

        [TestMethod]
        public void SEG7xRAD90xBND10_SPD5_CUT1()
        {
            var config = new PlateConfig()
            {
                segments = 7,
                plateRadius = 90,
                bendRadius = 10,
                spindelRadius = 5,
                spindelCut = 1
            };

            SaveMesh(config);
        }
    }
}
