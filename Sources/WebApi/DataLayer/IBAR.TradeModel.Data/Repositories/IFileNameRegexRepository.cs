using IBAR.TradeModel.Data.Entities;
using System.Collections.Generic;
using System.Linq;

namespace IBAR.TradeModel.Data.Repositories
{
    public interface IFileNameRegexRepository
    {
        IEnumerable<string> GetFilePatterns();

        IQueryable<FileNameRegex> FileNameRegexesQuery();
    }

    public class FileNameRegexRepository : IFileNameRegexRepository
    {
        private readonly TradeModelContext _dbContext;

        public FileNameRegexRepository(TradeModelContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<string> GetFilePatterns()
        {
            return _dbContext.FileNameRegexes.Select(x => x.FileRegex);
        }

        public IQueryable<FileNameRegex> FileNameRegexesQuery()
        {
            return _dbContext.FileNameRegexes;
        }
    }
}
