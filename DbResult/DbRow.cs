using System.Collections;
using System.Collections.Generic;

namespace System.Data.Common
{
    public struct DbRow : IEnumerable<DbValue>, IEnumerator<DbValue>
    {
        public int RowIndex { get; private set; }

        private List<string> column;
        public string[] Column
        {
            get => this.column.ToArray();
        }
        private List<Type> ColumnTypes;
        private List<object> Values;

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

        public bool MoveNext()
        {
            if (index == this.Values.Count - 1)
            {
                this.Reset();
                return false;
            }

            index++;
            return true;
        }

        public void Dispose()
        {

        }

        public void Reset()
        {
            index = -1;
        }

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

        public DbValue this[int column]
        {
            get
            {
                return this.GetValue(column);
            }
        }

        public DbValue this[string column_name]
        {
            get
            {
                return this.GetValue(column_name);
            }
        }

        internal DbRow(int row_index, IEnumerable<object> values, IEnumerable<string> columns, IEnumerable<Type> columnTypes)
        {
            this.index = -1;
            this.RowIndex = row_index;
            this.column = new List<string>(columns);
            this.Values = new List<object>(values);
            this.ColumnTypes = new List<Type>(columnTypes);
        }

        public object GetObject(int column)
        {
            return this.Values[column];
        }

        public object GetObject(string column_name)
        {
            int column = this.column.IndexOf(column_name);
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
            return new DbValue(this.ColumnTypes[column], this.Values[column], this.RowIndex, this.column[column]);
        }

        public DbValue GetValue(string column_name)
        {
            int column = this.column.IndexOf(column_name);
            return new DbValue(this.ColumnTypes[column], this.Values[column], this.RowIndex, column_name);
        }

        public Type GetColumnType(int column)
        {
            return this.ColumnTypes[column];
        }

        public Type GetColumnType(string column_name)
        {
            int column = this.column.IndexOf(column_name);
            return this.ColumnTypes[column];
        }
    }
}
