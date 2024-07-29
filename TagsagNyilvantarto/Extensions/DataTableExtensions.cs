using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TagsagNyilvantarto.Extensions;

public static class DataTableExtensions
{
    public static IEnumerable<TSource> GetValuesFromColumn<TSource>(this DataTable dataTable, string colName)
    {
        if (dataTable is null || dataTable.Rows.Count == 0)
            return Array.Empty<TSource>();

        if (dataTable.Columns[colName].DataType == typeof(TSource))
            return dataTable.AsEnumerable().Select(r => r.Field<TSource>(colName));

        throw new MissingFieldException("Field was not found in the table with this name and type.", colName);
    }

    public static IEnumerable<TSource> GetDistinctValuesFromColumn<TSource>(this DataTable dataTable, string colName) => dataTable.GetValuesFromColumn<TSource>(colName).Distinct();

    public static IReadOnlyList<string> GetColumNames(this DataTable dataTable)
    {
        if (dataTable is null)
            return Array.Empty<string>();

        var colNames = new List<string>();
        foreach (DataColumn col in dataTable.Columns)
            colNames.Add(col.ColumnName);

        return colNames.AsReadOnly();
    }

    public static DataView ToDataView(this DataTable dataTable) => new(dataTable);
}
