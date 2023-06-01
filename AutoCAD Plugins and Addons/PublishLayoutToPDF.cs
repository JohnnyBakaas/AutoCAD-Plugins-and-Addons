using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.Runtime;


namespace AutoCAD_Plugins_and_Addons
{
    internal class PublishLayoutToPDF
    {
        [CommandMethod("PublishLayoutToPDFA1")]
        public void PublishLayoutToPDFA1()
        {
            // Get the active document
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;

            // Get the layout name as an argument from AutoCAD
            PromptStringOptions promptOptions = new PromptStringOptions("\nEnter the layout name: ");
            PromptResult promptResult = doc.Editor.GetString(promptOptions);
            if (promptResult.Status != PromptStatus.OK)
                return;

            string layoutName = promptResult.StringResult.Trim();

            // Get the current database and transaction
            Database database = doc.Database;
            using (Transaction transaction = database.TransactionManager.StartTransaction())
            {
                // Get the layout from the database using the provided layout name
                Layout layout = GetLayout(database, layoutName);
                if (layout == null)
                {
                    // Layout not found, handle the error
                    doc.Editor.WriteMessage($"Layout '{layoutName}' not found.");
                    return;
                }

                // Publish the layout to PDF
                PublishPdf(doc, layout);
            }
        }

        private Layout GetLayout(Database database, string layoutName)
        {
            Layout layout = null;

            // Open the layouts dictionary
            DBDictionary layoutDict = (DBDictionary)database.LayoutDictionaryId.GetObject(OpenMode.ForRead);

            // Check if the layout name exists in the dictionary
            if (layoutDict.Contains(layoutName))
            {
                ObjectId layoutId = layoutDict.GetAt(layoutName);
                layout = layoutId.GetObject(OpenMode.ForRead) as Layout;
            }

            return layout;
        }

        private void PublishPdf(Document doc, Layout layout)
        {
            using (var plotInfo = new PlotInfo())
            {
                using (var plotSettings = new PlotSettings(layout.ModelType))
                {
                    plotSettings.CopyFrom(layout);

                    using (var plotSettingsValidator = PlotSettingsValidator.Current)
                    {
                        plotSettingsValidator.SetPlotType(plotSettings, Autodesk.AutoCAD.DatabaseServices.PlotType.Extents);
                        plotSettingsValidator.SetUseStandardScale(plotSettings, true);
                        plotSettingsValidator.SetStdScaleType(plotSettings, StdScaleType.ScaleToFit);

                        // Set the PDF options
                        plotSettingsValidator.SetPlotConfigurationName(plotSettings, "DWG To PDF.pc3", "ISO_full_bleed_A1_(841.00_x_594.00_MM)");
                        plotSettingsValidator.SetPlotCentered(plotSettings, true);

                        plotInfo.Layout = layout.ObjectId;
                        plotInfo.OverrideSettings = plotSettings;

                        using (var plotEngine = PlotFactory.CreatePublishEngine())
                        {
                            plotEngine.BeginPlot(null, null);
                            plotEngine.BeginDocument(plotInfo, doc.Name, null, 1, true, "");
                            PlotPageInfo pageInfo = new PlotPageInfo();
                            plotEngine.BeginPage(pageInfo, plotInfo, true, null);
                            plotEngine.EndPage(null);
                            plotEngine.EndDocument(null);
                            plotEngine.EndPlot(false);
                        }
                    }
                }
            }
        }



    }
}
