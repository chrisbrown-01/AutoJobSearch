using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoJobSearchGUI.Helpers;
using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using FluentAssertions;

namespace AutoJobSearchGUI.Tests.Helpers
{
    public class ContactsHelpers_Tests
    {
        private readonly IFixture _fixture;

        public ContactsHelpers_Tests()
        {
            _fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        }

        [Fact]
        public void ConvertContactsToContactModels_CorrectlyCompletesConversion()
        {
            // Arrange
            var contacts = _fixture.CreateMany<Contact>().ToList();
            var associatedJobIds = _fixture.CreateMany<ContactAssociatedJobId>();

            // Act
            var contactModels = ContactsHelpers.ConvertContactsToContactModels(contacts, associatedJobIds);

            // Assert
            contactModels.Should().NotBeEmpty();
            contactModels.Should().AllBeOfType<ContactModel>();
            contactModels.Should().HaveCount(contacts.Count);

            for (int i = 0; i < contacts.Count; i++)
            {
                contactModels[i].Id.Should().Be(contacts[i].Id);
                contactModels[i].Company.Should().Be(contacts[i].Company);
                contactModels[i].Email.Should().Be(contacts[i].Email);
                contactModels[i].LinkedIn.Should().Be(contacts[i].LinkedIn);
                contactModels[i].Location.Should().Be(contacts[i].Location);
                contactModels[i].Name.Should().Be(contacts[i].Name);
                contactModels[i].Notes.Should().Be(contacts[i].Notes);
                contactModels[i].Phone.Should().Be(contacts[i].Phone);
                contactModels[i].Title.Should().Be(contacts[i].Title);
                contactModels[i].CreatedAt.Should().Be(contacts[i].CreatedAt);
                contactModels[i].EnableEvents.Should().Be(false);
            }
        }

        [Fact]
        public void ConvertContactToContactModel_CorrectlyCompletesConversion()
        {
            // Arrange
            var contact = _fixture.Create<Contact>();
            var associatedJobIds = _fixture.CreateMany<int>();

            // Act
            var contactModel = ContactsHelpers.ConvertContactToContactModel(contact, associatedJobIds);

            // Assert
            contactModel.Should().NotBeNull();
            contactModel.Should().BeOfType<ContactModel>();

            contactModel.Id.Should().Be(contact.Id);
            contactModel.Company.Should().Be(contact.Company);
            contactModel.Email.Should().Be(contact.Email);
            contactModel.LinkedIn.Should().Be(contact.LinkedIn);
            contactModel.Location.Should().Be(contact.Location);
            contactModel.Name.Should().Be(contact.Name);
            contactModel.Notes.Should().Be(contact.Notes);
            contactModel.Phone.Should().Be(contact.Phone);
            contactModel.Title.Should().Be(contact.Title);
            contactModel.CreatedAt.Should().Be(contact.CreatedAt);
            contactModel.EnableEvents.Should().Be(false);
            contactModel.JobListingIds.Should().BeEquivalentTo(associatedJobIds);
        }
    }
}
