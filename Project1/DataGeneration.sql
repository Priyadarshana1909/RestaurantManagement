------(1) This sp holds type the Insert, Update, Delete of table Restaurant


--Declare @ID INT
--EXEC USP_Restaurant @RestaurantID = 3, @RestaurantName = 'Chocalate House', @Address = '2 Maninagar Ahmedabad', @MobileNo = '5134567890', 
--@IsDelete = 0, @OutpurRestaurantId  = @ID OUTPUT
--SELECT @ID AS 'Record Number Affected'
--GO

--Select * from Restaurant

--EXEC sys.sp_cdc_enable_table  
--@source_schema = N'dbo',  
--@source_name   = N'Employee_Main',  
--@role_name     = NULL,  
--@filegroup_name = NULL,  
--@supports_net_changes = 0 
--GO

--Select * From Restaurant
--sp_helptext USP_Restaurant
CREATE OR ALTER PROCEDURE USP_Restaurant (
                                @RestaurantID INT,
                                @RestaurantName NVARCHAR(200),
								@Address NVARCHAR(500),
								@MobileNo NVARCHAR(10),
								@IsDelete BIT,
								@OutpurRestaurantId INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @RestaurantID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid restaurant id',11,1); 
			END
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete
				IF (LEN(ISNULL(@MobileNo,'')) <> 10)
				BEGIN
					RAISERROR ('Mobile number should be exactly 10 digits.',11,1,@MobileNo); 
				END;

				IF (LEN(ISNULL(@Address,'')) < 10)
				BEGIN
					RAISERROR ('Address should be atleast more than 10 characters.',11,1,@Address); 
				END;

				IF (ISNUMERIC(LEFT(ISNULL(@Address,''),1)) = 0)
				BEGIN
				   RAISERROR ('1st letter of the address column should be numeric only.',11,1,@Address); 
				END;

				IF(ISNULL(@RestaurantName,'') = '')
				BEGIN
				   RAISERROR ('Restaurant Name should not be null.',11,1,@RestaurantName); 
				END;

			END

			IF (@IsDelete = 0 AND @RestaurantID = 0)
			BEGIN
			   -- Add Operation
				INSERT INTO Restaurant
								(RestaurantName,
									[Address],
									MobileNo)
					VALUES     ( @RestaurantName,
									@Address,
									@MobileNo);
			END;
			ELSE IF (@IsDelete = 0 AND @RestaurantID > 0)
			BEGIN
			-- Update Operation
					UPDATE Restaurant
						SET RestaurantName = @RestaurantName,
							[Address] = @Address,
							MobileNo = @MobileNo                   
						WHERE RestaurantID  = @RestaurantID;
			END;
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Restaurant WHERE RestaurantID = @RestaurantID;
			END;
	
			SELECT @OutpurRestaurantId = @@IDENTITY;
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END;
GO


------(2) This sp holds type the Insert, Update, Delete of table cuisine served by Cuisine

Declare @ID INT
EXEC USP_Cuisine @CuisineID = 0,@RestaurantID =3,@CuisineName='Pleasure Point',
@IsDelete = 0, @OutputCuisineId  = @ID OUTPUT
SELECT @ID AS 'Record Number Affected'
GO


--select * from Cuisine
--update Cuisine set RestaurantID= 7 where CuisineID=12

CREATE OR ALTER PROCEDURE USP_Cuisine (
								@CuisineID INT,
                                @RestaurantID INT,
                                @CuisineName NVARCHAR(50),								
								@IsDelete BIT,
								@OutputCuisineId INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @CuisineID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid Cuisine id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete				
		
				IF(ISNULL(@CuisineName,'') = '')
				BEGIN
				   RAISERROR ('Cuisine Name should not be null.',11,1,@CuisineName); 
				END;

			END
			IF (@IsDelete = 0 AND @CuisineID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Cuisine
								(RestaurantID,
									CuisineName)
					VALUES     ( @RestaurantID,
									@CuisineName);
			END;
			ELSE IF (@IsDelete = 0 AND @CuisineID > 0)
			BEGIN
			-- Update Operation
					UPDATE Cuisine
						SET RestaurantID = @RestaurantID,
							CuisineName = @CuisineName            
						WHERE CuisineID  = @CuisineID;
			END;
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Cuisine WHERE CuisineID  = @CuisineID;
			END;
			SELECT @OutputCuisineId = @@IDENTITY;
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END;
GO
------(3) This sp holds type the Insert, Update, Delete of table RestaurantMenuItem

Declare @ID INT
EXEC USP_RestaurantMenuItem @MenuItemID = 0, @CuisineID = 4,@ItemName = 'Dessert',@ItemPrice = 117.9,
@IsDelete = 0, @OutputMenuItemID  = @ID OUTPUT
SELECT @ID AS 'Record Number affected'
GO

--select * from RestaurantMenuItem
--update Cuisine set RestaurantID= 7 where CuisineID=12


CREATE OR ALTER PROCEDURE USP_RestaurantMenuItem (
								@MenuItemID INT,
                                @CuisineID INT,
                                @ItemName NVARCHAR(100),	
								@ItemPrice FLOAT,	
								@IsDelete BIT,
								@OutputMenuItemID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @MenuItemID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid MenuItem id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete				
				IF(ISNULL(@ItemName, '') = '')
				BEGIN
				   RAISERROR ('Item Name should not be null.',11,1,@ItemName); 
				END;

				IF(@ItemPrice <= 0)
				BEGIN
				   RAISERROR ('Item Price should be greater than 0.',11,1); 
				END;

			END
			IF (@IsDelete = 0 AND @MenuItemID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO RestaurantMenuItem
								(CuisineID,
									ItemName,
									ItemPrice)
					VALUES     ( @CuisineID,
									@ItemName,
									@ItemPrice);
			END;
			ELSE IF (@IsDelete = 0 AND @MenuItemID > 0)
			BEGIN
			-- Update Operation
					UPDATE RestaurantMenuItem
						SET CuisineID = @CuisineID,
							ItemName = @ItemName,
							ItemPrice = @ItemPrice
						WHERE MenuItemID  = @MenuItemID;
			END;
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM RestaurantMenuItem WHERE MenuItemID  = @MenuItemID;
			END;
	
			SELECT @OutputMenuItemId = @@IDENTITY;
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END;
GO

------(4) This sp holds type the Insert, Update, Delete of table DiningTable 

--Declare @ID INT
--EXEC USP_DiningTable @DiningTableID = 4, @RestaurantID = 3,@Location = 'MG Palace',
--@IsDelete = 0, @OutputDiningTableID  = @ID OUTPUT
--SELECT @ID AS 'Record Number affected'
--GO

--select * from DiningTable
--update DiningTable set DiningTableID= 3 where CuisineID=12

--Insert into DiningTable Values (2,'Kadi')


CREATE OR ALTER PROCEDURE USP_DiningTable (
								@DiningTableID INT,
                                @RestaurantID INT,
                                @Location NVARCHAR(100),									
								@IsDelete BIT,
								@OutputDiningTableID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @DiningTableID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid DiningTable id',11,1); 
			END;

			IF (@IsDelete = 0 AND @DiningTableID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO DiningTable
								(RestaurantID,
									[Location])
					VALUES     ( @RestaurantID,
									@Location);
				
				SELECT @OutputDiningTableID = @@IDENTITY;

				INSERT INTO DiningTableTrack (DiningTableID, TableStatus)
				VALUES (@OutputDiningTableID, 'Vacant');

			END;
			ELSE IF (@IsDelete = 0 AND @DiningTableID > 0)
			BEGIN
			-- Update Operation
					UPDATE DiningTable
						SET RestaurantID = @RestaurantID,
							[Location] = @Location
						WHERE DiningTableID  = @DiningTableID;

						SELECT @OutputDiningTableID = @@IDENTITY;
			END;
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM DiningTable WHERE DiningTableID  = @DiningTableID;
				SELECT @OutputDiningTableID = @@IDENTITY;
			END;
	
			
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END
GO

------This Function holds type the Select query of RestaurantMenuItem where it returns the ItemPrice of respective MenuItemID

CREATE OR ALTER FUNCTION [dbo].[UDF_GetItemPrice] (
    @MenuItemID INT
)
RETURNS FLOAT AS
BEGIN
    Declare @ItemPrice FLOAT = 0;
	SELECT @ItemPrice = ItemPrice From  RestaurantMenuItem WHERE MenuItemId = @MenuItemID
	
    RETURN ISNULL(@ItemPrice,0);
END
GO


--Declare @ID INT
--EXEC USP_Order @OrderID = 17, @RestaurantID = 3, @MenuItemID = 4, @ItemQuantity = 15, 
--@OrderAmount = 250,@DiningTableID= 6,
--@IsDelete = 1
--SELECT @ID AS 'number of records affected'
--GO

--Select * From DiningTable
--Select * from RestaurantMenuItem
--Select * from [Order]
--Select * from Cuisine

------(5) This sp holds type the Insert, Update, Delete of table Order


--Select * From Restaurant
--sp_helptext USP_Restaurant

--Select * from [Order]

--EXEC USP_Order
--EXEC USP_Order @OrderID = 7, @RestaurantID = 1, @MenuItemID = 7, @ItemQuantity = 44, 
--@OrderAmount = 343,@DiningTableID= 2,
--@IsDelete = 0


CREATE OR ALTER PROCEDURE USP_Order (
                                @OrderID INT,                                
								@RestaurantID INT,
								@MenuItemID INT,
								@ItemQuantity INT,
								@OrderAmount FLOAT,
								@DiningTableID INT,
								@IsDelete INT,
								@OrderDate DATETIME = NULL,
								@RowCount INT = NULL,
								@OutputId INT = NULL
								)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION
	
			IF(@IsDelete = 1 AND @OrderID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid order id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete
				IF (@OrderDate IS NULL)
				BEGIN
					Set @OrderDate = GETDATE();
				END

				IF (@ItemQuantity <= 0)
				BEGIN
					RAISERROR ('ItemQuantity should be greater than 0',11,1,@ItemQuantity); 
				END;		

			END;

			IF (@IsDelete = 0 AND @OrderID = 0)
			BEGIN
			
				INSERT INTO [Order]
								(RestaurantID,
									OrderDate,
									MenuItemID,
									ItemQuantity,
									OrderAmount,
									DiningTableID)
					VALUES     ( @RestaurantID,
									GETDATE(),
									@MenuItemID,
									@ItemQuantity,
									[dbo].[UDF_GetItemPrice](@MenuItemID) * @ItemQuantity,
									@DiningTableID);

				 Update DiningTableTrack Set TableStatus = 'Occupied'
				 WHERE DiningTableID = @DiningTableID;
			END
			ELSE IF (@IsDelete = 0 AND @OrderID > 0)
			BEGIN
			-- Update Operation
					UPDATE [Order]
						SET RestaurantID = @RestaurantID,
							OrderDate = @OrderDate,
							MenuItemID = @MenuItemID,
							ItemQuantity = @ItemQuantity,
							OrderAmount = [dbo].[UDF_GetItemPrice](@MenuItemID) * @ItemQuantity,
							DiningTableID = @DiningTableID
						WHERE OrderID  = @OrderID 
						AND OrderID NOT IN (Select OrderID From Bills);

						SELECT @RowCount = @@RowCount;
						IF(@RowCount = 0)
						BEGIN
							RAISERROR ('Cannot Allow Update/Delete on this OrderId, Bills are already processed on these OrderID',11,1,@OrderID );
						END;
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM [Order] WHERE OrderID = @OrderID 
									AND OrderID NOT IN (Select OrderID From Bills);
				SELECT @RowCount = @@RowCount;
				IF(@RowCount = 0)
				BEGIN
					RAISERROR ('Cannot Allow Update/Delete on this OrderId,Bills are already processed on these OrderID',11,1,@OrderID ); 
				END;
			END;
	
			--SELECT @OutputId = @@IDENTITY
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	

	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END
GO
------(6) This sp holds type the Insert, Update, Delete of table Customer 

--Declare @ID INT
--EXEC USP_Customer @CustomerID = 0, @RestaurantID ='3', @CustomerName = 'Priyanka Parmer ',@MobileNo = '8538978242',
--@IsDelete = 0
--SELECT @ID AS 'number of records affected'
--GO

--select * from RestaurantMenuItem
--update Cuisine set RestaurantID= 7 where CuisineID=12

--Select * from Customer


CREATE OR ALTER PROCEDURE USP_Customer (
								@CustomerID INT,
                                @RestaurantID INT,
                                @CustomerName NVARCHAR(100),	
								@MobileNo NVARCHAR(10),	
								@IsDelete BIT,								
								@OutputID INT = NULL)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION

			IF(@IsDelete = 1 AND @CustomerID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid CustomerID id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
				-- in case of add / delete				
				IF(Len(ISNULL(@CustomerName,'')) < 10)
				BEGIN
				   RAISERROR ('CustomerName should have atleast 10 characters.',11,1,@CustomerName); 
				END;

				If((@CustomerName LIKE '%[!@#$%^*()_-+=:?<>]%') OR (@CustomerName LIKE '%[0-9]%'))
				BEGIN
				   RAISERROR ('No special character, no integer is used for CustomerName..',11,1,@CustomerName); 
				END;

				IF (LEN(ISNULL(@MobileNo,'')) <> 10)
				BEGIN
					RAISERROR ('Mobile number should be exactly 10 digits.',11,1,@MobileNo); 
				END;

			END
			IF (@IsDelete = 0 AND @CustomerID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Customer
								(RestaurantID,
									CustomerName,
									MobileNo)
					VALUES     ( @RestaurantID,
									@CustomerName,
									@MobileNo);
			END
			ELSE IF (@IsDelete = 0 AND @CustomerID > 0)
			BEGIN
			-- Update Operation
					UPDATE Customer
						SET RestaurantID = @RestaurantID,
							CustomerName = @CustomerName,
							MobileNo = @MobileNo
						WHERE CustomerID  = @CustomerID
						AND CustomerID NOT IN (Select CustomerID from Bills);
					SELECT @OutputId = @@ROWCOUNT
			END
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Customer WHERE CustomerID  = @CustomerID
									 AND CustomerID NOT IN (Select CustomerID from Bills);
				SELECT @OutputId = @@ROWCOUNT
			END
			IF(@OutputID = 0)
			BEGIN
				RAISERROR('Data cannot be allowed to be updated / deleted AS Customer is already used in billing.',11,1);
			END;
			
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END
GO

------THIS TABLE FUNCTION HOLDS THE TYPE OF SELECT QUERY WHERE IT RETURNS THE TABLE RECORD OF ORDER OF RESPECTIVE ORDERID

CREATE OR ALTER FUNCTION [dbo].[UDP_GetOrderDetails]
(
  @OrderID INT
)
RETURNS TABLE
AS
RETURN
(
SELECT OrderDate,
	   RestaurantID,
       MenuItemID,
       ItemQuantity,
       OrderAmount,
       DiningTableID
       FROM dbo.[Order]
       WHERE OrderID = @OrderID);
GO

------(7) This sp holds type the Insert, Update, Delete of table BILLS
--SELECT * FROM Bills
CREATE OR ALTER PROCEDURE USP_Billis (
								@BillsID INT,
                                @OrderID INT,
								@RestaurantID INT,
								@BillAmount FLOAT,
								@CustomerID INT,
                               	@IsDelete BIT,
								@OutputID INT OUTPUT)								
AS
BEGIN
BEGIN TRY
	BEGIN TRANSACTION
	        
			DECLARE @DiningTableID INT
			IF(@IsDelete = 1 AND @BillsID <= 0)
			BEGIN
				-- in case of delete
				RAISERROR ('Invalid Bills Id',11,1); 
			END;
			ELSE IF (@IsDelete = 0)
			BEGIN
			    IF(@OrderID <= 0)
				BEGIN
					RAISERROR ('Invalid Order Id',11,1); 
				END;

			    SELECT @BillAmount = OrderAmount,
				@RestaurantID = RestaurantID,
				@DiningTableID = DiningTableID
				FROM [dbo].[UDP_GetOrderDetails](@OrderID);

			END;
			IF (@IsDelete = 0 AND @BillsID = 0)
			 BEGIN
			   -- Add Operation
				INSERT INTO Bills (OrderID,RestaurantID, BillAmount, CustomerID)
					VALUES ( @OrderID,@RestaurantID, @BillAmount, @CustomerID);
				
				Update DiningTableTrack Set TableStatus = 'Occupied'
				 WHERE DiningTableID = @DiningTableID;
					
			END;
			ELSE IF (@IsDelete = 0 AND @BillsID > 0)
			BEGIN
			-- Update Operation
					UPDATE Bills
						SET OrderID = @OrderID,
							RestaurantID = @RestaurantID,
							BillAmount = @BillAmount,
							CustomerID = @CustomerID
						WHERE BillsID  = @BillsID;

						SELECT @OutputId = @@IDENTITY;
			END;
			ELSE IF (@IsDelete = 1)
			BEGIN
				DELETE FROM Bills WHERE BillsID  = @BillsID;

				SELECT @OutputId = @@IDENTITY;
			END;
	
			SELECT @OutputId = @@IDENTITY;
	 COMMIT TRANSACTION
END TRY
BEGIN CATCH
	DECLARE 
		@ErrorMessage  NVARCHAR(4000), 
		@ErrorSeverity INT, 
		@ErrorState    INT;

		SELECT 
			@ErrorMessage = ERROR_MESSAGE(), 
			@ErrorSeverity = ERROR_SEVERITY(), 
			@ErrorState = ERROR_STATE();

		-- return the error inside the CATCH block
		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		
		-- Transaction uncommittable
		IF (XACT_STATE()) = -1
		  ROLLBACK TRANSACTION
 
	-- Transaction committable
		IF (XACT_STATE()) = 1
		  COMMIT TRANSACTION

END CATCH;

END
GO