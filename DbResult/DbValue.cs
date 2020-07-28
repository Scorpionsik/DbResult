namespace System.Data.Common
{
    /// <summary>
    /// Элемент таблицы.
    /// </summary>
    public struct DbValue
    {
        public int RowIndex { get; private set; }

        public string ColumnName { get; private set; }

        /// <summary>
        /// Тип данных для этого элемента.
        /// </summary>
        public Type TypeValue { get; private set; }

        /// <summary>
        /// Значение, указанное в этом элементе.
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        internal DbValue(Type type, object obj, int row, string column)
        {
            this.RowIndex = row;
            this.ColumnName = column;
            this.TypeValue = type;
            this.Value = obj;
        }

        public int ToInt32()
        {
            if (this.TypeValue == typeof(int) || this.TypeValue == typeof(Int32))
            {
                return Convert.ToInt32(this.Value);
            }
            else throw new Exception("Not Int32");
        }

        public double ToDouble()
        {
            if (this.TypeValue == typeof(double) || this.TypeValue == typeof(Double))
            {
                return Convert.ToDouble(this.Value);
            }
            else throw new Exception("Not Double");
        }

        public bool ToBoolean()
        {
            if (this.TypeValue == typeof(bool) || this.TypeValue == typeof(SByte))
            {
                return Convert.ToBoolean(this.Value);
            }
            else throw new Exception("Not Boolean");
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
