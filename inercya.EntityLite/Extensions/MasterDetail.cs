using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inercya.EntityLite.Extensions
{
	public class MasterDetail<M, D>
	{
		public M Master { get; set; }
		public IList<D> Details { get; set; }
	}

	public class MasterDetail<M, D1, D2>
	{
		public M Master { get; set; }
		public IList<D1> Details1 { get; set; }
		public IList<D2> Details2 { get; set; }
	}

}
