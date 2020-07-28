using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Data.Common
{
    public class DbResult : IEnumerable<DbRow>, IEnumerator<DbRow>
    {
        public List<string> Columns { get; private set; }
        private List<Type> ColumnTypes;
        private List<List<object>> Rows;

        #region IEnumerable
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this;
        }

        IEnumerator<DbRow> IEnumerable<DbRow>.GetEnumerator()
        {
            return this;
        }
        #endregion

        #region IEnumerator
        private int index = -1;

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

        public void Reset()
        {
            index = -1;
        }

        public void Dispose()
        {

        }

        public object Current
        {
            get
            {
                return this.GetRow(index);
            }
        }

        DbRow IEnumerator<DbRow>.Current
        {
            get
            {
                return this.GetRow(index);
            }
        }
        #endregion

        public DbValue this[int column, int row]
        {
            get => this.GetValue(column, row);
        }

        public DbValue this[string column_name, int row]
        {
            get => this.GetValue(column_name, row);
        }

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

        public int RowsCount
        {
            get => this.Rows.Count;
        }

        public bool HasRows
        {
            get => this.Rows.Count == 0 ? false : true;
        }

        public DbResult(DbDataReader data)
        {
            this.ColumnTypes = new List<Type>();
            this.Columns = new List<string>();
            this.Rows = new List<List<object>>();
            if (data.HasRows)
            {
                while (data.Read())
                {
                    List<object> tmp_row = new List<object>();
                    int countCols = data.FieldCount;
                    for (int i = 0; i < countCols; i++)
                    {
                        if(this.Columns.Count < countCols)
                        {
                            this.Columns.Add(data.GetName(i));
                            this.ColumnTypes.Add(data.GetFieldType(i));
                        }
                        tmp_row.Add(data.GetValue(i));
                    }
                    this.Rows.Add(tmp_row);
                }
            }
            data.Close();
        }

        public object GetObject(int column, int row)
        {
            return this.Rows[row][column];
        }

        public object GetObject(string column_name, int row)
        {
            int column = this.Columns.IndexOf(column_name);
            return this.Rows[row][column];
        }

        public DbValue GetValue(int column, int row)
        {
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column]);
        }

        public DbValue GetValue(string column_name, int row)
        {
            int column = this.Columns.IndexOf(column_name);
            return new DbValue(this.ColumnTypes[column], this.Rows[row][column]);
        }

        public DbRow GetRow(int row)
        {
            return new DbRow(row, this.Rows[row], this.Columns, this.ColumnTypes);
        }

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

        public DbColumn GetColumn(string column_name)
        {
            int tmp_col_index = this.Columns.IndexOf(column_name);
            Type type = this.GetColumnType(tmp_col_index);
            List<object> tmp_values = new List<object>();
            foreach(List<object> tmp_row in this.Rows)
            {
                tmp_values.Add(tmp_row[tmp_col_index]);
            }
            return new DbColumn(column_name, type, tmp_values);
        }

        public Type GetColumnType(int column)
        {
            return this.ColumnTypes[column];
        }

        public Type GetColumnType(string column_name)
        {
            int column = this.Columns.IndexOf(column_name);
            return this.ColumnTypes[column];
        }
    }
}
