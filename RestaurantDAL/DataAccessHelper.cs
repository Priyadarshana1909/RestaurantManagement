using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace RestaurantDAL
{
    public class DataAccessHelper
    {
        #region "Common functions"
        public static List<T> ConvertToList<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName.ToLower())
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : row[pro.Name]);
                    }

                    return objT;
                }).ToList();
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static IEnumerable<T> ConvertToEnumerable<T>(DataTable dt)
        {
            try
            {
                var columnNames = dt.Columns.Cast<DataColumn>()
                    .Select(c => c.ColumnName.ToLower())
                    .ToList();

                var properties = typeof(T).GetProperties();

                return dt.AsEnumerable().Select(row =>
                {
                    var objT = Activator.CreateInstance<T>();

                    foreach (var pro in properties)
                    {
                        if (columnNames.Contains(pro.Name.ToLower()))
                            pro.SetValue(objT, row[pro.Name] == DBNull.Value ? null : row[pro.Name]);
                    }

                    return objT;
                }).AsEnumerable();
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion
    }
}
