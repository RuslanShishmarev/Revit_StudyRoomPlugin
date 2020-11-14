using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace StudyRoomPlugin
{
    /// <summary>
    /// Логика взаимодействия для UserWindRoom.xaml
    /// </summary>
    public partial class UserWindRoom : Window
    {
        List<Room> _allRooms;
        List<Level> _allLevels;
        Document _doc;
        public UserWindRoom(List<Room> rooms, List<Level> levels, Document doc)
        {
            InitializeComponent();
            _allRooms = rooms;
            _allLevels = levels;
            _doc = doc;
            //добавляем в визуальный список помещения
            AllRoomsView.ItemsSource = _allRooms;
            AllRoomsView.DisplayMemberPath = "Name";
        }
        private void SortRoomsInProject(Object sender, EventArgs e)
        {
            //передаем окну выбора этажа и сортировки все уровни, помещения, документ и визуальный список UI для помещений
            LevelViewWind levelWind = new LevelViewWind(_allLevels, _allRooms, _doc, AllRoomsView);

            //присваиваем новому окну положение по центру, относительно основного окна
            levelWind.Owner = this;
            levelWind.WindowStartupLocation = WindowStartupLocation.CenterOwner;

            //открываем окно выбора сортировки            
            levelWind.ShowDialog();
        }
        private void SetToUIRoomName(object sender, SelectionChangedEventArgs args)
        {
            //Получаем имя помещения по выбранному объекту в списке UI
            Room room = AllRoomsView.SelectedItem as Room;
            NewNameRoomView.Text = room.get_Parameter(BuiltInParameter.ROOM_NAME).AsString();
        }
        private void SetNewRoomName(Object sender, EventArgs e)
        {
            //Получаем значение текста для нового имени
            string newName = NewNameRoomView.Text;
            //получаем помещение
            Room room = AllRoomsView.SelectedItem as Room;
            //записываем новое имя
            using (Transaction t = new Transaction(_doc))
            {
                t.Start("SetName");
                room.get_Parameter(BuiltInParameter.ROOM_NAME).Set(newName);
                t.Commit();
            }
            //обновляем UI список
            AllRoomsView.Items.Refresh();
        }
    }
}
