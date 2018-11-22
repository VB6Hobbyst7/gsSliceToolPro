﻿using System;
using System.Collections.Generic;
using System.IO;

using Gtk;
using GLib;
using g3;
using gs;
using gs.info;

namespace SliceViewer
{


	class MainClass
	{
		static string GPX_PATH = Util.IsRunningOnMono() ? "gpx" : "gpx.exe";

		public static Window MainWindow;
		public static SliceViewCanvas View;

		public static SingleMaterialFFFSettings LastSettings;


        public static bool SHOW_RELOADED_GCODE_PATHS = false;


		public static void Main(string[] args)
		{
			ExceptionManager.UnhandledException += delegate (UnhandledExceptionArgs expArgs) {
				Console.WriteLine(expArgs.ExceptionObject.ToString());
				expArgs.ExitApplication = true;
			};

			Gtk.Application.Init();

			MainWindow = new Window("gsSlicerViewer");
			MainWindow.SetDefaultSize(900, 600);
			MainWindow.SetPosition(WindowPosition.Center);
			MainWindow.DeleteEvent += delegate {
				Gtk.Application.Quit();
			};

            string sPath = "../../../sample_files/disc_single_layer.gcode";
            //string sPath = "../../../sample_files/disc_0p6mm.gcode";
            //string sPath = "../../../sample_files/square_linearfill.gcode";
            //string sPath = "../../../sample_files/thin_hex_test_part.gcode";
            //string sPath = "../../../sample_files/box_infill_50.gcode";
            //string sPath = "../../../sample_files/tube_adapter.gcode";
            //string sPath = "../../../sample_files/ring_2p2_makerbot.gcode";
            //string sPath = "/Users/rms/Desktop/print_experiment/cura_ring_2p2.gcode";
            //string sPath = "/Users/rms/Desktop/print_experiment/slic3r_ring_2p2.gcode";

            DMesh3 readMesh = null;

            //GCodeFile genGCode = MakerbotTests.SimpleFillTest();
            //GCodeFile genGCode = MakerbotTests.SimpleShellsTest();
            //GCodeFile genGCode = MakerbotTests.InfillBoxTest();

            //GeneralPolygon2d poly = GetPolygonFromMesh("../../../sample_files/bunny_open.obj");
            //GCodeFile genGCode = MakerbotTests.ShellsPolygonTest(poly);
            //GCodeFile genGCode = MakerbotTests.StackedPolygonTest(poly, 2);
            //GCodeFile genGCode = MakerbotTests.StackedScaledPolygonTest(poly, 20, 0.5);

            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bunny_solid_2p5cm.obj");
            readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bunny_solid_5cm_min.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/basic_step.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/slab_5deg.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/sphere_angles_1cm.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/inverted_cone_1.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/tube_adapter.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/tube_1.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/50x50x1_box.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/crop_bracket.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/thinwall2.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/box_and_cylsheet.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/box_and_opensheet.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/radial_fins.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/radial_fins_larger.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bunny_hollow_5cm.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/notch_test_1.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/variable_thins.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/arrow_posx.obj");
            //readMesh = StandardMeshReader.ReadMesh("c:\\scratch\\bunny_fixed_flat.obj");
            //MeshUtil.ScaleMesh(readMesh, Frame3f.Identity, 1.1f*Vector3f.One);

            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/unsupported_slab_5deg.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/overhang_slab_1.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/overhang_slab_1_tall.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/overhang_slab_1_taller.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/edge_overhang.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/support_tilted_cone.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/support_mintip.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/support_mintip_vtx.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/tilted_thin_slab.obj");
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/planar_clearance_test_1.obj");
            //readMesh = StandardMeshReader.ReadMesh("C:\\git\\gsGeometryTests\\test_output\\DELETEME_bunny.obj");

			//readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bridge_variable_circ.obj");
			readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bridge_variable_circ_6mm.stl");
			//readMesh = StandardMeshReader.ReadMesh("../../../sample_files/bridge_test_x.obj");

            //Vector3d basePt = readMesh.CachedBounds.Point(0, 0, -1);
            //MeshTransforms.Translate(readMesh, -basePt);

            DMesh3 cavityMesh = null;
            //cavityMesh = StandardMeshReader.ReadMesh("../../../sample_files/cylinder_cavity.obj");

            // interesting test case for clipselfoverlaps and scheduler
            //readMesh = StandardMeshReader.ReadMesh("../../../sample_files/Slim_Type1.stl");

            DMesh3 supportMesh = null;
            //supportMesh = StandardMeshReader.ReadMesh("../../../sample_files/edge_overhang_support.obj");
            //supportMesh = StandardMeshReader.ReadMesh("C:\\git\\gsGeometryTests\\test_output\\DELETEME_support.obj");

            // rotate to be z-up
            //MeshTransforms.Rotate(readMesh, Vector3d.Zero, Quaternionf.AxisAngleD(Vector3f.AxisX, 90));
            //if ( supportMesh != null )
            //    MeshTransforms.Rotate(supportMesh, Vector3d.Zero, Quaternionf.AxisAngleD(Vector3f.AxisX, 90));

            //readMesh = CalibrationModelGenerator.MakePrintStepSizeTest(10.0f, 10.0f, 0.1, 1.0, 10);

            //DMesh3[] meshComponents = MeshConnectedComponents.Separate(readMesh);
            DMesh3[] meshComponents = new DMesh3[] { readMesh };

            PrintMeshAssembly meshes = new PrintMeshAssembly();
            meshes.AddMeshes(meshComponents);

            if (cavityMesh != null)
                meshes.AddMesh(cavityMesh, PrintMeshOptions.Cavity());
            if (supportMesh != null)
                meshes.AddMesh(supportMesh, PrintMeshOptions.Support());

            AxisAlignedBox3d bounds = meshes.TotalBounds;
            AxisAlignedBox2d bounds2 = new AxisAlignedBox2d(bounds.Center.xy, bounds.Width / 2, bounds.Height / 2);

            View = new SliceViewCanvas();
            MainWindow.Add(View);

            if (readMesh != null) {
                // generate gcode file for mesh
                sPath = GenerateGCodeForMeshes(meshes);
            }

            if (SHOW_RELOADED_GCODE_PATHS) {
                LoadGeneratedGCodeFile(sPath);
            }

            //GenerateGCodeForSliceFile("c:\\scratch\\output.gslice");

            MainWindow.KeyReleaseEvent += Window_KeyReleaseEvent;

            // support drag-drop
            Gtk.TargetEntry[] target_table = new TargetEntry[] {
              new TargetEntry ("text/uri-list", 0, 0),
            };
            Gtk.Drag.DestSet(MainWindow, DestDefaults.All, target_table, Gdk.DragAction.Copy);
            MainWindow.DragDataReceived += MainWindow_DragDataReceived; ;


            MainWindow.ShowAll();

            Gtk.Application.Run();
        }




