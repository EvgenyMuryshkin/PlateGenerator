# PlateGenerator
Generate base plates for 3D printing models with variuos parameters

You need to have Visual Studio or .NET Core 2.2 installed in order to run generator.

Configure and run unit test from Visual Studio or command line to produce STL with base plate.


To tun from command line, cd to planter folder and run 
```
dotnet test --filter SEG5xRAD60xPWDT5_SPD3_CUT05
```

This tests produces plate with 5 segments, 60mm radius, with spindel hole and a spindel cut.

```csharp
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
```

outout looks like this

![mesh](https://raw.githubusercontent.com/EvgenyMuryshkin/PlateGenerator/master/images/mesh.png "Mesh")

![render](https://raw.githubusercontent.com/EvgenyMuryshkin/PlateGenerator/master/images/render.png "Redner")



