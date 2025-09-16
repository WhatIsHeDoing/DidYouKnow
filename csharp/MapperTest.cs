using Mapster;
using Xunit;

namespace csharp
{
    /// <summary>
    /// Tests mapping fields from one class to another using Mapster,
    /// rather than doing it yourself every time.  Great for mapping
    /// view models to service contract models, etc.
    /// </summary>
    /// <remarks>https://github.com/MapsterMapper/Mapster</remarks>
    public class MapperTest
    {
        class PersonTypeOne
        {
            public Guid Id { get; set; }
            public DateTime DateOfBirth { get; set; }
            public required string FirstName { get; set; }
            public required string Surname { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        class PersonTypeTwo
        {
            // ReSharper disable UnusedAutoPropertyAccessor.Local
            public Guid Id { get; set; }
            public DateTime DoB { get; set; }
            public required string Name { get; set; }
        }

        /// <summary>
        /// Shows how mapping can be customised to map fields with different
        /// names, and also perform extra work before and after that mapping.
        /// </summary>
        [Fact]
        public void CustomMapping()
        {
            // Set up the mapping between the person types.
            TypeAdapterConfig<PersonTypeOne, PersonTypeTwo>
                .NewConfig()
                .Map(dest => dest.DoB, opt => opt.DateOfBirth)
                //.Map(dest => dest.Name, opt => opt.Ignore())
                .Map(dest => dest.Name, src => src.FirstName + " " + src.Surname);

            // Instantiate a person of the first type.
            var personTypeOne = new PersonTypeOne
            {
                Id = Guid.NewGuid(),
                DateOfBirth = DateTime.Now,
                FirstName = "Bob",
                Surname = "Hope"
            };

            // Map that to the second type.
            var personTypeTwo = personTypeOne.Adapt<PersonTypeTwo>();

            // Assert the data has been copied as expected.
            Assert.Equal(personTypeOne.Id, personTypeTwo.Id);
            Assert.Equal(personTypeOne.DateOfBirth, personTypeTwo.DoB);
            Assert.Equal("Bob Hope", personTypeTwo.Name);
        }
    }
}
