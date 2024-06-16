CREATE VIEW [dbo].[Product_Detailed]
AS
SELECT
    P.ProductID, P.ProductName, P.SupplierID, P.CategoryID, P.QuantityPerUnit, P.UnitPrice,
    P.UnitsInStock, P.UnitsOnOrder, P.ReorderLevel, P.Discontinued,
    C.CategoryName, S.CompanyName AS SupplierName
FROM
    [dbo].[Products] P
    LEFT OUTER JOIN [dbo].[Categories] C
        ON P.CategoryID = C.CategoryID
    LEFT OUTER JOIN [dbo].Suppliers S
        ON P.SupplierID = S.SupplierID   


GO
CREATE VIEW [dbo].[OrderDetailsSummary]
WITH SCHEMABINDING
AS
    SELECT
        OD.OrderID, SUM(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS OrderTotal, COUNT_BIG(*) AS LineCount
    FROM
        [dbo].[Order Details] OD
    GROUP BY
        OD.OrderID    
GO

CREATE UNIQUE CLUSTERED INDEX [UK_OrderDetailsSummary_OrderID] 
ON [dbo].[OrderDetailsSummary]([OrderID])

GO

CREATE VIEW [dbo].[Order_Extended]
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

CREATE VIEW OrderDetail_Detailed
AS
	SELECT 
		OD.[OrderID], OD.[ProductID], OD.[UnitPrice], OD.[Quantity], OD.[Discount],
		P.ProductName, C.CategoryName, CONVERT(money, OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS SubTotal
	FROM
		[dbo].[Order Details] OD
		LEFT OUTER JOIN [dbo].[Products] P ON OD.ProductID = P.ProductID
		LEFT OUTER JOIN [dbo].[Categories] C ON P.CategoryID = C.CategoryID

GO

CREATE VIEW OrderDetail_Extended
AS
	SELECT 
		OD.[OrderID], OD.[ProductID], OD.[UnitPrice], OD.[Quantity], OD.[Discount],
		P.ProductName, C.CategoryName, CU.City, CU.Country, 
		O.CustomerID, CU.CompanyName AS CustomerName, CU.Region, O.OrderDate
	FROM
		[dbo].[Order Details] OD
		LEFT OUTER JOIN [dbo].[Products] P ON OD.ProductID = P.ProductID
		LEFT OUTER JOIN [dbo].[Categories] C ON P.CategoryID = C.CategoryID
		LEFT OUTER JOIN [dbo].[Orders] O ON OD.OrderID = O.OrderID
		LEFT OUTER JOIN [dbo].[Customers] CU ON O.CustomerID = CU.CustomerID
GO

CREATE FUNCTION GetEmployeeSubTree(@EmployeeId int)
RETURNS TABLE
AS
RETURN
WITH H
AS
(
    SELECT E.EmployeeID, E.LastName, E.FirstName,  E.ReportsTo, E.City
    FROM
        [dbo].[Employees] E
    WHERE
        E.EmployeeID = @EmployeeId
    UNION ALL
    SELECT E.EmployeeID, E.LastName, E.FirstName,  E.ReportsTo, E.City
    FROM
        [dbo].[Employees] E
        INNER JOIN H ON E.ReportsTo = H.EmployeeID
)
SELECT * FROM H

GO

CREATE VIEW [dbo].[ProductSale_Quarter]
AS
    SELECT
        P.CategoryID, C.CategoryName, P.ProductID, P.ProductName,
        DATEPART(year, O.OrderDate) AS [Year],
        DATEPART(quarter, O.OrderDate) AS [Quarter],
        SUM(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS Sales
    FROM
        
        dbo.Products P
        LEFT OUTER JOIN dbo.Categories C
            ON P.CategoryID = C.CategoryID
        LEFT OUTER JOIN 
        (
            dbo.Orders O
            INNER JOIN [dbo].[Order Details] OD
                ON O.OrderID = OD.OrderID
        ) ON P.ProductID = OD.ProductID
    GROUP BY
        P.CategoryID, C.CategoryName, P.ProductID, P.ProductName,
        DATEPART(year, O.OrderDate),
        DATEPART(quarter, O.OrderDate)
GO

CREATE VIEW [dbo].[ProductSale_Year]
AS
SELECT
    P.ProductID, P.ProductName,
    DATEPART(year, O.OrderDate) AS [Year],
    SUM(OD.Quantity * OD.UnitPrice * (1 - OD.Discount)) AS Sales,
    COUNT(*) AS Orders
FROM
    dbo.Products P
    LEFT OUTER JOIN 
    (
        dbo.Orders O
        INNER JOIN [dbo].[Order Details] OD
        ON O.OrderID = OD.OrderID
    ) ON P.ProductID = OD.ProductID
GROUP BY
    P.ProductID, P.ProductName,
    DATEPART(year, O.OrderDate)
GO

SELECT * INTO OrderDetailsCopy
FROM [dbo].[Order Details]
WHERE 1 = 0


CREATE TABLE dbo.JsonItems 
(
	JsonItemId int NOT NULL IDENTITY(1,1) CONSTRAINT PK_JsonItems PRIMARY KEY,
	DataJson nvarchar(max)
)

CREATE TABLE dbo.DateItems
(
    DateItemId int NOT NULL IDENTITY(1,1) CONSTRAINT PK_DateItems PRIMARY KEY,
    [UtcDateTime] datetime,
    ItemDateTime datetime,
    ItemDate datetime
)
GO




