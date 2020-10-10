using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
namespace StudyRoomPlugin
{
    class WorkClass
    {
        public static Tuple<List<Room>, List<Level>> GetAllRooms(Document doc)
        {
            FilteredElementCollector newRoomFilter = new FilteredElementCollector(doc);
            ICollection<Element> allRooms = newRoomFilter.OfCategory(BuiltInCategory.OST_Rooms).WhereElementIsNotElementType().ToElements();

            List<Level> allRoomLevel = new List<Level>();

            List<string> levelNames = new List<string>();

            List<Room> allRoomsList = new List<Room>();

            Dictionary<string, List<Room>> allRoomsByLevelDict = new Dictionary<string, List<Room>>();
            
            foreach (Element roomEl in allRooms)
            {
                Room room = roomEl as Room;
                Level level = room.Level;

                if (allRoomsByLevelDict.ContainsKey(level.Name))
                    allRoomsByLevelDict[level.Name].Add(room);
                else
                {
                    List<Room> roomList = new List<Room>() { room };
                    allRoomsByLevelDict[level.Name] = roomList;
                }
                if (!levelNames.Contains(level.Name))
                {
                    levelNames.Add(level.Name);
                    allRoomLevel.Add(level);
                }
            }

            foreach(string key in allRoomsByLevelDict.Keys)
            {
                Room[] roomsArray = allRoomsByLevelDict[key].ToArray();
                Array.Sort(roomsArray, new RoomComparerByNum());
                List <Room> rooms = roomsArray.ToList();
                foreach (Room room in rooms)
                    allRoomsList.Add(room);
            }

            return Tuple.Create(allRoomsList, allRoomLevel); 
        }
    }
}
