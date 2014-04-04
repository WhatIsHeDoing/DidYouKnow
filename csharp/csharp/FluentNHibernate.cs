using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using FluentNHibernate.Mapping;
using NHibernate;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using System.Reflection;
using NHibernate.Cfg;
using System.IO;
using NHibernate.Tool.hbm2ddl;
using System.Data.SQLite;

namespace csharp
{
    [TestClass]
    public class FluentNHibernate
    {
        public class Employee
        {
            public virtual int Id { get; protected set; }
            public virtual string FirstName { get; set; }
            public virtual string LastName { get; set; }
            public virtual Store Store { get; set; }
        }

        public class Product
        {
            public virtual int Id { get; protected set; }
            public virtual string Name { get; set; }
            public virtual double Price { get; set; }
            public virtual IList<Store> StoresStockedIn { get; protected set; }

            public Product()
            {
                StoresStockedIn = new List<Store>();
            }
        }

        public class Store
        {
            public virtual int Id { get; protected set; }
            public virtual string Name { get; set; }
            public virtual IList<Product> Products { get; set; }
            public virtual IList<Employee> Staff { get; set; }

            public Store()
            {
                Products = new List<Product>();
                Staff = new List<Employee>();
            }

            public virtual void AddProduct(Product product)
            {
                product.StoresStockedIn.Add(this);
                Products.Add(product);
            }

            public virtual void AddEmployee(Employee employee)
            {
                employee.Store = this;
                Staff.Add(employee);
            }
        }

        public class EmployeeMap : ClassMap<Employee>
        {
            public EmployeeMap()
            {
                Id(x => x.Id);
                Map(x => x.FirstName);
                Map(x => x.LastName);
                References(x => x.Store);
            }
        }

        public class StoreMap : ClassMap<Store>
        {
            public StoreMap()
            {
                Id(x => x.Id);
                Map(x => x.Name);

                HasMany(x => x.Staff)
                  .Inverse()
                  .Cascade.All();

                HasManyToMany(x => x.Products)
                 .Cascade.All()
                 .Table("StoreProduct");
            }
        }

        public class ProductMap : ClassMap<Product>
        {
            public ProductMap()
            {
                Id(x => x.Id);
                Map(x => x.Name);
                Map(x => x.Price);

                HasManyToMany(x => x.StoresStockedIn)
                  .Cascade.All()
                  .Inverse()
                  .Table("StoreProduct");
            }
        }

        public static void AddProductsToStore(Store store, params Product[] products)
        {
            foreach (var product in products)
            {
                store.AddProduct(product);
            }
        }

        public static void AddEmployeesToStore(Store store, params Employee[] employees)
        {
            foreach (var employee in employees)
            {
                store.AddEmployee(employee);
            }
        }

        public const string dbFile = "firstProject.db";

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(
                SQLiteConfiguration.Standard
                  .UsingFile(dbFile)
              )
              .Mappings(m =>
                m.FluentMappings.AddFromAssembly(Assembly.GetExecutingAssembly()))
              .ExposeConfiguration(BuildSchema)
              .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(dbFile))
                File.Delete(dbFile);

            // this NHibernate tool takes a configuration (with mapping info in)
            // and exports a database schema from it
            new SchemaExport(config)
              .Create(false, true);
        }

        public void Main()
        {
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    // create a couple of Stores each with some Products and Employees
                    var barginBasin = new Store { Name = "Bargin Basin" };
                    var superMart = new Store { Name = "SuperMart" };

                    var potatoes = new Product { Name = "Potatoes", Price = 3.60 };
                    var fish = new Product { Name = "Fish", Price = 4.49 };
                    var milk = new Product { Name = "Milk", Price = 0.79 };
                    var bread = new Product { Name = "Bread", Price = 1.29 };
                    var cheese = new Product { Name = "Cheese", Price = 2.10 };
                    var waffles = new Product { Name = "Waffles", Price = 2.41 };

                    var daisy = new Employee { FirstName = "Daisy", LastName = "Harrison" };
                    var jack = new Employee { FirstName = "Jack", LastName = "Torrance" };
                    var sue = new Employee { FirstName = "Sue", LastName = "Walkters" };
                    var bill = new Employee { FirstName = "Bill", LastName = "Taft" };
                    var joan = new Employee { FirstName = "Joan", LastName = "Pope" };

                    // add products to the stores, there's some crossover in the products in each
                    // store, because the store-product relationship is many-to-many
                    AddProductsToStore(barginBasin, potatoes, fish, milk, bread, cheese);
                    AddProductsToStore(superMart, bread, cheese, waffles);

                    // add employees to the stores, this relationship is a one-to-many, so one
                    // employee can only work at one store at a time
                    AddEmployeesToStore(barginBasin, daisy, jack, sue);
                    AddEmployeesToStore(superMart, bill, joan);

                    // save both stores, this saves everything else via cascading
                    session.SaveOrUpdate(barginBasin);
                    session.SaveOrUpdate(superMart);

                    transaction.Commit();
                }

                // retreive all stores and display them
                using (session.BeginTransaction())
                {
                    var stores = session.CreateCriteria(typeof(Store))
                      .List<Store>();

                    foreach (var store in stores)
                    {
                        //WriteStorePretty(store);
                    }
                }
            }
        }

        [TestMethod]
        public void TestMethod1()
        {
            Main();
        }
    }
}
