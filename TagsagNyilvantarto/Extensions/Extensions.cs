using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TagsagNyilvantarto.Extensions
{
    public static class Extensions
    {
        public static IEnumerable<TSource> GetValues<TSource>(this DataTable dataTable, string colName)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
                return new List<TSource>();

            IEnumerable<DataRow> rows = dataTable.Rows.Cast<DataRow>(); // vagy dataTable.AsEnumerable();
            if (dataTable.Columns[colName].DataType == typeof(TSource))
            {
                return rows.Select(r => r.Field<TSource>(colName));
            }

            throw new MissingFieldException("Field not found in the table with this name and type.", colName);
        }


        public static IEnumerable<TSource> GetDistinctValues<TSource>(this DataTable dataTable, string colName) => dataTable.GetValues<TSource>(colName).Distinct();

        public static IList<string> GetColumNames(this DataTable dataTable)
        {
            IList<string> colNames = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                colNames.Add(col.ColumnName);
            }

            return colNames;
        }
    }
}
