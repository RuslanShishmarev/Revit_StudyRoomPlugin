using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;

namespace StudyRoomPlugin
{
    class RoomComparerByNum : IComparer<Room>
    {
        public int Compare(Room x, Room y)
        {
            double num1 = Convert.ToDouble(x.Number.Replace(".", ","));
            double num2 = Convert.ToDouble(y.Number.Replace(".", ","));
            if (num1 > num2)
                return 1;
            else if (num1 < num2)
                return -1;
            else
                return 0;
        }
    }
}
