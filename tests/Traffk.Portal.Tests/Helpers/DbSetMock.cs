using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;

namespace Traffk.Portal.Tests.Helpers
{
    public static class DbSetMock
    {
        public static Mock<DbSet<T>> Create<T>(params T[] elements) where T : class
        {
            return new List<T>(elements).AsDbSetMock();
        }
    }

    public static class ListExtensions
    {
        public static Mock<DbSet<T>> AsDbSetMock<T>(this List<T> list) where T : class
        {
            IQueryable<T> queryableList = list.AsQueryable();
            Mock<DbSet<T>> dbSetMock = new Mock<DbSet<T>>();
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Provider).Returns(queryableList.Provider);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.Expression).Returns(queryableList.Expression);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(queryableList.ElementType);
            dbSetMock.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(queryableList.GetEnumerator());
            return dbSetMock;
        }
    }
}
