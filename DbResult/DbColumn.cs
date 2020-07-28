using System.Collections;
using System.Collections.Generic;

namespace System.Data.Common
{
    public struct DbColumn : IEnumerable<DbValue>, IEnumerator<DbValue>
    {
        public string ColumnName { get; private set; }
        public Type ColumnType { get; private set; }
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

        public int RowsCount
        {
            get => this.Values.Count;
        }

        public DbValue this[int row]
        {
            get
            {
                return new DbValue(this.ColumnType, this.Values[row], row, this.ColumnName);
            }
        }

        internal DbColumn(string name, Type type, IEnumerable<object> values)
        {
            this.index = -1;
            this.ColumnName = name;
            this.ColumnType = type;
            this.Values = new List<object>(values);
        }

        public object GetObject(int row)
        {
            return this.Values[row];
        }

        public DbValue GetValue(int row)
        {
            return new DbValue(this.ColumnType, this.Values[row], row, this.ColumnName);
        }
    }
}
