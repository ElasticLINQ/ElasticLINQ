using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IQToolkit;
using IQToolkit.Data;

namespace Test
{
    public class MultiTableTests : TestHarness
    {
        public static void Run(MultiTableContext db)
        {
            new MultiTableTests().RunTests(db, null, null, true);
        }

        public static void Run(MultiTableContext db, string testName)
        {
            new MultiTableTests().RunTest(db, null, true, testName);
        }

        protected MultiTableContext db;

        protected void RunTests(MultiTableContext db, string baselineFile, string newBaselineFile, bool executeQueries)
        {
            this.db = db;
            var provider = (DbEntityProvider)db.Provider;
            base.RunTests(provider, baselineFile, newBaselineFile, executeQueries);
        }

        protected void RunTest(MultiTableContext db, string baselineFile, bool executeQueries, string testName)
        {
            this.db = db;
            var provider = (DbEntityProvider)db.Provider;
            base.RunTest(provider, baselineFile, executeQueries, testName);
        }

        protected override void SetupTest()
        {
            this.CleaupDatabase();
        }

        protected override void TeardownTest()
        {
            this.CleaupDatabase();
        }

        private void CleaupDatabase()
        {
            ExecSilent("DELETE FROM TestTable3");
            ExecSilent("DELETE FROM TestTable2");
            ExecSilent("DELETE FROM TestTable1");
        }

        protected override void SetupSuite()
        {
            ExecSilent("DROP TABLE TestTable3");
            ExecSilent("DROP TABLE TestTable2");
            ExecSilent("DROP TABLE TestTable1");
            ExecSilent("CREATE TABLE TestTable1 (ID int IDENTITY(1) PRIMARY KEY, Value1 VARCHAR(10))");
            ExecSilent("CREATE TABLE TestTable2 (ID int PRIMARY KEY REFERENCES TestTable1(ID), Value2 VARCHAR(10))");
            ExecSilent("CREATE TABLE TestTable3 (ID int PRIMARY KEY REFERENCES TestTable1(ID), Value3 VARCHAR(10))");
        }

        protected override void TeardownSuite()
        {
            ExecSilent("DROP TABLE TestTable3");
            ExecSilent("DROP TABLE TestTable2");
            ExecSilent("DROP TABLE TestTable1");
        }

        public void TestInsert()
        {
            int id = 
                db.MultiTableEntities.Insert(
                    new MultiTableEntity
                    {
                        Value1 = "ABC",
                        Value2 = "DEF",
                        Value3 = "GHI"
                    },
                    m => m.ID
                );

            var entity = db.MultiTableEntities.SingleOrDefault(m => m.ID == id);
            AssertTrue(entity != null);
            AssertValue("ABC", entity.Value1);
            AssertValue("DEF", entity.Value2);
            AssertValue("GHI", entity.Value3);
        }

        public void TestInsertReturnId()
        {
            var id = 
                db.MultiTableEntities.Insert(
                    new MultiTableEntity
                    {
                        Value1 = "ABC",
                        Value2 = "DEF",
                        Value3 = "GHI"
                    },
                    m => m.ID
                );

            var entity = db.MultiTableEntities.SingleOrDefault(m => m.ID == id);
            AssertTrue(entity != null);
            AssertValue("ABC", entity.Value1);
            AssertValue("DEF", entity.Value2);
            AssertValue("GHI", entity.Value3);
        }

        public void TestInsertBatch()
        {
            var ids =
                db.MultiTableEntities.Batch(
                    new[] {
                        new MultiTableEntity
                        {
                            Value1 = "ABC",
                            Value2 = "DEF",
                            Value3 = "GHI"
                        },
                        new MultiTableEntity
                        {
                            Value1 = "123",
                            Value2 = "456",
                            Value3 = "789"
                        }
                    },
                    (u, m) => u.Insert(m, x => x.ID)
                ).ToList();

            var entity1 = db.MultiTableEntities.SingleOrDefault(m => m.ID == ids[0]);
            AssertTrue(entity1 != null);
            AssertValue("ABC", entity1.Value1);
            AssertValue("DEF", entity1.Value2);
            AssertValue("GHI", entity1.Value3);

            var entity2 = db.MultiTableEntities.SingleOrDefault(m => m.ID == ids[1]);
            AssertTrue(entity2 != null);
            AssertValue("123", entity2.Value1);
            AssertValue("456", entity2.Value2);
            AssertValue("789", entity2.Value3);
        }

