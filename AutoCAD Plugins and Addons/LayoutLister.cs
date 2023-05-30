
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;

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
                DBDictionary layouts = (DBDictionary)tr.GetObject(doc.Database.LayoutDictionaryId, OpenMode.ForRead);
                Console.WriteLine("\nLayout names: ");
                foreach (DBDictionaryEntry entry in layouts)
                {
                    Console.WriteLine(entry.Key);
                }
            }
        }
    }
}