        static string GenerateGCodeForMeshes(PrintMeshAssembly meshes)
        {
			bool ENABLE_SUPPORT_ZSHIFT = true;

			// configure settings
			MakerbotSettings settings = new MakerbotSettings(Makerbot.Models.Replicator2);
			//MonopriceSettings settings = new MonopriceSettings(Monoprice.Models.MP_Select_Mini_V2);
			//PrintrbotSettings settings = new PrintrbotSettings(Printrbot.Models.Plus);
			settings.ExtruderTempC = 200;
            settings.Shells = 2;
            settings.InteriorSolidRegionShells = 0;
            settings.SparseLinearInfillStepX = 10;
            settings.ClipSelfOverlaps = false;
            //settings.RoofLayers = settings.FloorLayers = 0;
            //settings.LayerRangeFilter = new Interval1i(245, 255);
            settings.LayerRangeFilter = new Interval1i(6, 40);

            settings.GenerateSupport = false;
            settings.EnableSupportShell = true;
            settings.SupportSolidSpace = 0.35;

			//settings.Machine.NozzleDiamMM = 0.75;
			//settings.Machine.MaxLayerHeightMM = 0.5;
			//settings.FillPathSpacingMM = settings.Machine.NozzleDiamMM;
			//settings.LayerHeightMM = 0.5;

			//settings.LayerRangeFilter = new Interval1i(130, 140);

            LastSettings = settings.CloneAs<SingleMaterialFFFSettings>();

            System.Console.WriteLine("Slicing...");

            // slice meshes
            MeshPlanarSlicerPro slicer = new MeshPlanarSlicerPro() {
                LayerHeightMM = settings.LayerHeightMM,
                SliceFactoryF = PlanarSlicePro.FactoryF
            };
            slicer.Add(meshes);
            PlanarSliceStack slices = slicer.Compute();

            System.Console.WriteLine("Generating GCode...");

            // run print generator
			SingleMaterialFFFPrintGenPro printGen =
				new SingleMaterialFFFPrintGenPro(meshes, slices, settings);

			if (ENABLE_SUPPORT_ZSHIFT)
                printGen.LayerPostProcessor = new SupportConnectionPostProcessor() { ZOffsetMM = 0.2f };
            printGen.AccumulatePathSet = (SHOW_RELOADED_GCODE_PATHS == false);

            printGen.Generate();
            GCodeFile genGCode = printGen.Result;

            System.Console.WriteLine("Writing GCode...");

            string sWritePath = "../../../sample_output/generated.gcode";
            StandardGCodeWriter writer = new StandardGCodeWriter();
            using (StreamWriter w = new StreamWriter(sWritePath)) {
                writer.WriteFile(genGCode, w);
            }

            if (settings is MakerbotSettings) {
                System.Diagnostics.Process.Start(GPX_PATH, "-p " + sWritePath);
            }

            if ( SHOW_RELOADED_GCODE_PATHS == false) {
                View.SetPaths(printGen.AccumulatedPaths, settings);
            }

            return sWritePath;
        }





