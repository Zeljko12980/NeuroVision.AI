using Dapper;
using System.Data;
using System.Globalization;

namespace BuildingBlocks.Dapper
{
    internal static class DapperTypeHandlers
    {
        private static int registered;

        public static void EnsureRegistered()
        {
            if (Interlocked.Exchange(ref registered, 1) == 1)
            {
                return;
            }

            SqlMapper.RemoveTypeMap(typeof(TimeSpan));
            SqlMapper.RemoveTypeMap(typeof(TimeOnly));
            SqlMapper.AddTypeHandler(new TimeSpanHandler());
            SqlMapper.AddTypeHandler(new TimeOnlyHandler());
        }

        private sealed class TimeSpanHandler : SqlMapper.TypeHandler<TimeSpan>
        {
            public override void SetValue(IDbDataParameter parameter, TimeSpan value)
            {
                parameter.Value = value;
            }

            public override TimeSpan Parse(object value) => value switch
            {
                TimeSpan timeSpan => timeSpan,
                TimeOnly timeOnly => timeOnly.ToTimeSpan(),
                DateTime dateTime => dateTime.TimeOfDay,
                string text => TimeSpan.Parse(text, CultureInfo.InvariantCulture),
                _ => throw new DataException(
                    $"Cannot convert {value.GetType().FullName} to TimeSpan.")
            };
        }

        private sealed class TimeOnlyHandler : SqlMapper.TypeHandler<TimeOnly>
        {
            public override void SetValue(IDbDataParameter parameter, TimeOnly value)
            {
                parameter.Value = value;
            }

            public override TimeOnly Parse(object value) => value switch
            {
                TimeOnly timeOnly => timeOnly,
                TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
                DateTime dateTime => TimeOnly.FromTimeSpan(dateTime.TimeOfDay),
                string text => TimeOnly.Parse(text, CultureInfo.InvariantCulture),
                _ => throw new DataException(
                    $"Cannot convert {value.GetType().FullName} to TimeOnly.")
            };
        }
    }
}
