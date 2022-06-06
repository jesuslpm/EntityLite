/*
Copyright 2014 i-nercya intelligent software

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace inercya.EntityLite
{
    public class SortDescriptor
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

    }
}
