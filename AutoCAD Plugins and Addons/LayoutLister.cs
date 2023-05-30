
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace Test_329.NET_4._8
{
    public class LayoutLister
    {
        [CommandMethod("ListLayouts")]
        public void ListLayouts()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            using (Transaction tr = doc.Database.TransactionManager.StartTransaction())
            {
                Editor editor = Autodesk.AutoCAD.ApplicationServices.Application.DocumentManager.MdiActiveDocument.Editor;
                DBDictionary layouts = (DBDictionary)tr.GetObject(doc.Database.LayoutDictionaryId, OpenMode.ForRead);
                editor.WriteMessage("\nLayout names: ");
                foreach (DBDictionaryEntry entry in layouts)
                {
                    editor.WriteMessage($" |||{entry.Key}||| ");
                }
            }
        }
    }
}
