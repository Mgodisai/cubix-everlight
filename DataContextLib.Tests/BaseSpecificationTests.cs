using DataContextLib.Specifications;
using FluentAssertions;
using static DataContextLib.Tests.DataRepositoryTests;

namespace DataContextLib.Tests
{
    [TestFixture]
    public class BaseSpecificationTests
    {
        [Test]
        public void TestEntitySpecification_Should_HaveCorrectCriteria()
        {
            var spec = new TestEntitySpecification();

            spec.Criteria.Count.Should().Be(1);
            spec.Criteria.First().Compile()(new TestEntity { Id = Guid.NewGuid() }).Should().BeTrue();
        }

        [Test]
        public void TestEntitySpecification_Should_HaveCorrectIncludes()
        {
            var spec = new TestEntitySpecification();

            spec.Includes.Should().HaveCount(2, "because we added two include expression");
        }

        [Test]
        public void TestEntitySpecification_Should_HaveCorrectOrderBy()
        {
            var spec = new TestEntitySpecification();

            spec.OrderBy.Should().NotBeNull("because we set an OrderBy expression");
        }

        [Test]
        public void TestEntitySpecification_Should_HaveCorrectOrderByDescending()
        {
            var spec = new TestEntitySpecification();

            spec.OrderByDescending.Should().NotBeNull("because we set an OrderByDescending expression");
        }

        [Test]
        public void TestEntitySpecification_Should_HaveCorrectPaging()
        {
            var spec = new TestEntitySpecification();

            spec.Skip.Should().Be(1, "because we set paging to skip 1 item");
            spec.Take.Should().Be(10, "because we set paging to take 10 items");
            spec.IsPagingEnabled.Should().BeTrue("because we enabled paging");
        }
    }

    public class TestEntitySpecification : BaseSpecification<TestEntity>
    {
        public TestEntitySpecification()
        {
            AddInclude(x => x.Name);
            AddInclude(x => x.Id);
            AddCriteria(x => x.Id != Guid.Empty);
            AddOrderBy(x => x.Name);
            AddOrderByDescending(x => x.Id);
            ApplyPaging(1, 10);
        }
    }
}
