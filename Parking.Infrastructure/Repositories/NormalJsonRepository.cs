using System.Collections.Generic;
using System.Threading.Tasks;
using Parking.Domain.Models;
using Parking.Infrastructure.Interfaces;
using Parking.Infrastructure.Helpers;

namespace Parking.Infrastructure.Repositories
{
    public class NormalJsonRepository : INormalRepository
    {
        public async Task<IEnumerable<Normal>> SelectAllAsync()
        {
            var path = "normal.json";
            var data = FileHelper.Read(FileHelper.PersistancePath + path);
            var result = JsonHelper.Deserialise<IEnumerable<Normal>>(data);

            return result;
        }
    }
}
