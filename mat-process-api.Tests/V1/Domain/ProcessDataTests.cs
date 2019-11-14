using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mat_process_api.V1.Domain;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace matprocess_api.Tests.V1.Domain
{
    [TestFixture]
    public class ProcessDataTests
    {
        private MatProcessData processData;

        [SetUp]
        public void set_up()
        {
            processData = new MatProcessData();
        }

        [Test]
        public void process_data_has_id()
        {
            Assert.Null(processData.Id);
        }

        [Test]
        public void process_has_process_type()
        {
            Assert.Null(processData.ProcessType);
        }
        [Test]
        public void process_data_has_date_created()
        {
            DateTime date = new DateTime(2019, 11, 21);
            processData.DateCreated = date;
            Assert.AreEqual(date, processData.DateCreated);
        }

        [Test]
        public void process_data_has_date_last_modified()
        {
            DateTime date = new DateTime(2019, 11, 21);
            processData.DateLastModified = date;
            Assert.AreEqual(date, processData.DateLastModified);
        }
        [Test]
        public void process_data_has_date_completed()
        {
            DateTime date = new DateTime(2019, 11, 21);
            processData.DateCompleted = date;
            Assert.AreEqual(date, processData.DateCompleted);
        }

        [Test]
        public void process_data_has_data_schema_version()
        {
            Assert.Zero(processData.DataSchemaVersion);
        }

        [Test]
        public void process_data_has_process_stage()
        {
            Assert.Null(processData.ProcessStage);
        }

        [Test]
        public void process_data_has_pre_process_data()
        {
            Assert.Null(processData.PreProcessData);
        }

        [Test]
        public void process_data_has_process_data_object()
        {
            Assert.Null(processData.ProcessData);
        }
    }
}