        static void GenerateGCodeForSliceFile(string sliceFile)
        {
            PlanarSliceStack slices = new PlanarSliceStack();
            using (TextReader reader = new StreamReader(sliceFile)) {
                slices.ReadSimpleSliceFormat(reader);
            }

            // configure settings
            MakerbotSettings settings = new MakerbotSettings(Makerbot.Models.Replicator2);
            //MonopriceSettings settings = new MonopriceSettings(Monoprice.Models.MP_Select_Mini_V2);
            //PrintrbotSettings settings = new PrintrbotSettings(Printrbot.Models.Plus);
            settings.Shells = 2;
            settings.SparseLinearInfillStepX = 10;
            settings.InteriorSolidRegionShells = 1;
            settings.ClipSelfOverlaps = true;
            settings.GenerateSupport = true;
            settings.SupportSpacingStepX = 5.0;
            settings.SupportVolumeScale = 1.0;

            //settings.LayerRangeFilter = new Interval1i(0,10);


            LastSettings = settings.CloneAs<SingleMaterialFFFSettings>();

            // empty...
            PrintMeshAssembly meshes = new PrintMeshAssembly();

            // run print generator
            SingleMaterialFFFPrintGenerator printGen =
                new SingleMaterialFFFPrintGenerator(meshes, slices, settings);

            printGen.LayerPostProcessor = new SupportConnectionPostProcessor() { ZOffsetMM = 0.15f };
            printGen.AccumulatePathSet = (SHOW_RELOADED_GCODE_PATHS == false);

            printGen.Generate();
            GCodeFile genGCode = printGen.Result;

            string sWritePath = "../../../sample_output/generated.gcode";
            StandardGCodeWriter writer = new StandardGCodeWriter();
            using (StreamWriter w = new StreamWriter(sWritePath)) {
                writer.WriteFile(genGCode, w);
            }

            if ( settings is MakerbotSettings ) {
                System.Diagnostics.Process.Start(GPX_PATH, "-p " + sWritePath);
            }

            if (SHOW_RELOADED_GCODE_PATHS) {
                LoadGeneratedGCodeFile(sWritePath);
            } else {
                View.SetPaths(printGen.AccumulatedPaths, settings);
            }
        }









        static void LoadGCodeFile(string sPath) {
			GenericGCodeParser parser = new GenericGCodeParser();
			GCodeFile gcode;
			using (FileStream fs = new FileStream(sPath, FileMode.Open, FileAccess.Read)) {
				using (TextReader reader = new StreamReader(fs)) {
					gcode = parser.Parse(reader);
				}
			}

			GCodeToToolpaths converter = new GCodeToToolpaths();
			MakerbotInterpreter interpreter = new MakerbotInterpreter();
			interpreter.AddListener(converter);
			InterpretArgs interpArgs = new InterpretArgs();
			interpreter.Interpret(gcode, interpArgs);

			ToolpathSet Paths = converter.PathSet;
			View.SetPaths(Paths);		
		}





        static void LoadGeneratedGCodeFile(string sPath)
        {
            // read gcode file
            GenericGCodeParser parser = new GenericGCodeParser();
            GCodeFile gcode;
            using (FileStream fs = new FileStream(sPath, FileMode.Open, FileAccess.Read)) {
                using (TextReader reader = new StreamReader(fs)) {
                    gcode = parser.Parse(reader);
                }
            }

            // write back out gcode we loaded
            //StandardGCodeWriter writer = new StandardGCodeWriter();
            //using ( StreamWriter w = new StreamWriter("../../../sample_output/writeback.gcode") ) {
            //	writer.WriteFile(gcode, w);
            //}

            GCodeToToolpaths converter = new GCodeToToolpaths();
            MakerbotInterpreter interpreter = new MakerbotInterpreter();
            interpreter.AddListener(converter);

            InterpretArgs interpArgs = new InterpretArgs();
            interpreter.Interpret(gcode, interpArgs);

            View.SetPaths(converter.PathSet);
            if (LastSettings != null)
                View.PathDiameterMM = (float)LastSettings.Machine.NozzleDiamMM;
        }







