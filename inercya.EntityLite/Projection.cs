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
