using Core.Common.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Common
{
    public class SpecificationEvaluter<T> where T : class
    {

        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // Apply criteria
            if (spec.Criteria is not null)
            {
                query = query.Where(spec.Criteria);
            }

            // Apply Includes 
            query = spec.Includes.Aggregate(query,
                (currentQuery, includeExpression) => currentQuery.Include(includeExpression));


            // Apply OrderBy
            if (spec.OrderBy is not null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending is not null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            foreach (var includeString in spec.IncludeStrings)
                query = query.Include(includeString);

            if (spec.AsNoTracking)
                query = query.AsNoTracking();

            return query;
        }

    }
}
