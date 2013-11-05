using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace inercya.EntityLite
{
    [Serializable]
    public class SortDescriptor : ISerializable
    {
        public string FieldName { get; set; }
        public SortOrder SortOrder { get; set; }

        public SortDescriptor()
        {
            this.SortOrder = SortOrder.Ascending;
        }

        public SortDescriptor(string fieldName, SortOrder sortOrder)
        {
            this.FieldName = fieldName;
            this.SortOrder = sortOrder;
        }

        public SortDescriptor(string fieldName)
        {
            this.FieldName = fieldName;
            this.SortOrder = SortOrder.Ascending;
        }

        #region ISerializable Members

        protected SortDescriptor(SerializationInfo info, StreamingContext context)
        {
            this.FieldName = info.GetString("FN");
            this.SortOrder = (System.Data.SqlClient.SortOrder)info.GetInt32("SO");
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FN", this.FieldName);
            info.AddValue("SO", (int)this.SortOrder);
        }

        #endregion
    }
}
