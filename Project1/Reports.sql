--1) Create procedure to get the list of all currently vacant tables in restaurant. 
--   Procedure should accept RestaurantID as input parameter.


--Exec USP_VACANTTABLEINRESTAURANT 2

CREATE OR ALTER PROCEDURE  USP_VACANTTABLEINRESTAURANT (
											 @RestaurantID INT)
AS
BEGIN
			SELECT		R.RestaurantID,R.RestaurantName,
						D.DiningTableID 
			FROM		Restaurant R
			INNER JOIN
						DiningTable D
			ON 
						R.RestaurantID = D.RestaurantID
			INNER JOIN
						DiningTableTrack DT
			ON 
						D.DiningTableID = DT.DiningTableID
			Where 
						DT.TableStatus = 'Vacant'
			AND			R.RestaurantID = @RestaurantID;
    
END
GO

--2) Create procedure to get the list of all customers, Order Details and Table they used for dining. 
--   Procedure should be written dynamically so that filters can be applied.
--   a. Procedure should accept @FilterBy & @OrderBy variable to filter and order the data dynamically.

--Exec USP_GetCustomerDynamically @CustomerID = 3
--EXEC USP_GetCustomerDynamically @ItemQuantity1 =  7,@ItemQuantity2 =8

--EXEC USP_GetCustomerDynamically @ItemQuantity1 =  5,@ItemQuantity2 =5
--EXEC USP_GetCustomerDynamically @OrderAmount1 = 64.5, @OrderAmount2 = 700

--EXEC USP_GetCustomerDynamically @OrderDateFrom = '2020-08-21', @OrderDateTo = '2022-07-21'

--EXEC USP_GetCustomerDynamically @OrderAmount1 = 40, @OrderAmount2 = 700, @SortBy = 'OrderDate', @SortDirection = ' DESC'

