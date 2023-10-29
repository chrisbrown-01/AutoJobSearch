using AutoJobSearchShared.Database;
using AutoJobSearchShared.Models;
using FluentAssertions;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchShared.Tests.Database
{
    public class IAutoJobSearchDb
    {
        private AutoJobSearchShared.Database.IAutoJobSearchDb _autoJobSearchDb;

        public IAutoJobSearchDb()
        {
            _autoJobSearchDb = Substitute.For<AutoJobSearchShared.Database.IAutoJobSearchDb>();
        }

        [Fact]
        public async Task CreateJobSearchProfileAsync_ShouldReturnJobSearchProfile()
        {
            _autoJobSearchDb.CreateJobSearchProfileAsync(Arg.Any<JobSearchProfile>()).Returns(new JobSearchProfile());

            var createdProfile = await _autoJobSearchDb.CreateJobSearchProfileAsync(new JobSearchProfile());

            createdProfile.Should().NotBeNull();    
            createdProfile.Should().BeOfType<JobSearchProfile>();
        }
    }
}
