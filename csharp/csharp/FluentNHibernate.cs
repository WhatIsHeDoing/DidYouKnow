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
            public virtual string Name { get; set; }
            public virtual Store Store { get; set; }
        }

        public class Store
        {
            public virtual int Id { get; protected set; }
            public virtual string Name { get; set; }
            public virtual IList<Employee> Staff { get; set; }

            public Store()
            {
                Staff = new List<Employee>();
            }

            public virtual Store AddEmployee(Employee employee)
            {
                employee.Store = this;
                Staff.Add(employee);
                return this;
            }
        }

        public class EmployeeMap : ClassMap<Employee>
        {
            public EmployeeMap()
            {
                Id(x => x.Id);
                Map(x => x.Name);
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
            }
        }

        public const string dbFile = "firstProject.db";

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
                .Database
                (SQLiteConfiguration.Standard.UsingFile(dbFile))
                .Mappings(m => m.FluentMappings
                    .AddFromAssembly(Assembly.GetExecutingAssembly()))
                .ExposeConfiguration(BuildSchema)
                .BuildSessionFactory();
        }

        private static void BuildSchema(Configuration config)
        {
            // delete the existing db on each run
            if (File.Exists(dbFile))
            {
                File.Delete(dbFile);
            }

            // this NHibernate tool takes a configuration
            // (with mapping info in) and exports a database schema from it
            new SchemaExport(config).Create(false, true);
        }

        [TestMethod]
        public void TestDatabase()
        {
            var sessionFactory = CreateSessionFactory();

            using (var session = sessionFactory.OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    session.SaveOrUpdate(new Store { Name = "Bargin Basin" }
                        .AddEmployee(new Employee { Name = "Daisy" })
                        .AddEmployee(new Employee { Name = "Jack" }));

                    transaction.Commit();
                }

                using (session.BeginTransaction())
                {
                    var stores = session
                        .CreateCriteria(typeof(Store)).List<Store>();

                    Assert.AreEqual(stores.Count, 1, "store found");

                    Assert.AreEqual
                        (stores[0].Staff.Count, 2, "store has 2 employees");
                }
            }
        }
    }
}
