using System;
using System.Collections.Generic;
using System.Linq;

namespace Generics.Tables
{
    public class Table<TRow, TColumn, TValue>
    {
        private readonly Dictionary<TRow, Dictionary<TColumn, TValue>> _data = new();
        private readonly HashSet<TRow> _rows = new();
        private readonly HashSet<TColumn> _columns = new();

        public IReadOnlySet<TRow> Rows => _rows;
        public IReadOnlySet<TColumn> Columns => _columns;

        public void AddRow(TRow row) => _rows.Add(row);
        public void AddColumn(TColumn column) => _columns.Add(column);

        public OpenIndexer Open => new(this);
        public ExistedIndexer Existed => new(this);

        public class OpenIndexer
        {
            private readonly Table<TRow, TColumn, TValue> _table;

            public OpenIndexer(Table<TRow, TColumn, TValue> table)
            {
                _table = table;
            }

            public TValue this[TRow row, TColumn column]
            {
                get
                {
                    if (_table._data.TryGetValue(row, out var rowData)
                        && rowData.TryGetValue(column, out var value))
                        return value;
                    return default;
                }
                set
                {
                    _table._rows.Add(row);
                    _table._columns.Add(column);

                    if (!_table._data.ContainsKey(row))
                        _table._data[row] = new Dictionary<TColumn, TValue>();

                    _table._data[row][column] = value;
                }
            }
        }

        public class ExistedIndexer
        {
            private readonly Table<TRow, TColumn, TValue> _table;

            public ExistedIndexer(Table<TRow, TColumn, TValue> table)
            {
                _table = table;
            }

            public TValue this[TRow row, TColumn column]
            {
                get
                {
                    if (!_table._rows.Contains(row))
                        throw new ArgumentException($"Row {row} does not exist");
                    if (!_table._columns.Contains(column))
                        throw new ArgumentException($"Column {column} does not exist");

                    if (_table._data.TryGetValue(row, out var rowData)
                        && rowData.TryGetValue(column, out var value))
                        return value;
                    return default;
                }
                set
                {
                    if (!_table._rows.Contains(row))
                        throw new ArgumentException($"Row {row} does not exist");
                    if (!_table._columns.Contains(column))
                        throw new ArgumentException($"Column {column} does not exist");

                    if (!_table._data.ContainsKey(row))
                        _table._data[row] = new Dictionary<TColumn, TValue>();

                    _table._data[row][column] = value;
                }
            }
        }
    }
}