CREATE OR ALTER PROCEDURE USP_GetCustomerDynamically ( 
		@CustomerID		INT = NULL,
		@CustomerName	NVARCHAR(100) = NULL,
		@RestaurantID	INT = NULL,
		@DiningTableID	INT = NULL,
		@OrderID		INT = NULL,
		@Location		NVARCHAR(100) = NULL,
		@ItemQuantity1	INT = NULL,
		@ItemQuantity2	INT = NULL,
		@OrderAmount1	FLOAT = NULL,
		@OrderAmount2	FLOAT = NULL,
		@OrderDate		DATE = NULL,
		@OrderDateFrom	DATE = NULL,
		@OrderDateTo	DATE = NULL,
		@SortBy NVARCHAR(50) = NULL,
		@SortDirection NVARCHAR(10) = ' ASC'
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE 
		@SQLQUERY		NVARCHAR(MAX),
		@ParamDefinition AS NVarchar(2000),  
		@lCustomerID	INT,
		@lCustomerName	NVARCHAR(100),
		@lRestaurantID	INT,
		@lDiningTableID INT,
		@lOrderID		INT,
		@lLocation		NVARCHAR(100),
		@lItemQuantity1	INT,
		@lItemQuantity2	INT,
		@lOrderAmount1	FLOAT,
		@lOrderAmount2	FLOAT,
		@lOrderDate		DATE,
		@lOrderDateFrom DATE,
		@lOrderDateTo	DATE
		

	SET @SQLQUERY = 'SELECT		C.CustomerID,C.CustomerName,D.RestaurantID,D.DiningTableID,D.[Location], 
								O.OrderID,O.ItemQuantity,O.OrderAmount,O.OrderDate  from Bills B 
					INNER JOIN	Customer C
					ON			B.CustomerID =  C.CustomerID
					INNER JOIN	[Order] O
					ON			O.OrderID = B.OrderID
					INNER JOIN	DiningTable D
					ON			O.DiningTableID = D.DiningTableID
					WHERE		1=1'
		Print @SQLQUERY;
		IF @CustomerID IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND  C.CustomerID =@lCustomerID';				
		END;
		IF @CustomerName IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND  C.CustomerName =@lCustomerName';				
		END;
		IF @RestaurantID IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND  D.RestaurantID = @lRestaurantID';				
		END;
		IF @DiningTableID IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND  D.DiningTableID = @lDiningTableID';				
		END;
		IF @OrderID IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND   O.OrderID = @lOrderID';				
		END;
		IF @Location IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND  D.Location = @lLocation';				
		END;
		IF @ItemQuantity1 IS NOT NULL OR @ItemQuantity2 IS NOT NULL
		BEGIN
				IF @ItemQuantity1 IS NOT NULL AND @ItemQuantity2 IS NULL
				BEGIN
					SET @SQLQUERY = @SQLQUERY + ' AND O.ItemQuantity = @lItemQuantity1';				
				END;
				ELSE
				BEGIN
					SET @SQLQUERY = @SQLQUERY + ' AND  O.ItemQuantity BETWEEN @lItemQuantity1 AND @lItemQuantity2';				
				END;
		END
		IF @OrderAmount1 IS NOT NULL OR @OrderAmount2 IS NOT NULL
		BEGIN
				IF @OrderAmount1 IS NOT NULL AND @OrderAmount2 IS NULL
				BEGIN
					SET @SQLQUERY = @SQLQUERY + ' AND  O.OrderAmount = @lOrderAmount1';
				END;
				ELSE
				BEGIN
					SET @SQLQUERY = @SQLQUERY + ' AND  O.OrderAmount BETWEEN @lOrderAmount1 AND @lOrderAmount2';
				END;
		END
		Print @lOrderDate
		IF @OrderDate IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND   FORMAT(O.OrderDate,' + '''yyyy-MM-dd''' + ') = @lOrderDate';				
		END;
		IF @OrderDateFrom IS NOT NULL OR @OrderDateTo IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' AND   FORMAT(O.OrderDate,' + '''yyyy-MM-dd''' + ') BETWEEN @lOrderDateFROM AND @lOrderDateTO';				
		END;

		
		IF @SortBy IS NOT NULL
		BEGIN
				SET @SQLQUERY = @SQLQUERY + ' ORDER BY '  + @SortBy + @SortDirection;
		END; 
		
	PRINT @OrderDate;
	
	PRINT @SQLQUERY;
	SET	  @ParamDefinition = N'@lCustomerID INT, 
							 @lCustomerName NVARCHAR(100),
							 @lRestaurantID INT,
							 @lDiningTableID INT,
							 @lOrderID INT,
							 @lLocation NVARCHAR(100),
							 @lItemQuantity1 INT,
							 @lItemQuantity2 INT,
							 @lOrderAmount1	FLOAT,
							 @lOrderAmount2	FLOAT,							 
							 @lOrderDate DATE,
							 @lOrderDateFrom DATE,
							 @lOrderDateTo DATE'  
	EXECUTE sp_Executesql   @SQLQuery,
							@ParamDefinition,
							@lCustomerID = @CustomerID,
							@lCustomerName = @CustomerName,
							@lRestaurantID	= @RestaurantID,
							@lDiningTableID = @DiningTableID,
							@lOrderID		= @OrderID,
							@lLocation		= @Location,
							@lItemQuantity1	= @ItemQuantity1,
							@lItemQuantity2	= @ItemQuantity2,
							@lOrderAmount1  = @OrderAmount1,
							@lOrderAmount2  = @OrderAmount2,
							@lOrderDate     = @OrderDate,
							@lOrderDateFrom = @OrderDateFrom,
							@lOrderDateTo   = @OrderDateTo
END;
GO

--3) Create a procedure to display Restaurantwise, Yearwise total order amount. 

--Exec USP_GetRestaurantProfitByYear 'OrderDate'

CREATE OR ALTER PROCEDURE  USP_GetRestaurantProfitByYear (
											 @ListBy nVarchar(10))
AS
BEGIN
	IF(@ListBy = 'Restaurant')
	BEGIN
			SELECT		R.RestaurantID,R.RestaurantName,Sum(O.OrderAmount) AS TotalAmount 
			FROM		Restaurant R
			INNER JOIN
						[Order] O
			ON			R.RestaurantID = O.RestaurantID
			Group By 
						R.RestaurantID,R.RestaurantName
			Order BY
						R.RestaurantID;
	END;
	IF(@ListBy = 'OrderDate' )
	BEGIN
			SELECT  R.RestaurantID,R.RestaurantName,
					CAST(O.OrderDate AS DATE) AS OrdersDate,
					YEAR(CAST(O.OrderDate AS DATE)) [year],
					Sum(O.OrderAmount) as TotalAmount 
			FROM 
					Restaurant R
			INNER JOIN 
					[Order] O
			ON		R.RestaurantID = O.RestaurantID
			Group By	
					CAST(O.OrderDate AS DATE),R.RestaurantID,R.RestaurantName
			Order BY
					CAST(O.OrderDate AS DATE);

		--Select  CAST(O.OrderDate AS DATE) AS OrdersDate,
		--		YEAR(CAST(O.OrderDate AS DATE)) [year],
		--		Sum(O.OrderAmount) as TotalAmount  
		--FROM   [Order] O 
		--Group By	
		--		CAST(O.OrderDate AS DATE)
	END;
    
END
GO

--4) Create procedure to list daywise, tablewise total order amount.

--Exec USP_GetAmountByDayAndTable 'Table'

CREATE OR ALTER PROCEDURE  USP_GetAmountByDayAndTable (
											 @ListBy nVarchar(10))
AS
BEGIN
	IF(@ListBy = 'Table')
	BEGIN
			SELECT		CAST(OrderDate AS DATE) AS OrdersDate,YEAR(CAST(OrderDate AS DATE)) [year],
						Sum(OrderAmount) as TotalAmount 
			FROM		[Order]
			GROUP BY	CAST(OrderDate AS DATE);
	END
	IF(@ListBy = 'OrderDate' )
	BEGIN
			SELECT		CAST(OrderDate AS DATE) AS OrdersDate,DAY(CAST(OrderDate AS DATE)) [DAY],
						Sum(OrderAmount) as TotalAmount 
			FROM		[Order]
			GROUP BY	CAST(OrderDate AS DATE)
			ORDER BY	[DAY];
	END
    
END
GO

--5) Create view to list cusinewise item details.

--SELECT * FROM VCuisineList

CREATE OR ALTER VIEW VCuisineList
As
	SELECT		C.CuisineID,C.CuisineName,I.ItemName,I.ItemPrice 
	FROM		Cuisine C
	INNER JOIN
				RestaurantMenuItem I
	ON 
				C.CuisineID = I.CuisineID
	Order By	C.CuisineID
	OFFSET 0 ROWS;
Go

