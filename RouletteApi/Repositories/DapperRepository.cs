using Dapper;
using RouletteApi.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace RouletteApi.Repositories
{
    public abstract class DapperRepository : IDapperRepository
    {
        private string _connectionString;

        public DapperRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        private static Dictionary<Type, DbType> DAPPERDBTYPEMAP = new Dictionary<Type, DbType>()
        {
            [typeof(byte)] = DbType.Byte,
            [typeof(sbyte)] = DbType.SByte,
            [typeof(short)] = DbType.Int16,
            [typeof(ushort)] = DbType.UInt16,
            [typeof(int)] = DbType.Int32,
            [typeof(uint)] = DbType.UInt32,
            [typeof(long)] = DbType.Int64,
            [typeof(ulong)] = DbType.UInt64,
            [typeof(float)] = DbType.Single,
            [typeof(double)] = DbType.Double,
            [typeof(decimal)] = DbType.Decimal,
            [typeof(bool)] = DbType.Boolean,
            [typeof(string)] = DbType.String,
            [typeof(char)] = DbType.StringFixedLength,
            [typeof(Guid)] = DbType.Guid,
            [typeof(DateTime)] = DbType.DateTime,
            [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
            [typeof(TimeSpan)] = DbType.Time,
            [typeof(byte[])] = DbType.Binary,
            [typeof(byte?)] = DbType.Byte,
            [typeof(sbyte?)] = DbType.SByte,
            [typeof(short?)] = DbType.Int16,
            [typeof(ushort?)] = DbType.UInt16,
            [typeof(int?)] = DbType.Int32,
            [typeof(uint?)] = DbType.UInt32,
            [typeof(long?)] = DbType.Int64,
            [typeof(ulong?)] = DbType.UInt64,
            [typeof(float?)] = DbType.Single,
            [typeof(double?)] = DbType.Double,
            [typeof(decimal?)] = DbType.Decimal,
            [typeof(bool?)] = DbType.Boolean,
            [typeof(char?)] = DbType.StringFixedLength,
            [typeof(Guid?)] = DbType.Guid,
            [typeof(DateTime?)] = DbType.DateTime,
            [typeof(DateTimeOffset?)] = DbType.DateTimeOffset,
            [typeof(TimeSpan?)] = DbType.Time,
            [typeof(object)] = DbType.Object
        };

        /// <summary>
        /// HACK: Get the DBType for Dapper DynamicParameters
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private DbType LookupDBType(Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null) type = underlyingType;
            if (type.IsEnum && !DAPPERDBTYPEMAP.ContainsKey(type))
            {
                type = Enum.GetUnderlyingType(type);
            }
            if (DAPPERDBTYPEMAP.TryGetValue(type, out DbType dbType))
            {
                return dbType;
            }

            return DbType.Object;
        }

        public void AddDapperDynamicParameter<T>(DynamicParameters dbParameters, string paramName, bool IsDirectionOut, T paramObject)
        {
            if (paramObject == null)
                dbParameters.Add(paramName);
            else
                dbParameters.Add(paramName, paramObject, LookupDBType(typeof(T)), IsDirectionOut ? ParameterDirection.Output : ParameterDirection.Input);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters parameters = null)// where T : class
        {
            return await QueryAsync<T>(query, CommandType.StoredProcedure, parameters);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string query, CommandType commandType, DynamicParameters parameters = null)// where T : class
        {
            using (var db = new SqlConnection(_connectionString))
            {
                if (parameters != null)
                {
                    var result = await db.QueryAsync<T>(query, param: parameters, commandType: commandType);
                    return result;
                }
                else
                {
                    var result = await db.QueryAsync<T>(query, commandType: commandType);
                    return result;
                }
            }
        }

        public async Task ExecuteAsync(string query, DynamicParameters parameters = null)
        {
            await ExecuteAsync(query, CommandType.StoredProcedure, parameters);
        }

        public async Task ExecuteAsync(string query, CommandType commandType, DynamicParameters parameters = null)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                if (parameters != null)
                {
                    await db.ExecuteAsync(query, param: parameters, commandType: commandType);
                }
                else
                {
                    await db.ExecuteAsync(query, commandType: commandType);
                }
            }
        }

        public async Task<(int ReturnValue, DynamicParameters Parameters)> ExecuteOutParamAsync(string query, DynamicParameters parameters)
        {
            using (var db = new SqlConnection(_connectionString))
            {
                var result = await db.ExecuteAsync(query, param: parameters, commandType: CommandType.StoredProcedure);
                return (result, parameters);
            }

        }
    }
}
