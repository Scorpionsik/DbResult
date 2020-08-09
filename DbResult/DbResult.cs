using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Объект-таблица, сформированная из полученных данных.
    /// </summary>
    public class DbResult : IEnumerable<DbRow>, IEnumerator<DbRow>
    {
        private List<string> columns;
        /// <summary>
        /// Названия столбцов таблицы.
        /// </summary>
        public string[] Columns
        {
            get => this.columns.ToArray();
        }

        /// <summary>
        /// Типы данных в столбцах таблицы.
        /// </summary>
        private List<Type> ColumnTypes;

        /// <summary>
        /// Коллекция строк таблицы.
        /// </summary>
        private List<List<object>> Rows;

        #region IEnumerable

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns> Объект System.Collections.IEnumerator, который используется для прохода по коллекции.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        /// <summary>
        /// Возвращает перечислитель, который осуществляет итерацию по коллекции.
        /// </summary>
        /// <returns> Объект System.Collections.IEnumerator, который используется для прохода по коллекции.</returns>
        IEnumerator<DbRow> IEnumerable<DbRow>.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerator
        private int index = -1;

        /// <summary>
        /// Перемещает перечислитель к следующему элементу коллекции строк таблицы.
        /// </summary>
        /// <returns>Значение true, если перечислитель был успешно перемещен к следующему элементу; значение false, если перечислитель достиг конца коллекции.</returns>
        /// <exception cref="InvalidOperationException">Коллекция была изменена после создания перечислителя.</exception>
        public bool MoveNext()
        {
            if (index == this.Rows.Count - 1)
            {
                this.Reset();
                return false;
            }

            index++;
            return true;
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
        /// Необходим для реализации интерфейса <see cref="IDisposable"/>, не используется в этом классе.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Возвращает элемент коллекции, соответствующий текущей позиции перечислителя.
        /// </summary>
        object IEnumerator.Current
        {
            get
            {
                return this.GetRow(index);
            }
        }

        /// <summary>
        /// Возвращает элемент коллекции, соответствующий текущей позиции перечислителя.
        /// </summary>
        DbRow IEnumerator<DbRow>.Current
        {
            get
            {
                return this.GetRow(index);
            }
        }
        #endregion

        /// <summary>
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue this[int column, int row]
        {
            get => this.GetValue(column, row);
        }

        /// <summary>
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue this[string column_name, int row]
        {
            get => this.GetValue(column_name, row);
        }

        /// <summary>
        /// Возвращает все элементы по номеру строки.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <returns>Коллекция элементов таблицы <see cref="DbRow"/>.</returns>
        public DbRow this[int row]
        {
            get
            {
                return this.GetRow(row);
            }
        }

        /// <summary>
        /// Возвращает все элементы по названию столбца.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Коллекция элементов таблицы <see cref="DbColumn"/>.</returns>
        public DbColumn this[string column_name]
        {
            get
            {
                return this.GetColumn(column_name);
            }
        }

        /// <summary>
        /// Количество строк в таблице.
        /// </summary>
        public int RowsCount
        {
            get => this.Rows.Count;
        }

        public int ColumnsCount
        {
            get => this.columns.Count;
        }

        /// <summary>
        /// Есть ли в таблице хотя бы 1 строка; true, если есть.
        /// </summary>
        public bool HasRows
        {
            get => this.Rows.Count == 0 ? false : true;
        }

        /// <summary>
        /// Считывает один или несколько прямонаправленных потоков наборов результатов и формирует двумерный массив из полученных данных.
        /// </summary>
        /// <param name="data">Данные для считывания.</param>
        /// <param name="closeData">Нужно ли вызвать для <paramref name="data"/> метод <see cref="DbDataReader.Close"/>; true, если нужно вызвать.</param>
        public DbResult(DbDataReader data, bool closeData = true)
        {
            this.ColumnTypes = new List<Type>();
            this.columns = new List<string>();
            this.Rows = new List<List<object>>();
            if (data.HasRows)
            {
                while (data.Read())
                {
                    List<object> tmp_row = new List<object>();
                    int countCols = data.FieldCount;
                    for (int i = 0; i < countCols; i++)
                    {
                        if(this.ColumnsCount < countCols)
                        {
                            this.columns.Add(data.GetName(i));
                            this.ColumnTypes.Add(data.GetFieldType(i));
                        }
                        tmp_row.Add(data.GetValue(i));
                    }
                    this.Rows.Add(tmp_row);
                }
            }
            if(closeData) data.Close();
        }

        private DbResult(IEnumerable<string> columns, IEnumerable<Type> columnTypes, IEnumerable<List<object>> rows)
        {
            this.columns = new List<string>(columns);
            this.ColumnTypes = new List<Type>(columnTypes);
            this.Rows = new List<List<object>>(rows);
        }

        /// <summary>
        /// Получает элемент таблицы по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(int column, int row)
        {
            return this.Rows[row][column];
        }

        /// <summary>
        /// Получает элемент таблицы по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(string column_name, int row)
        {
            int column = this.columns.IndexOf(column_name);
            return this.Rows[row][column];
        }

        /// <summary>
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue GetValue(int column, int row)
        {
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column], row, this.columns[column]);
        }

        /// <summary>
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue GetValue(string column_name, int row)
        {
            int column = this.columns.IndexOf(column_name);
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column], row, column_name);
        }

        /// <summary>
        /// Возвращает все элементы по номеру строки.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <returns>Коллекция элементов таблицы <see cref="DbRow"/>.</returns>
        public DbRow GetRow(int row)
        {
            return new DbRow(row, this.Rows[row], this.Columns, this.ColumnTypes);
        }

        /// <summary>
        /// Возвращает все элементы по номеру столбца.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <returns>Коллекция элементов таблицы <see cref="DbColumn"/>.</returns>
        public DbColumn GetColumn(int column)
        {
            string column_name = this.Columns.ElementAt(column);
            Type type = this.GetColumnType(column);
            List<object> tmp_values = new List<object>();
            foreach (List<object> tmp_row in this.Rows)
            {
                tmp_values.Add(tmp_row[column]);
            }
            return new DbColumn(column_name, type, tmp_values);
        }

        /// <summary>
        /// Возвращает все элементы по названию столбца.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Коллекция элементов таблицы <see cref="DbColumn"/>.</returns>
        public DbColumn GetColumn(string column_name)
        {
            int tmp_col_index = this.columns.IndexOf(column_name);
            Type type = this.GetColumnType(tmp_col_index);
            List<object> tmp_values = new List<object>();
            foreach(List<object> tmp_row in this.Rows)
            {
                tmp_values.Add(tmp_row[tmp_col_index]);
            }
            return new DbColumn(column_name, type, tmp_values);
        }

        /// <summary>
        /// Получить тип данных по номеру столбца.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <returns>Тип данных выбранного столбца.</returns>
        public Type GetColumnType(int column)
        {
            return this.ColumnTypes[column];
        }

        /// <summary>
        /// Получить тип данных по названию столбца.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Тип данных выбранного столбца.</returns>
        public Type GetColumnType(string column_name)
        {
            int column = this.columns.IndexOf(column_name);
            return this.ColumnTypes[column];
        }

    
    }
}
