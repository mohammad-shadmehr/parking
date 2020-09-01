using System.Collections.Generic;
using System.Threading.Tasks;

namespace Parking.Infrastructure.Interfaces
{
    public interface IRepository<T>
    {
        Task<IEnumerable<T>> SelectAllAsync();
    }
}
