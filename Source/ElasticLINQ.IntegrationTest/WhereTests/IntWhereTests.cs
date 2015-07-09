using System.Linq;
using ElasticLinq.IntegrationTest.Models;
using Xunit;

namespace ElasticLinq.IntegrationTest.WhereTests
{
    public class IntWhereTests
    {
        const int Middle = 50;

        [Fact]
        public void LessThanConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id < Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => Middle < w.Id));
        }

        [Fact]
        public void LessThanOrEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id <= Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => Middle <= w.Id));
        }

        [Fact]
        public void GreaterThanConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id > Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => Middle > w.Id));
        }

        [Fact]
        public void GreaterThanOrEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id >= Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => Middle >= w.Id));
        }

        [Fact]
        public void EqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => Middle == w.Id));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id == Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id.Equals(Middle)));
            DataAssert.Same<WebUser>(q => q.Where(w => Middle.Equals(w.Id)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(w.Id, Middle)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(Middle, w.Id)));
        }

        [Fact]
        public void NotEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => Middle != w.Id));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Id != Middle));
            DataAssert.Same<WebUser>(q => q.Where(w => !w.Id.Equals(Middle)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Middle.Equals(w.Id)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(w.Id, Middle)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(Middle, w.Id)));
        }
    }
}