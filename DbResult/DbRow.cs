using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    public struct DbRow : IEnumerable<DbValue>, IEnumerator<DbValue>
    {
        /// <summary>
        /// Номер текущей строки в базе данных.
        /// </summary>
        public int RowIndex { get; private set; }

        /// <summary>
        /// Названия столбцов таблицы.
        /// </summary>
        public string[] Columns { get; private set; }

        /// <summary>
        /// Типы данных в столбцах таблицы.
        /// </summary>
        public Type[] ColumnTypes { get; private set; }

        /// <summary>
        /// Значения в строке.
        /// </summary>
        private object[] Values;

        #region IEnumerable
        IEnumerator<DbValue> IEnumerable<DbValue>.GetEnumerator()
        {
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerator
        private int index;

        /// <summary>
        /// Перемещает перечислитель к следующему элементу коллекции строк таблицы.
        /// </summary>
        /// <returns>Значение true, если перечислитель был успешно перемещен к следующему элементу; значение false, если перечислитель достиг конца коллекции.</returns>
        /// <exception cref="InvalidOperationException">Коллекция была изменена после создания перечислителя.</exception>
        public bool MoveNext()
        {
            if (index == this.Values.Length - 1)
            {
                this.Reset();
                return false;
            }

            index++;
            return true;
        }

        /// <summary>
        /// Необходим для реализации интерфейса <see cref="IDisposable"/>, не используется в этом классе.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Устанавливает перечислитель в его начальное положение, т. е. перед первым элементом коллекции.
        /// </summary>
        /// <exception cref="InvalidOperationException">Коллекция была изменена после создания перечислителя.</exception>
        public void Reset()
        {
            index = -1;
        }

        /// <summary>
        /// Возвращает элемент коллекции, соответствующий текущей позиции перечислителя.
        /// </summary>
        public object Current
        {
            get
            {
                return this.GetValue(index);
            }
        }

        DbValue IEnumerator<DbValue>.Current
        {
            get
            {
                return this.GetValue(index);
            }
        }
        #endregion
        /// <summary>
        /// Возвращает элемент строки по номеру столбца.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <returns>Элемент таблицы.</returns>
        public object this[int column]
        {
            get
            {
                return this.GetObject(column);
            }
        }

        /// <summary>
        /// Возвращает элемент строки по названию столбца.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Элемент таблицы.</returns>
        public object this[string column_name]
        {
            get
            {
                return this.GetObject(column_name);
            }
        }

        internal DbRow(int row_index, IEnumerable<object> values, IEnumerable<string> columns, IEnumerable<Type> columnTypes)
        {
            this.index = -1;
            this.RowIndex = row_index;
            this.Columns = columns.ToArray();
            this.Values = values.ToArray();
            this.ColumnTypes = columnTypes.ToArray();
        }

        /// <summary>
        /// Возвращает элемент строки по номеру столбца.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(int column)
        {
            return this.Values[column];
        }

        /// <summary>
        /// Возвращает элемент строки по названию столбца.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(string column_name)
        {
            int column = Array.IndexOf(this.Columns, column_name);
            return this.Values[column];
        }

        public int GetInt32(int column)
        {
            return Convert.ToInt32(this.GetObject(column));
        }

        public int GetInt32(string column_name)
        {
            return Convert.ToInt32(this.GetObject(column_name));
        }

        public double GetDouble(int column)
        {
            return Convert.ToDouble(this.GetObject(column));
        }

        public double GetDouble(string column_name)
        {
            return Convert.ToDouble(this.GetObject(column_name));
        }

        public bool GetBoolean(int column)
        {
            return Convert.ToBoolean(this.GetObject(column));
        }

        public bool GetBoolean(string column_name)
        {
            return Convert.ToBoolean(this.GetObject(column_name));
        }

        public DbValue GetValue(int column)
        {
            return new DbValue(this.ColumnTypes[column], this.Values[column], this.RowIndex, this.Columns[column]);
        }

        public DbValue GetValue(string column_name)
        {
            int column = Array.IndexOf(this.Columns, column_name);
            return new DbValue(this.ColumnTypes[column], this.Values[column], this.RowIndex, column_name);
        }

        public Type GetColumnType(int column)
        {
            return this.ColumnTypes[column];
        }

        public Type GetColumnType(string column_name)
        {
            int column = Array.IndexOf(this.Columns, column_name);
            return this.ColumnTypes[column];
        }
    }
}