        static DMesh3 GenerateTubeMeshesForGCode(string sPath)
        {
            GenericGCodeParser parser = new GenericGCodeParser();
            GCodeFile gcode;
            using (FileStream fs = new FileStream(sPath, FileMode.Open, FileAccess.Read)) {
                using (TextReader reader = new StreamReader(fs)) {
                    gcode = parser.Parse(reader);
                }
            }
            GCodeToLayerTubeMeshes make_tubes = new GCodeToLayerTubeMeshes() {
                TubeProfile = Polygon2d.MakeCircle(0.2f, 12)
            };
            MakerbotInterpreter interpreter = new MakerbotInterpreter();
            interpreter.AddListener(make_tubes);
            interpreter.Interpret(gcode, new InterpretArgs());
            DMesh3 tubeMesh2 = make_tubes.GetCombinedMesh(1);
            return tubeMesh2;
        }







		void OnException(object o, UnhandledExceptionArgs args)
		{

		}


		private static void Window_KeyReleaseEvent(object sender, KeyReleaseEventArgs args)
		{
			if (args.Event.Key == Gdk.Key.Up) {
				if ((args.Event.State & Gdk.ModifierType.ShiftMask) != 0)
					View.CurrentLayer = View.CurrentLayer + 10;
				else
					View.CurrentLayer = View.CurrentLayer + 1;
			} else if (args.Event.Key == Gdk.Key.Down) {
				if ((args.Event.State & Gdk.ModifierType.ShiftMask) != 0)
					View.CurrentLayer = View.CurrentLayer - 10;
				else
					View.CurrentLayer = View.CurrentLayer - 1;

			} else if (args.Event.Key == Gdk.Key.n) {
				if (View.NumberMode == SliceViewCanvas.NumberModes.NoNumbers)
					View.NumberMode = SliceViewCanvas.NumberModes.PathNumbers;
				else
					View.NumberMode = SliceViewCanvas.NumberModes.NoNumbers;
				View.QueueDraw();

			} else if (args.Event.Key == Gdk.Key.f) {
				View.ShowFillArea = !View.ShowFillArea;
				View.QueueDraw();

			} else if (args.Event.Key == Gdk.Key.t) {
				View.ShowTravels = !View.ShowTravels;
				View.QueueDraw();

			} else if (args.Event.Key == Gdk.Key.e) {
				View.ShowDepositMoves = !View.ShowDepositMoves;
				View.QueueDraw();

            } else if (args.Event.Key == Gdk.Key.p) {
                View.ShowAllPathPoints = !View.ShowAllPathPoints;
                View.QueueDraw();

            } else if (args.Event.Key == Gdk.Key.b) {
				View.ShowBelowLayer = !View.ShowBelowLayer;
				View.QueueDraw();

            } else if (args.Event.Key == Gdk.Key.i) {
                View.ShowIssues = !View.ShowIssues;
                View.QueueDraw();

            } else if ( args.Event.Key == Gdk.Key.q ) {
                //SliceViewerTests.TestDGraph2();
                SliceViewerTests.TestFill();
                //SliceViewerTests.TestOffset();

            } else if ( args.Event.Key == Gdk.Key.E ) {
                List<PolyLine2d> paths = View.GetPolylinesForLayer(View.CurrentLayer);
                SVGWriter writer = new SVGWriter();
                SVGWriter.Style lineStyle = SVGWriter.Style.Outline("black", 0.2f);
                foreach (var p in paths)
                    writer.AddPolyline(p, lineStyle);
                writer.Write("c:\\scratch\\__LAST_PATHS.svg");

            }
		}






		static void MainWindow_DragDataReceived(object o, DragDataReceivedArgs args)
		{
			string data = System.Text.Encoding.UTF8.GetString(args.SelectionData.Data);
			data = data.Trim('\r', '\n', '\0');
			if (Util.IsRunningOnMono()) {
				data = data.Replace("file://", "");
			} else {
				data = data.Replace("file:///", "");
			}
			data = data.Replace("%20", " ");        // gtk inserts these for spaces? maybe? wtf.
			try {
                if (data.EndsWith(".gslice")) {
                    GenerateGCodeForSliceFile(data);
                } else {
                    LoadGCodeFile(data);
                }
			} catch (Exception e) {
				using (var dialog = new Gtk.MessageDialog(MainWindow, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok,
					"Exception loading {0} : {1}", data, e.Message)) {
					dialog.Show();
				}
			}
		}





		static GeneralPolygon2d GetPolygonFromMesh(string sPath) {
			DMesh3 mesh = StandardMeshReader.ReadMesh(sPath);
			MeshBoundaryLoops loops = new MeshBoundaryLoops(mesh);

			PlanarComplex complex = new PlanarComplex();

			foreach (var loop in loops ) {
				Polygon2d poly = new Polygon2d();
				DCurve3 curve = MeshUtil.ExtractLoopV(mesh, loop.Vertices);
				foreach (Vector3d v in curve.Vertices)
					poly.AppendVertex(v.xy);
				complex.Add(poly);
			}

			PlanarComplex.SolidRegionInfo solids = complex.FindSolidRegions(0.0, false);
			return solids.Polygons[0];
		}



	}
}
