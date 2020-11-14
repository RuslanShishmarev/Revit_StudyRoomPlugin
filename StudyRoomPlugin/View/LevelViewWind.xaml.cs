using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;

namespace StudyRoomPlugin
{
    /// <summary>
    /// Логика взаимодействия для LevelViewWind.xaml
    /// </summary>
    public partial class LevelViewWind : Window
    {
        List<Level> _allLevels;
        List<Room> _allRooms;
        Document _doc;
        ListBox _AllRoomsView;
        public LevelViewWind(List<Level> levels, List<Room> rooms, Document doc, ListBox AllRoomsView)
        {
            InitializeComponent();
            _allLevels = levels;
            _allRooms = rooms;
            _doc = doc;
            _AllRoomsView = AllRoomsView;
            //создаем RadioButton с уровнями и добаляем в UI
            foreach (Level level in _allLevels)
            {
                RadioButton checkLevel = new RadioButton();
                checkLevel.Content = level.Name;

                AllLevelsView.Children.Add(checkLevel);
            }
            
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            //Получаем стартовое значение от пользователя
            double startValue = Convert.ToDouble(StartNumberValueView.Text.Replace(".", ","));
            //Объявляем новый словарь для сортировки помещений по уровням
            Dictionary<string, List<Room>> allRoomsByLevels = new Dictionary<string, List<Room>>();

            foreach(Room room in _allRooms)
            {
                string levelRoomName = room.Level.Name;
                //Проверяем на наличие ключа по имени этажа
                if (allRoomsByLevels.ContainsKey(levelRoomName))
                {
                    allRoomsByLevels[levelRoomName].Add(room);
                }
                else
                {
                    List<Room> rooms = new List<Room>() { room };
                    allRoomsByLevels[levelRoomName] = rooms;
                }
            }
            //Получим все элементы RadioButton и получим 
            //Да, я знаю, что RadioButton в пределах группы может быть выбран только один. 
            //Я до этого писал для ComboBox. Мне лень переписывать:) Может вы захотите переписать для ComboBox
            UIElementCollection radioButtons = AllLevelsView.Children;
            List<String> allCheckedLevels = new List<string>();

            foreach (UIElement element in radioButtons)
            {

                RadioButton radioButton = element as RadioButton;
                if (radioButton.IsChecked == true)
                    allCheckedLevels.Add(radioButton.Content.ToString());
            }
            //Пройдемся по всем выбраннм уровням и зададим новый номер
            using(Transaction t = new Transaction(_doc))
            {
                t.Start("SetNumber");
                foreach(string level in allCheckedLevels)
                {
                    List<Room> checkedRooms = allRoomsByLevels[level];
                    for (int i = 0; i < checkedRooms.Count(); i++)
                    {
                        Room room = checkedRooms[i];
                        string roomLevelName = room.Level.Name;
                        if (allCheckedLevels.Contains(roomLevelName))
                        {
                            //Получаем значение после запятой
                            double newNumber = 0;
                            double addNum = Math.Round(startValue - Math.Truncate(startValue), 3);
                            //делаем проверку остатка на значение 0
                            if (addNum == 0)
                                newNumber = startValue + i;
                            else
                            {
                                string checkAddNum = addNum.ToString();
                                int a = Convert.ToInt32(checkAddNum.Split(',')[1]);
                                double newStep = Convert.ToDouble(addNum / a);
                                newNumber = startValue + newStep*i;
                            }
                            room.get_Parameter(BuiltInParameter.ROOM_NUMBER).Set(newNumber.ToString().Replace(",", "."));
                        }
                    }
                }
                t.Commit();
            }
            //Обновим UI 
            _AllRoomsView.Items.Refresh();
            MessageBox.Show("Все отлично!");
        }
    }
}
