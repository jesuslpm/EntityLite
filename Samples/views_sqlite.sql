CREATE VIEW OrderDetailsSummary
AS
	SELECT
		OD.OrderID, SUM(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS OrderTotal, COUNT(*) AS LineCount
	FROM
		[OrderDetails] OD
	GROUP BY
		OD.OrderID;

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
		[Orders] O
		LEFT OUTER JOIN Customers C
			ON O.CustomerID = C.CustomerID
		LEFT OUTER JOIN Employees E
			ON O.EmployeeID = E.EmployeeID
		LEFT OUTER JOIN Shippers S
			ON O.ShipVia = S.ShipperID
		LEFT OUTER JOIN  OrderDetailsSummary OS
			ON O.OrderID = OS.OrderID;
      
CREATE VIEW OrderDetail_Detailed
AS
	SELECT
		OD.[OrderDetailID], OD.[OrderID], OD.[ProductID], OD.[UnitPrice], OD.Quantity, OD.Discount,
		(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS SubTotal,
		P.ProductName, C.CategoryName
	FROM
		[OrderDetails] OD
		INNER JOIN Products P ON OD.ProductID = P.ProductID
		INNER JOIN Categories C ON P.CategoryID = C.CategoryID;
   

CREATE VIEW Product_Detailed
AS
	SELECT 
		P.ProductID, P.ProductName, P.SupplierID, P.CategoryID, P.QuantityPerUnit, P.UnitPrice, 
		P.UnitsInStock, P.UnitsOnOrder, P.ReorderLevel, P.Discontinued,
		C.CategoryName
	FROM
		[Products] P
		LEFT OUTER JOIN [Categories] C
			ON P.CategoryID = C.CategoryID;