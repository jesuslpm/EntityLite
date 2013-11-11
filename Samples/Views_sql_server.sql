USE Norhtwind
GO

CREATE VIEW OrderDetail_Detailed
AS
	SELECT
		OD.[OrderDetailID], OD.[OrderID], OD.[ProductID], OD.[UnitPrice], OD.Quantity, OD.Discount,
		(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS SubTotal,
		P.ProductName, C.CategoryName
	FROM
		[dbo].[OrderDetails] OD
		INNER JOIN dbo.Products P ON OD.ProductID = P.ProductID
		INNER JOIN dbo.Categories C ON P.CategoryID = C.CategoryID

GO

CREATE VIEW OrderDetailsSummary
WITH SCHEMABINDING
AS
	SELECT
		OD.OrderID, SUM(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS OrderTotal, COUNT_BIG(*) AS LineCount
	FROM
		[dbo].[OrderDetails] OD
	GROUP BY
		OD.OrderID

GO

CREATE UNIQUE CLUSTERED INDEX UK_OrderDetailsSummary_OrderID
ON OrderDetailsSummary(OrderID)

GO
CREATE VIEW Order_Extended
AS
	SELECT 
		O.OrderID, O.CustomerID, O.EmployeeID, O.OrderDate, O.RequiredDate, O.ShippedDate, O.ShipVia, 
		O.Freight, O.ShipName, O.ShipAddress, O.ShipCity, O.ShipRegion, O.ShipPostalCode, O.ShipCountry,
		C.CompanyName AS CustomerCompanyName,
		E.FirstName AS EmployeeFirstName, E.LastName AS EmployeeLastName,
		S.CompanyName AS ShipperCompanyName,
		OS.OrderTotal, OS.LineCount
	FROM
		[dbo].[Orders] O
		LEFT OUTER JOIN dbo.Customers C
			ON O.CustomerID = C.CustomerID
		LEFT OUTER JOIN dbo.Employees E
			ON O.EmployeeID = E.EmployeeID
		LEFT OUTER JOIN dbo.Shippers S
			ON O.ShipVia = S.ShipperID
		LEFT OUTER JOIN  dbo.OrderDetailsSummary OS WITH (NOEXPAND)
			ON O.OrderID = OS.OrderID
GO

CREATE VIEW Product_Detailed
AS
	SELECT 
		P.ProductID, P.ProductName, P.SupplierID, P.CategoryID, P.QuantityPerUnit, P.UnitPrice, 
		P.UnitsInStock, P.UnitsOnOrder, P.ReorderLevel, P.Discontinued,
		C.CategoryName
	FROM
		[dbo].[Products] P
		LEFT OUTER JOIN [dbo].[Categories] C
			ON P.CategoryID = C.CategoryID
GO