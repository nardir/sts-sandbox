using System;
using Xunit;
using EnsureThat;

namespace Axerrio.DDD.Guard.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Guard_StringEmpty_ThrowsException()
        {
            string s = string.Empty;

            var ex = Assert.Throws<ArgumentException>((() => EnsureArg.IsNotEmpty(s, nameof(s))));
        }

        [Fact]
        public void Guard_ObjectNull_ThrowsException()
        {
            //Arrange
            object o = null;

            //Act

            //Act - Assert
            var ex = Assert.Throws<ArgumentNullException>(() => {
                var o2 = EnsureArg.IsNotNull(o, nameof(o));
                    });
        }
    }
}
