using inercya.EntityLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Samples.Entities
{

    public class OrderDetailSearchCriteria
    {

        public int? OrderId { get; set;}
        public DateTime? FromDate { get; set;}
        public DateTime? ToDate { get; set;}
        public Decimal? MinPrice { get; set;}
        public Decimal? MaxPrice { get; set;}
        public string CustomerId { get; set;}
        public string CustomerName { get; set;}
        public string City { get; set;}
        public string Region { get; set;}
        public string Country { get; set;}
        public int? ProductId { get; set;}
        public string ProductName { get; set;}

    }

    public partial class OrderDetailRepository
    {
        public IQueryLite<OrderDetail> SearchQuery(OrderDetailSearchCriteria criteria)
        {
            var query = this.Query(Projection.Extended);
            if (criteria.City != null) query.And(OrderDetailFields.City, OperatorLite.Equals, criteria.City);
            if (criteria.Country != null) query.And(OrderDetailFields.Country, OperatorLite.Equals, criteria.Country);
            if (criteria.CustomerId != null) query.And(OrderDetailFields.CustomerId, OperatorLite.Equals, criteria.CustomerId);
            if (criteria.CustomerName != null) query.And(OrderDetailFields.CustomerName, OperatorLite.StartsWith, criteria.CustomerName);
            if (criteria.FromDate.HasValue) query.And(OrderDetailFields.OrderDate, OperatorLite.GreaterOrEquals, criteria.FromDate);
            if (criteria.MaxPrice.HasValue) query.And(OrderDetailFields.UnitPrice, OperatorLite.LessOrEquals, criteria.MaxPrice);
            if (criteria.MinPrice.HasValue) query.And(OrderDetailFields.UnitPrice, OperatorLite.GreaterOrEquals, criteria.MinPrice);
            if (criteria.OrderId.HasValue) query.And(OrderDetailFields.OrderId, OperatorLite.Equals, criteria.OrderId);
            if (criteria.ProductId.HasValue) query.And(OrderDetailFields.ProductId, OperatorLite.Equals, criteria.ProductId);
            if (criteria.ProductName != null) query.And(OrderDetailFields.ProductName, OperatorLite.StartsWith, criteria.ProductName);
            if (criteria.Region != null) query.And(OrderDetailFields.Region, OperatorLite.Equals, criteria.Region);
            if (criteria.ToDate.HasValue) query.And(OrderDetailFields.OrderDate, OperatorLite.LessOrEquals, criteria.ToDate);
            return query;
        }
    }
}
