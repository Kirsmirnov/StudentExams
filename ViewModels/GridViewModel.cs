using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace StudentExams.ViewModels
{
    public class GridViewModel
    {
        static int _instance;
        public int Instance { get { return _instance; } }
        public IEnumerable Items { get; set; }
        public GridViewColumn[] Columns { get; set; }
        public GridViewCommand[] Commands { get; set; }
        public string IdProperty { get; set; }
        public string AddNewUrl { get; set; }
        public string EditUrl { get; set; }
        public string DeleteUrl { get; set; }
        public string AddNewText { get; set; }
        public string DeleteConfirmText { get; set; }
        public bool AllowDelete { get; set; }
        public bool AllowEdit { get; set; }
        public bool AllowAdd { get; set; }

        public GridViewModel()
        {
            AddNewText = "Добавить...";
            DeleteConfirmText = "Удалить выбранную запись?";
            AllowAdd = true;
            AllowDelete = true;
            AllowEdit = true;
            _instance++;
            Columns = new GridViewColumn[0];
            Commands = new GridViewCommand[0];
            Items = new object[0];
        }

        public string[] FormatDataItemFields(object dataItem)
        {
            string[] fields = new string[Columns.Length];
            for (int i = 0; i < Columns.Length; i++)
            {
                object dataProperty = GetDataItemProperty(dataItem, Columns[i].Property);
                fields[i] = FormatDataItemField(dataProperty, Columns[i]);
            }
            return fields;
        }


        public object GetDataItemProperty(object dataItem, string property)
        {
            if (string.IsNullOrEmpty(property))
                return "";

            Type type = dataItem.GetType();
            PropertyInfo propInfo = type.GetProperty(property);
            if (propInfo != null)
                return propInfo.GetValue(dataItem);

            return "";
        }


        private string FormatDataItemField(object dataField, GridViewColumn column)
        {
            if (dataField == null)
                return "";

            if (dataField is DateTime)
            {
                DateTime value = Convert.ToDateTime(dataField);
                if (value == DateTime.MinValue)
                    return "";

                if (!string.IsNullOrEmpty(column.Format))
                {
                    return value.ToString(column.Format);
                }
                else
                {
                    if (value.Date == value)
                        return value.ToString("dd.MM.yyyy");
                    else
                        return value.ToString("dd.MM.yyyy HH:mm:ss");
                }
            }

            if(dataField is decimal)
            {
                decimal value = Convert.ToDecimal(dataField);
                if (!string.IsNullOrEmpty(column.Format))
                    return value.ToString(column.Format);
            }

            if(dataField is double || dataField is float)
            {
                double value = Convert.ToDouble(dataField);
                if (!string.IsNullOrEmpty(column.Format))
                    return value.ToString(column.Format);
            }

            if(dataField is int)
            {
                int value = Convert.ToInt32(dataField);
                if (!string.IsNullOrEmpty(column.Format))
                    return value.ToString(column.Format);
            }

            return dataField.ToString();
        }
    }

    public class GridViewColumn
    {
        public string Title { get; set; }
        public string Property { get; set; }
        public string Format { get; set; }
        public string HeaderStyleSpec { get; set; }
        public string StyleSpec { get; set; }
        public GridViewColumn(string title, string property)
        {
            Title = title;
            Property = property;
        }
    }


    public class GridViewCommand
    {
        public string HeaderTitle { get; set; }
        public string Title { get; set; }
        public string Hint { get; set; }
        public string Url { get; set; }
        public GridViewCommand(string title, string url) : this(title, url, "")
        {
        }
        public GridViewCommand(string title, string url, string hint)
        {
            Title = title;
            Url = url;
            Hint = hint;
        }
    }
}