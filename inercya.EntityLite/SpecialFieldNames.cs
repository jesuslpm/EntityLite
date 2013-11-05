using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite
{
	public class SpecialFieldNames
	{
		public string CreatedDateFieldName { get; set; }
		public string ModifiedDateFieldName { get; set; }
		public string CreatedByFieldName { get; set; }
		public string ModifiedByFieldName { get; set; }
		public string EntityRowVersionFieldName { get; set; }

		public SpecialFieldNames()
		{
			this.CreatedDateFieldName = "CreatedDate";
			this.ModifiedDateFieldName = "ModifiedDate";
			this.CreatedByFieldName = "CreatedBy";
			this.ModifiedByFieldName = "ModifiedBy";
			this.EntityRowVersionFieldName = "EntityRowVersion";
		}
	}
}
