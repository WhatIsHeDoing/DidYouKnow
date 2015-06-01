using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csharp
{
    /// <summary>
    /// Tests mapping fields from one class to another using AutoMapper,
    /// rather than doing it yourself everytime.  Great for mapping
    /// view models to service contract models, etc.
    /// </summary>
    [TestClass]
    public class AutoMapperTest
    {
        public class PersonTypeOne
        {
            public Guid Id { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string FirstName { get; set; }
            public string Surname { get; set; }
        }

        public class PersonTypeTwo
        {
            public Guid Id { get; set; }
            public DateTime DoB { get; set; }
            public string Name { get; set; }
        }

        /// <summary>
        /// Shows how mapping can be customised to map fields with different
        /// names, and also perform extra work before and after that mapping.
        /// </summary>
        [TestMethod]
        public void CustomMapping()
        {
            // Set up the mapping between the person types.
            Mapper.CreateMap<PersonTypeOne, PersonTypeTwo>()
                .ForMember(dest => dest.DoB, opt =>
                    opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                    dest.Name = src.FirstName + " " + src.Surname);

            // Ensure that the mapping is valid.
            try
            {
                Mapper.AssertConfigurationIsValid();
            }
            catch (Exception e)
            {
                Assert.Fail("Configuration invalid: " + e.Message);
            }

            // Instantiate a person of the first type.
            var personTypeOne = new PersonTypeOne
            {
                Id = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                FirstName = "Bob",
                Surname = "Hope"
            };

            // Map that to the second type.
            var personTypeTwo = Mapper.Map
                <PersonTypeOne, PersonTypeTwo>(personTypeOne);

            // Assert the data has been copied as expected.
            Assert.AreEqual(personTypeOne.Id, personTypeTwo.Id,
                "Exact name match");

            Assert.AreEqual(personTypeOne.DateOfBirth, personTypeTwo.DoB,
                "Custom match on names");

            Assert.AreEqual("Bob Hope", personTypeTwo.Name,
                "Set after mapping");
        }
    }
}
