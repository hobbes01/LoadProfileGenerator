﻿using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace ReleaseMaker {
    public class WinLpgCopier:CopierBase {
        [NotNull]
        public static List<string> CopyLpgFiles([NotNull] string src, [NotNull] string dst)
        {
            List<string> programFiles = new List<string>();
            var srcDi = new DirectoryInfo(src);
            Copy(programFiles, srcDi, src, dst,"Autofac.dll");
            Copy(programFiles, srcDi, src, dst,"Autofac.dll");
            Copy(programFiles, srcDi, src, dst,"Automation.dll");
            Copy(programFiles, srcDi, src, dst,"CalcPostProcessor.dll");
            Copy(programFiles, srcDi, src, dst,"CalculationController.dll");
            Copy(programFiles, srcDi, src, dst,"CalculationEngine.dll");
            Copy(programFiles, srcDi, src, dst,"ChartCreator2.dll");
            //Copy(programFiles, srcDi, src, dst,"ChartPDFCreator.dll");
            Copy(programFiles, srcDi, src, dst,"Common.dll");
            Copy(programFiles, srcDi, src, dst,"Database.dll");
            //Copy(programFiles, srcDi, src, dst,"SimulationEngineLib.dll");
            Copy(programFiles, srcDi, src, dst,"System.Buffers.dll");
            Copy(programFiles, srcDi, src, dst,"System.Memory.dll");
            Copy(programFiles, srcDi, src, dst,"System.Numerics.Vectors.dll");
            Copy(programFiles, srcDi, src, dst,"System.Resources.Extensions.dll");
            Copy(programFiles, srcDi, src, dst,"System.Runtime.CompilerServices.Unsafe.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.DocumentObjectModel-wpf.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.DocumentObjectModel.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.Rendering-wpf.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.Rendering.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.RtfRendering.dll");

            //Copy(desrc, dst, "MigraDoc.DocumentObjectModel.resources.dll");
            //
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.DocumentObjectModel-gdi.dll");
            //string desrc = Path.Combine(src, "de");
            //Copy(programFiles, desrc, dst, "MigraDoc.DocumentObjectModel-gdi.resources.dll");
            //Copy(programFiles, desrc, dst, "MigraDoc.Rendering-gdi.resources.dll");
            //Copy(programFiles, desrc, dst, "MigraDoc.RtfRendering-gdi.resources.dll");
            //Copy(programFiles, desrc, dst, "PdfSharp-gdi.resources.dll");
            //Copy(programFiles, desrc, dst, "PdfSharp.Charting-gdi.resources.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.Rendering-gdi.dll");
            //Copy(programFiles, srcDi, src, dst,"MigraDoc.RtfRendering-gdi.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp-gdi.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp.Charting-gdi.dll");
            //Copy(desrc, dst, "PdfSharp.Charting.resources.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp.Charting.dll");
            //Copy(desrc, dst, "PdfSharp.resources.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp.dll");
            Copy(programFiles, srcDi, src, dst,"Newtonsoft.Json.dll");
            Copy(programFiles, srcDi, src, dst,"OxyPlot.dll");
            //Copy(programFiles, srcDi, src, dst,"OxyPlot.Pdf.dll");
            Copy(programFiles, srcDi, src, dst,"OxyPlot.Wpf.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp-wpf.dll");
            //Copy(programFiles, srcDi, src, dst,"PdfSharp.Charting-wpf.dll");
            //Copy(programFiles, srcDi, src, dst,"SettlementProcessing.dll");
            Copy(programFiles, srcDi, src, dst,"SQLite.Interop.dll");
            //Copy(programFiles, srcDi, src, dst,"sqlite3.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.SQLite.dll");
            Copy(programFiles, srcDi, src, dst,"LoadProfileGenerator.exe");
            Copy(programFiles, srcDi, src, dst,"LoadProfileGenerator.exe.config");
            //Copy(programFiles, srcDi, src, dst,"SimulationEngine.exe");
            //Copy(programFiles, srcDi, src, dst,"PowerArgs.dll");
            Copy(programFiles, srcDi, src, dst,"EntityFramework.dll");
            Copy(programFiles, srcDi, src, dst,"EntityFramework.SqlServer.dll");
            Copy(programFiles, srcDi, src, dst,"JetBrains.Annotations.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.SQLite.EF6.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.SQLite.Linq.dll");
            Copy(programFiles, srcDi, src, dst,"Microsoft.Bcl.AsyncInterfaces.dll");
            //Copy(programFiles, srcDi, src, dst,"Humanizer.dll");
            Copy(programFiles, srcDi, src, dst,"HarfBuzzSharp.dll");
            Copy(programFiles, srcDi, src, dst,"libHarfBuzzSharp.dll");
            Copy(programFiles, srcDi, src, dst,"libHarfBuzzSharp.dylib");
            Copy(programFiles, srcDi, src, dst,"libSkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst,"libSkiaSharp.dylib");
            Copy(programFiles, srcDi, src, dst,"Microsoft.Win32.Registry.AccessControl.dll");
            Copy(programFiles, srcDi, src, dst,"Microsoft.Win32.Registry.dll");
            Copy(programFiles, srcDi, src, dst,"Microsoft.Win32.SystemEvents.dll");
            Copy(programFiles, srcDi, src, dst,"OxyPlot.SkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst,"SkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst,"SkiaSharp.HarfBuzz.dll");
            Copy(programFiles, srcDi, src, dst,"System.CodeDom.dll");
            Copy(programFiles, srcDi, src, dst,"System.Configuration.ConfigurationManager.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.Odbc.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.OleDb.dll");
            Copy(programFiles, srcDi, src, dst,"System.Data.SqlClient.dll");
            Copy(programFiles, srcDi, src, dst,"System.Diagnostics.EventLog.dll");
            Copy(programFiles, srcDi, src, dst,"System.Diagnostics.PerformanceCounter.dll");
            Copy(programFiles, srcDi, src, dst,"System.Drawing.Common.dll");
            Copy(programFiles, srcDi, src, dst,"System.IO.FileSystem.AccessControl.dll");
            Copy(programFiles, srcDi, src, dst,"System.IO.Packaging.dll");
            Copy(programFiles, srcDi, src, dst,"System.IO.Pipes.AccessControl.dll");
            Copy(programFiles, srcDi, src, dst,"System.IO.Ports.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.AccessControl.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Cryptography.Cng.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Cryptography.Pkcs.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Cryptography.ProtectedData.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Cryptography.Xml.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Permissions.dll");
            Copy(programFiles, srcDi, src, dst,"System.Security.Principal.Windows.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.Duplex.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.Http.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.NetTcp.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.Primitives.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.Security.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceModel.Syndication.dll");
            Copy(programFiles, srcDi, src, dst,"System.ServiceProcess.ServiceController.dll");
            Copy(programFiles, srcDi, src, dst,"System.Text.Encoding.CodePages.dll");
            Copy(programFiles, srcDi, src, dst,"System.Threading.AccessControl.dll");
            Copy(programFiles, srcDi, src, dst,"libSkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst,"libSkiaSharp.dll");

            Copy(programFiles, srcDi, src, dst,"libSkiaSharp.dll");
            //string srcarm = Path.Combine(src, "arm");
            //string dstarm = Path.Combine(dst, "arm");
            Copy(programFiles, srcDi, src, dst, "arm64\\libSkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst, "x64\\libSkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst, "x86\\libSkiaSharp.dll");
            Copy(programFiles, srcDi, src, dst, "arm\\libSkiaSharp.so");
            Copy(programFiles, srcDi, src, dst, "arm64\\libSkiaSharp.so");
            Copy(programFiles, srcDi, src, dst, "x64\\libSkiaSharp.so");
            //Copy(programFiles, srcDi, src, dst,"EPPlus.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Collections.Immutable.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Composition.AttributedModel.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Composition.Convention.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Composition.Hosting.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Composition.Runtime.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Composition.TypedParts.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Reflection.Metadata.dll");
            //Copy(programFiles, srcDi, src, dst,"System.Text.Encoding.CodePages.dll");
            Copy(programFiles, srcDi, src, dst,"System.Threading.Tasks.Extensions.dll");
            Copy(programFiles, srcDi, src, dst,"xunit.abstractions.dll");
            //string src64 = Path.Combine(src, "x64");
            //Copy(programFiles, src64, dst, "sqlite3.dll");
            Copy(programFiles, srcDi, src, dst,"System.ValueTuple.dll");
            Copy(programFiles, srcDi, src, dst,"Utf8Json.dll");

            CheckIfFilesAreCompletelyCopied(src, programFiles);

            return programFiles;
            //Copy(programFiles, srcDi, src, dst,"netstandard.dll");
        }
    }
}