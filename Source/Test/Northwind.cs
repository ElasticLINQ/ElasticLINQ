// Copyright (c) Microsoft Corporation.  All rights reserved.
// This source code is made available under the terms of the Microsoft Public License (MS-PL)

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.IO;
using System.Runtime.Serialization;

namespace Test
{
    using IQToolkit;
    using IQToolkit.Data;
    using IQToolkit.Data.Mapping;

    public class Customer
    {
        public string CustomerID;
        public string ContactName;
        public string CompanyName;
        public string Phone;
        public string City;
        public string Country;
        public IList<Order> Orders;
    }

    public class Order
    {
        public int OrderID;
        public string CustomerID;
        public DateTime OrderDate;
        public Customer Customer;
        public List<OrderDetail> Details;
    }

    public class OrderDetail
    {
        public int? OrderID { get; set; }
        public int ProductID { get; set; }
        public Product Product;
    }

    public interface IEntity
    {
        int ID { get; }
    }

    public class Product : IEntity
    {
        public int ID;
        public string ProductName;
        public bool Discontinued;

        int IEntity.ID
        {
            get { return this.ID; }
        }
    }

    public class Employee
    {
        public int EmployeeID;
        public string LastName;
        public string FirstName;
        public string Title;
        public Address Address;
    }

    public class Address
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string Region { get; private set; }
        public string PostalCode { get; private set; }

