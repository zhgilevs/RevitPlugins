using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows;
using System.Windows.Controls;

namespace RevitPlugins.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    internal class Lesson2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document document = commandData.Application.ActiveUIDocument.Document;

            BuiltInCategory[] categories = new BuiltInCategory[]
                {
                    BuiltInCategory.OST_DuctCurves,
                    BuiltInCategory.OST_DuctFitting
                };

            using (Transaction trans = new Transaction(document, "Заполняем в ADSK_Группирование значение системного параметра Имя системы"))
            {
                trans.Start();

                foreach (BuiltInCategory bic in categories)
                {
                    FilteredElementCollector collector = new FilteredElementCollector(document)
                        .OfCategory(bic)
                        .WhereElementIsNotElementType();

                    foreach (Element elem in collector)
                    {
                        // Получаем значение параметра "Имя системы"
                        Parameter systemNameParam = elem.get_Parameter(BuiltInParameter.RBS_SYSTEM_NAME_PARAM);
                        string systemName = systemNameParam != null ? systemNameParam.AsString() : null;

                        // Получаем параметр "ADSK_Группирование"
                        Parameter groupParam = elem.LookupParameter("ADSK_Группирование");

                        // Устанавливаем значение
                        if (groupParam != null && !groupParam.IsReadOnly && !string.IsNullOrEmpty(systemName))
                        {
                            groupParam.Set(systemName);
                        }
                    }
                }

                trans.Commit();
            }

            MessageBox.Show("Проверь ADSK_Группирование у воздуховодов и фитингов воздуховодов");

            return Result.Succeeded;
        }
    }
}
