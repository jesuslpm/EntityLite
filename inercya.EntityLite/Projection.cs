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

namespace inercya.EntityLite
{
    public enum Projection
    {
        Minimal,
        Basic,
        Detailed,
        Extended,
        MinimalLocalized,
        BasicLocalized,
        DetailedLocalized,
        ExtendedLocalized,
		BaseTable
    }

    public static class ProjectionExtender
    {
        public static string GetProjectionName(this Projection projection)
        {
            switch (projection)
            {
                case Projection.Minimal: return "Minimal";
                case Projection.Basic: return "Basic";
                case Projection.Detailed: return "Detailed";
                case Projection.Extended: return "Extended";
                case Projection.BasicLocalized: return "BasicLocalized";
                case Projection.DetailedLocalized: return "DetailedLocalized";
                case Projection.ExtendedLocalized: return "ExtendedLocalized";
                case Projection.MinimalLocalized: return  "MinimalLocalized"; 
				case Projection.BaseTable: return "BaseTable";
                
                default:
                    throw new NotSupportedException("Projection: " + projection.ToString() + " not supported");

            }
        }
    }
}
