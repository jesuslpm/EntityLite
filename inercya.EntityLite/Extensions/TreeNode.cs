using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace inercya.EntityLite.Extensions
{
	[Serializable]
	[DataContract]
	public class TreeNode<T>
	{
		[XmlIgnore]
		public TreeNode<T> Parent { get; set; }
		[DataMember]
		public T Entity { get; set; }
		[DataMember]
		public List<TreeNode<T>> Children { get; set; }

		public TreeNode()
		{
			Children = new List<TreeNode<T>>();
		}
	}
}
