using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    /// <summary>
    /// Объект-таблица, сформированная из полученных данных.
    /// </summary>
    public struct DbResult : IEnumerable<DbRow>, IEnumerator<DbRow>
    {
        public static DbResult Empty { get; private set; }

        static DbResult()
        {
            Empty = new DbResult();
        }

        /// <summary>
        /// Названия столбцов таблицы.
        /// </summary>
        public string[] Columns { get; private set; }

        /// <summary>
        /// Типы данных в столбцах таблицы.
        /// </summary>
        public Type[] ColumnTypes { get; private set; }

        /// <summary>
        /// Коллекция строк таблицы.
        /// </summary>
        private List<object[]> Rows;

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
        private int index;

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
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object this[string column_name, int row]
        {
            get => this.GetObject(column_name, row);
        }

        /// <summary>
        /// Возвращает элемент таблицы в формате <see cref="DbValue"/> по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object this[int column, int row]
        {
            get => this.GetObject(column, row);
        }

        /*
        public DbRow this[int row]
        {
            get
            {
                return this.GetRow(row);
            }
        }

        public DbColumn this[string column_name]
        {
            get
            {
                return this.GetColumn(column_name);
            }
        }
        */

        /// <summary>
        /// Количество строк в таблице.
        /// </summary>
        public int RowsCount
        {
            get => this.Rows.Count;
        }

        /// <summary>
        /// Количество столбцов в таблице.
        /// </summary>
        public int ColumnsCount
        {
            get => this.Columns.Length;
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
            this.index = -1;
            this.ColumnTypes = new Type[0];
            this.Columns = new string[0];
            this.Rows = new List<object[]>();

            if (data.HasRows)
            {
                bool init = false;
                bool init_column = false;
                while (data.Read())
                {
                    int countCols = data.FieldCount;

                    if (!init)
                    {
                        this.ColumnTypes = new Type[countCols];
                        this.Columns = new string[countCols];
                        this.Rows = new List<object[]>();

                        init = true;
                    }

                    object[] tmp_row = new object[countCols];

                    
                    for (int i = 0; i < countCols; i++)
                    {
                        if(!init_column)
                        {
                            this.Columns[i] = data.GetName(i);
                            this.ColumnTypes[i] = data.GetFieldType(i);
                        }
                        tmp_row[i] = data.GetValue(i);
                    }
                    this.Rows.Add(tmp_row);
                    if (!init_column) init_column = true;
                }
            }
            if(closeData) data.Close();
        }

        /*
        private DbResult(IEnumerable<string> columns, IEnumerable<Type> columnTypes, IEnumerable<object[]> rows)
        {
            this.index = -1;
            this.Columns = columns.ToArray();
            this.ColumnTypes = columnTypes.ToArray();
            this.Rows = rows.ToList();
        }
        */

        #region DataTable
        /// <summary>
        /// Считывает один или несколько прямонаправленных потоков наборов результатов и создает на основе полученных данных экземпляр класса <see cref="DataTable"/>.
        /// </summary>
        /// <param name="data">Данные для считывания.</param>
        /// <param name="closeData">Нужно ли вызвать для <paramref name="data"/> метод <see cref="DbDataReader.Close"/>; true, если нужно вызвать.</param>
        /// <returns>Таблица данных.</returns>
        public static DataTable ToDataTable(DbDataReader data, bool closeData = true)
        {
            DataTable result = new DataTable();

            if (data.HasRows)
            {
                bool getColumn = false;
                while (data.Read())
                {
                    if (!getColumn)
                    {
                        for (int i = 0; i < data.FieldCount; i++)
                        {
                            result.Columns.Add(new DataColumn(data.GetName(i), data.GetFieldType(i)));
                        }
                        getColumn = true;
                    }

                    DataRow newRow = result.NewRow();
                    for(int i = 0; i < data.FieldCount; i++)
                    {
                        newRow[i] = data.GetValue(i);
                    }
                    result.Rows.Add(newRow);
                }
            }
            if (closeData) data.Close();
            return result;
        }

        /// <summary>
        /// Считывает <see cref="DbResult"/> и создает на его основе экземпляр класса <see cref="DataTable"/>.
        /// </summary>
        /// <param name="data">Данные для считывания.</param>
        /// <returns>Таблица данных.</returns>
        public static DataTable ToDataTable(DbResult data)
        {
            DataTable result = new DataTable();

            if (data.HasRows)
            {
                for (int i = 0; i < data.ColumnsCount; i++)
                {
                    result.Columns.Add(new DataColumn(data.Columns[i], data.ColumnTypes[i]));
                }

                for (int i = 0; i < data.RowsCount; i++)
                {
                    DataRow newRow = result.NewRow();
                    for(int j = 0; j < data.ColumnsCount; j++)
                    {
                        newRow[j] = data.GetObject(j, i);
                    }
                    result.Rows.Add(newRow);
                }   
            }

            return result;
        }

        /// <summary>
        /// Конвертирует текущий объект в экземпляр класса <see cref="DataTable"/>.
        /// </summary>
        /// <returns>Таблица данных.</returns>
        public DataTable ToDataTable()
        {
            return ToDataTable(this);
        }
        #endregion
       
        /// <summary>
        /// Возвращает объект по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(int column, int row)
        {
            return this.Rows[row][column];
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="int"/> и возвращает результат.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public int GetInt32(int column, int row)
        {
            return Convert.ToInt32(this.GetObject(column, row));
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="int"/> и возвращает результат.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public int GetInt32(string column_name, int row)
        {
            return Convert.ToInt32(this.GetObject(column_name, row));
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="double"/> и возвращает результат.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public double GetDouble(int column, int row)
        {
            return Convert.ToDouble(this.GetObject(column, row));
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="double"/> и возвращает результат.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public double GetDouble(string column_name, int row)
        {
            return Convert.ToDouble(this.GetObject(column_name, row));
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="bool"/> и возвращает результат.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public bool GetBoolean(int column, int row)
        {
            return Convert.ToBoolean(this.GetObject(column, row));
        }

        /// <summary>
        /// Пытается конвертировать объект в <see cref="bool"/> и возвращает результат.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public bool GetBoolean(string column_name, int row)
        {
            return Convert.ToBoolean(this.GetObject(column_name, row));
        }

        /// <summary>
        /// Возвращает объект по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public object GetObject(string column_name, int row)
        {
            int column = Array.IndexOf(this.Columns, column_name);
            return this.Rows[row][column];
        }

        /// <summary>
        /// Создает элемент таблицы в формате <see cref="DbValue"/> по номеру столбца и номеру строки.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue GetValue(int column, int row)
        {
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column], row, this.Columns[column]);
        }

        /// <summary>
        /// Создает элемент таблицы в формате <see cref="DbValue"/> по названию столбца и номеру строки.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <param name="row">Номер строки.</param>
        /// <returns>Элемент таблицы.</returns>
        public DbValue GetValue(string column_name, int row)
        {
            int column = Array.IndexOf(this.Columns, column_name);
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column], row, column_name);
        }

        /// <summary>
        /// Находит строку по номеру и создает на её основе объект <see cref="DbRow"/>.
        /// </summary>
        /// <param name="row">Номер строки.</param>
        /// <returns>Строка в формате <see cref="DbRow"/>.</returns>
        public DbRow GetRow(int row)
        {
            return new DbRow(row, this.Rows[row], this.Columns, this.ColumnTypes);
        }

        /// <summary>
        /// Находит столбец по номеру и создает на его основе объект <see cref="DbColumn"/>.
        /// </summary>
        /// <param name="column">Номер столбца.</param>
        /// <returns>Столбец в формате <see cref="DbColumn"/>.</returns>
        public DbColumn GetColumn(int column)
        {
            string column_name = this.Columns.ElementAt(column);
            Type type = this.GetColumnType(column);
            List<object> tmp_values = new List<object>();
            foreach (object[] tmp_row in this.Rows)
            {
                tmp_values.Add(tmp_row[column]);
            }
            return new DbColumn(column_name, type, tmp_values);
        }

        /// <summary>
        /// Находит столбец по названию и создает на его основе объект <see cref="DbColumn"/>.
        /// </summary>
        /// <param name="column_name">Название столбца.</param>
        /// <returns>Столбец в формате <see cref="DbColumn"/>.</returns>
        public DbColumn GetColumn(string column_name)
        {
            int tmp_col_index = Array.IndexOf(this.Columns, column_name);
            Type type = this.GetColumnType(tmp_col_index);
            List<object> tmp_values = new List<object>();
            foreach(object[] tmp_row in this.Rows)
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
            int column = Array.IndexOf(this.Columns, column_name);
            return this.ColumnTypes[column];
        }
    }
}