        public void TestUpdate()
        {
            var id = 
                db.MultiTableEntities.Insert(
                    new MultiTableEntity
                    {
                        Value1 = "ABC",
                        Value2 = "DEF",
                        Value3 = "GHI"
                    },
                    m => m.ID
                );

            var nUpdated = 
                db.MultiTableEntities.Update(
                    new MultiTableEntity
                    {
                        ID = id,
                        Value1 = "123",
                        Value2 = "456",
                        Value3 = "789"
                    }
                    );

            AssertTrue(nUpdated == 3);

            var entity = db.MultiTableEntities.SingleOrDefault(m => m.ID == id);
            AssertTrue(entity != null);
            AssertValue("123", entity.Value1);
            AssertValue("456", entity.Value2);
            AssertValue("789", entity.Value3);
        }

        public void TestUpdateBatch()
        {
            var ids =
                db.MultiTableEntities.Batch(
                    new[] {
                        new MultiTableEntity
                        {
                            Value1 = "ABC",
                            Value2 = "DEF",
                            Value3 = "GHI"
                        },
                        new MultiTableEntity
                        {
                            Value1 = "123",
                            Value2 = "456",
                            Value3 = "789"
                        }
                    },
                    (u, m) => u.Insert(m, x => x.ID)
                ).ToList();

            var nUpdated =
                db.MultiTableEntities.Batch(
                    new[] {
                        new MultiTableEntity
                        {
                            ID = ids[0],
                            Value1 = "ABCx",
                            Value2 = "DEFx",
                            Value3 = "GHIx"
                        },
                        new MultiTableEntity
                        {
                            ID = ids[1],
                            Value1 = "123x",
                            Value2 = "456x",
                            Value3 = "789x"
                        }
                    },
                    (u, m) => u.Update(m)
                );

            var entity1 = db.MultiTableEntities.SingleOrDefault(m => m.ID == ids[0]);
            AssertTrue(entity1 != null);
            AssertValue("ABCx", entity1.Value1);
            AssertValue("DEFx", entity1.Value2);
            AssertValue("GHIx", entity1.Value3);

            var entity2 = db.MultiTableEntities.SingleOrDefault(m => m.ID == ids[1]);
            AssertTrue(entity2 != null);
            AssertValue("123x", entity2.Value1);
            AssertValue("456x", entity2.Value2);
            AssertValue("789x", entity2.Value3);
        }

        public void TestInsertOrUpdateNew()
        {
            int id = db.MultiTableEntities.InsertOrUpdate(
                new MultiTableEntity
                {
                    Value1 = "ABC",
                    Value2 = "DEF",
                    Value3 = "GHI"
                },
                null,
                m => m.ID
                );

            AssertTrue(id > 0);

            var entity = db.MultiTableEntities.SingleOrDefault(m => m.ID == id);
            AssertTrue(entity != null);
            AssertValue("ABC", entity.Value1);
            AssertValue("DEF", entity.Value2);
            AssertValue("GHI", entity.Value3);
        }

        public void TestInsertOrUpdateExisting()
        {
            int id = db.MultiTableEntities.InsertOrUpdate(
                new MultiTableEntity
                {
                    Value1 = "ABC",
                    Value2 = "DEF",
                    Value3 = "GHI"
                },
                null,
                m => m.ID
                );

            AssertTrue(id > 0);

            db.MultiTableEntities.InsertOrUpdate(
                new MultiTableEntity
                {
                    ID = id,
                    Value1 = "123",
                    Value2 = "456",
                    Value3 = "789"
                }
                );

            var entity = db.MultiTableEntities.SingleOrDefault(m => m.ID == id);
            AssertTrue(entity != null);
            AssertValue("123", entity.Value1);
            AssertValue("456", entity.Value2);
            AssertValue("789", entity.Value3);
        }

        public void TestDelete()
        {
            var entity =
                new MultiTableEntity
                    {
                        Value1 = "ABC",
                        Value2 = "DEF",
                        Value3 = "GHI"
                    };

            entity.ID = db.MultiTableEntities.Insert(entity, m => m.ID);

            AssertTrue(db.MultiTableEntities.Any(m => m.ID == entity.ID));

            int nDeleted = db.MultiTableEntities.Delete(entity);

            AssertValue(3, nDeleted);

            AssertFalse(db.MultiTableEntities.Any(m => m.ID == entity.ID));        
        }

        public void SelectSubset()
        {
            var id = 
                db.MultiTableEntities.Insert(
                    new MultiTableEntity
                    {
                        Value1 = "ABC",
                        Value2 = "DEF",
                        Value3 = "GHI"
                    },
                    m => m.ID
                );

            var data = db.MultiTableEntities
                         .Select(m => new { m.ID, m.Value1 })
                         .SingleOrDefault(m => m.ID == id);

            AssertTrue(data != null);
        }
    }
}