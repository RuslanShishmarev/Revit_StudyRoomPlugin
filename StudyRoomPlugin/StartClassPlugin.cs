using System;
using System.Collections.Generic;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;

namespace StudyRoomPlugin
{
    [TransactionAttribute(TransactionMode.Manual)]
    [RegenerationAttribute(RegenerationOption.Manual)]

    public class StartClassPlugin : IExternalCommand
    {        
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            Tuple<List<Room>, List<Level>> result = WorkClass.GetAllRooms(doc);

            List<Room> allRooms = result.Item1;
            List<Level> allRoomLevel = result.Item2;

            //Передаем все помещения и уровни в класс для отображения
            UserWindRoom userWind = new UserWindRoom(allRooms, allRoomLevel, doc);

            userWind.ShowDialog();

            return Result.Succeeded;
        }
    }
}
