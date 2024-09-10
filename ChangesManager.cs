using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AccountantApp
{
    public static class ChangesManager
    {
        public class UndoRedoAction<TObj, TProp>
        {
            private Action<TObj, TProp> _setter;
            private TObj _target;
            private TProp _oldValue;
            private TProp _newValue;

            public UndoRedoAction(Action<TObj, TProp> setter, TObj target, TProp oldValue, TProp newValue)
            {
                _setter = setter;
                _target = target;
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public void Undo()
            {
                _setter(_target, _oldValue);
            }

            public void Redo()
            {
                _setter(_target, _newValue);
            }
        }

        public struct Change
        {
            public DateTime dateTime { get; set; }
            public string name { get; set; }
            public string status { get; set; }
            public string description { get; set; }

            public UndoRedoAction<object, object> undoRedoAction { get; set; }

            public Change()
            {
                dateTime = DateTime.Now;
                name = "none";
                status = "none";
                description = "none";
                undoRedoAction = new UndoRedoAction<object, object>((o, p) => { }, new object(), new object(), new object());
            }
        }

        private static ObservableCollection<Change> changes = new ObservableCollection<Change>();
        
        public static void addChange(Change change)
        {
            changes.Add(change);
        }

        public static void bind(DataGrid dataGrid)
        {
            dataGrid.ItemsSource = changes;
            changes.Clear();
        }

        public static void Undo()
        {
            for (int i = changes.Count - 1; i >= 0; i--)
            {
                if (changes[i].status != "Undo")
                {
                    var c = changes[i];
                    c.status = "Undo";
                    c.undoRedoAction.Undo();
                    changes[i] = c;
                    break;
                }
            }
        }

        public static void Redo()
        {
            for (int i = 0; i < changes.Count; i++)
            {
                if (changes[i].status == "Undo")
                {
                    var c = changes[i];
                    c.status = "Redo";
                    c.undoRedoAction.Redo();
                    changes[i] = c;
                    break;
                }
            }
        }
    }
}