        public Address(string street, string city, string region, string postalCode)
        {
            this.Street = street;
            this.City = city;
            this.Region = region;
            this.PostalCode = postalCode;
        }
    }

    public class Northwind
    {
        private IEntityProvider provider;

        public Northwind(IEntityProvider provider)
        {
            this.provider = provider;
        }

        public IEntityProvider Provider
        {
            get { return this.provider; }
        }

        public virtual IEntityTable<Customer> Customers
        {
            get { return this.provider.GetTable<Customer>("Customers"); }
        }

        public virtual IEntityTable<Order> Orders
        {
            get { return this.provider.GetTable<Order>("Orders"); }
        }

        public virtual IEntityTable<OrderDetail> OrderDetails
        {
            get { return this.provider.GetTable<OrderDetail>("OrderDetails"); }
        }

        public virtual IEntityTable<Product> Products
        {
            get { return this.provider.GetTable<Product>("Products"); }
        }

        public virtual IEntityTable<Employee> Employees
        {
            get { return this.provider.GetTable<Employee>("Employees"); }
        }
    }

    public class NorthwindWithAttributes : Northwind
    {
        public NorthwindWithAttributes(IEntityProvider provider)
            : base(provider)
        {
        }

        [Table]
        [Column(Member = "CustomerId", IsPrimaryKey = true)]
        [Column(Member = "ContactName")]
        [Column(Member = "CompanyName")]
        [Column(Member = "Phone")]
        [Column(Member = "City", DbType="NVARCHAR(20)")]
        [Column(Member = "Country")]
        [Association(Member = "Orders", KeyMembers = "CustomerID", RelatedEntityID = "Orders", RelatedKeyMembers = "CustomerID")]
        public override IEntityTable<Customer> Customers
        {
            get { return base.Customers; }
        }
        
        [Table]
        [Column(Member = "OrderID", IsPrimaryKey = true, IsGenerated = true)]
        [Column(Member = "CustomerID")]
        [Column(Member = "OrderDate")]
        [Association(Member = "Customer", KeyMembers = "CustomerID", RelatedEntityID = "Customers", RelatedKeyMembers = "CustomerID")]
        [Association(Member = "Details", KeyMembers = "OrderID", RelatedEntityID = "OrderDetails", RelatedKeyMembers = "OrderID")]
        public override IEntityTable<Order> Orders
        {
            get { return base.Orders; }
        }

        [Table(Name = "Order Details")]
        [Column(Member = "OrderID", IsPrimaryKey = true)]
        [Column(Member = "ProductID", IsPrimaryKey = true)]
        [Association(Member = "Product", KeyMembers = "ProductID", RelatedEntityID = "Products", RelatedKeyMembers = "ID")]
        public override IEntityTable<OrderDetail> OrderDetails
        {
            get { return base.OrderDetails; }
        }

        [Table]
        [Column(Member = "Id", Name="ProductId", IsPrimaryKey = true)]
        [Column(Member = "ProductName")]
        [Column(Member = "Discontinued")]
        public override IEntityTable<Product> Products
        {
            get { return base.Products; }
        }

        [Table]
        [Column(Member = "EmployeeID", IsPrimaryKey = true)]
        [Column(Member = "LastName")]
        [Column(Member = "FirstName")]
        [Column(Member = "Title")]
        [Column(Member = "Address.Street", Name = "Address")]
        [Column(Member = "Address.City")]
        [Column(Member = "Address.Region")]
        [Column(Member = "Address.PostalCode")]
        public override IEntityTable<Employee> Employees
        {
            get { return base.Employees; }
        }
    }

    public class MySqlNorthwind : NorthwindWithAttributes
    {
        public MySqlNorthwind(IEntityProvider provider)
            : base(provider)
        {
        }

        [Table(Name = "Order_Details")]
        public override IEntityTable<OrderDetail> OrderDetails
        {
            get { return base.OrderDetails; }
        }
    }

    public interface INorthwindSession
    {
        void SubmitChanges();
        ISessionTable<Customer> Customers { get; }
        ISessionTable<Order> Orders { get; }
        ISessionTable<OrderDetail> OrderDetails { get; }
    }

    public class NorthwindSession : INorthwindSession
    {
        IEntitySession session;

        public NorthwindSession(EntityProvider provider)
            : this(new EntitySession(provider))
        {
        }

        public NorthwindSession(IEntitySession session)
        {
            this.session = session;
        }

        public IEntitySession Session
        {
            get { return this.session; }
        }

        public void SubmitChanges()
        {
            this.session.SubmitChanges();
        }

        public ISessionTable<Customer> Customers
        {
            get { return this.session.GetTable<Customer>("Customers"); }
        }

        public ISessionTable<Order> Orders
        {
            get { return this.session.GetTable<Order>("Orders"); }
        }

        public ISessionTable<OrderDetail> OrderDetails
        {
            get { return this.session.GetTable<OrderDetail>("OrderDetails"); }
        }
    }


    public class CustomerX
    {
        public CustomerX(string customerId, string contactName, string companyName, string phone, string city, string country, List<OrderX> orders)
        {
            this.CustomerID = customerId;
            this.ContactName = contactName;
            this.CompanyName = companyName;
            this.Phone = phone;
            this.City = city;
            this.Country = country;
            this.Orders = orders;
        }

        public string CustomerID { get; private set; }
        public string ContactName { get; private set; }
        public string CompanyName { get; private set; }
        public string Phone { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public List<OrderX> Orders { get; private set; }
    }

    public class OrderX
    {
        public OrderX(int orderID, string customerID, DateTime orderDate, CustomerX customer)
        {
            this.OrderID = orderID;
            this.CustomerID = customerID;
            this.OrderDate = orderDate;
            this.Customer = customer;
        }

        public int OrderID { get; private set; }
        public string CustomerID { get; private set; }
        public DateTime OrderDate { get; private set; }
        public CustomerX Customer { get; private set; }
    }

    public class NorthwindX
    {
        EntityProvider provider;

        public NorthwindX(EntityProvider provider)
        {
            this.provider = provider;
        }

        [Table]
        [Column(Member = "CustomerId", IsPrimaryKey = true)]
        [Column(Member = "ContactName")]
        [Column(Member = "CompanyName")]
        [Column(Member = "Phone")]
        [Column(Member = "City", DbType = "NVARCHAR(20)")]
        [Column(Member = "Country")]
        [Association(Member = "Orders", KeyMembers = "CustomerID", RelatedEntityID = "Orders", RelatedKeyMembers = "CustomerID")]
        public IQueryable<CustomerX> Customers
        {
            get { return this.provider.GetTable<CustomerX>("Customers"); }
        }

        [Table]
        [Column(Member = "OrderID", IsPrimaryKey = true, IsGenerated = true)]
        [Column(Member = "CustomerID")]
        [Column(Member = "OrderDate")]
        [Association(Member = "Customer", KeyMembers = "CustomerID", RelatedEntityID = "Customers", RelatedKeyMembers = "CustomerID")]
        public IEntityTable<OrderX> Orders
        {
            get { return this.provider.GetTable<OrderX>("Orders"); }
        }
    }
}
