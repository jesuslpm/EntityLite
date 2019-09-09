using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
    public enum SaveResult
    {
        NotModified,
        Updated,
        Inserted
    }

    public class SaveCollectionResult<TEntity>
    {
        public List<TEntity> Updated { get; set; }
        public List<TEntity> Inserted { get; set; }
        public List<TEntity> Deleted { get; set; }
    }
}
