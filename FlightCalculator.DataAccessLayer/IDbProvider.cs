using System.Data;

namespace FlightCalculator.DataAccessLayer
{
    public interface IDbProvider
    {
        IDbConnection Create();
    }
}
