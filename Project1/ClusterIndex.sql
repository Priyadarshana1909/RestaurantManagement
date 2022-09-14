----Create Nonclustered index on all the foreign keys of each table. Also include other non-referential columns of the
----tables as included columns

DROP INDEX NC_Order_RestaurantID ON [ORDER];
CREATE NONCLUSTERED INDEX NC_Order_RestaurantID ON [ORDER] (RestaurantID) INCLUDE (OrderDate, ItemQuantity, OrderAmount);

DROP INDEX NC_Order_MenuItemID ON [ORDER];
CREATE NONCLUSTERED INDEX NC_Order_MenuItemID ON [ORDER] (MenuItemID) INCLUDE (OrderDate, ItemQuantity, OrderAmount);

DROP INDEX NC_Order_DiningTableID ON [ORDER];
CREATE NONCLUSTERED INDEX NC_Order_DiningTableID ON [ORDER] (DiningTableID) INCLUDE (OrderDate, ItemQuantity, OrderAmount);

DROP INDEX NC_Bills_RestaurantID ON BILLS;
CREATE NONCLUSTERED INDEX NC_Bills_RestaurantID ON BILLS (RestaurantID) INCLUDE (BillAmount);

DROP INDEX NC_Bills_CustomerID ON BILLS;
CREATE NONCLUSTERED INDEX NC_Bills_CustomerID ON BILLS (CustomerID) INCLUDE (BillAmount);

DROP INDEX  NC_Bills_OrderID ON BILLS;
CREATE NONCLUSTERED INDEX NC_Bills_OrderID ON BILLS (OrderID) INCLUDE (BillAmount);

DROP INDEX  NC_Cuisine_RestaurantID ON Cuisine;
CREATE NONCLUSTERED INDEX NC_Cuisine_RestaurantID ON Cuisine (RestaurantID) INCLUDE (CuisineName);

DROP INDEX  NC_Customer_RestaurantID ON Customer;
CREATE NONCLUSTERED INDEX NC_Customer_RestaurantID ON Customer (RestaurantID) INCLUDE (CustomerName,MobileNo);

DROP INDEX  NC_DiningTable_RestaurantID ON DiningTable;
CREATE NONCLUSTERED INDEX NC_DiningTable_RestaurantID ON DiningTable (RestaurantID) INCLUDE ([Location]);

DROP INDEX  NC_DiningTableTrack_DiningTableID ON DiningTableTrack;
CREATE NONCLUSTERED INDEX NC_DiningTableTrack_DiningTableID ON DiningTableTrack (DiningTableID) INCLUDE (TableStatus);

DROP INDEX  NC_RestaurantMenuItem_CuisineID ON RestaurantMenuItem;
CREATE NONCLUSTERED INDEX NC_RestaurantMenuItem_CuisineID ON RestaurantMenuItem (CuisineID) INCLUDE (ItemName,ItemPrice);
