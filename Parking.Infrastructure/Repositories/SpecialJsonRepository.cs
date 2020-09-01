using System.Collections.Generic;
using System.Threading.Tasks;
using Parking.Domain.Models;
using Parking.Infrastructure.Interfaces;
using Parking.Infrastructure.Helpers;

namespace Parking.Infrastructure.Repositories
{
    public class SpecialJsonRepository : ISpecialRepository
    {
        public async Task<IEnumerable<Special>> SelectAllAsync()
        {
            var path = "special.json";
            var data = FileHelper.Read(FileHelper.PersistancePath + path);
            var result = JsonHelper.Deserialise<IEnumerable<Special>>(data);

            return result;
        }
    }
}
