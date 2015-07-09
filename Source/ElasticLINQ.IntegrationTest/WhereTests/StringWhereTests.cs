using System.Linq;
using ElasticLINQ.IntegrationTest.Models;
using Xunit;

namespace ElasticLINQ.IntegrationTest.WhereTests
{
    public class StringWhereTests
    {
        const string Surname = "Morales";

        [Fact]
        public void EqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => Surname == w.Surname));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Surname == Surname));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Surname.Equals(Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => Surname.Equals(w.Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(w.Surname, Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => Equals(Surname, w.Surname)));
        }

        [Fact]
        public void NotEqualToConstant()
        {
            DataAssert.Same<WebUser>(q => q.Where(w => Surname != w.Surname));
            DataAssert.Same<WebUser>(q => q.Where(w => w.Surname != Surname));
            DataAssert.Same<WebUser>(q => q.Where(w => !w.Surname.Equals(Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Surname.Equals(w.Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(w.Surname, Surname)));
            DataAssert.Same<WebUser>(q => q.Where(w => !Equals(Surname, w.Surname)));
        }
    }
}