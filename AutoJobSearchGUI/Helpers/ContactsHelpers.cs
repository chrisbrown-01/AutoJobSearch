using AutoJobSearchGUI.Models;
using AutoJobSearchShared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoJobSearchGUI.Helpers
{
    internal static class ContactsHelpers
    {
        internal static List<ContactModel> ConvertContactsToContactModels(
            IEnumerable<Contact> contacts, 
            IEnumerable<ContactAssociatedJobId> contactsAssociatedJobIds)
        {
            var contactModels = new List<ContactModel>();

            foreach (var contact in contacts)
            {
                var contactModel = new ContactModel
                {
                    Id = contact.Id,
                    CreatedAt = contact.CreatedAt,
                    Company = contact.Company,
                    Location = contact.Location,
                    Name = contact.Name,
                    Title = contact.Title,
                    Email = contact.Email,
                    Phone = contact.Phone,
                    LinkedIn = contact.LinkedIn,
                    Notes = contact.Notes
                };

                contactModel.JobListingIds = contactsAssociatedJobIds.Where(x => x.ContactId == contactModel.Id).Select(x => x.JobListingId).ToList();

                contactModels.Add(contactModel);
            }

            return contactModels;
        }

        internal static ContactModel ConvertContactToContactModel(Contact contact, IEnumerable<int> associatedJobIds)
        {
            return new ContactModel()
            {
                Id = contact.Id,
                JobListingIds = associatedJobIds.ToList(),
                CreatedAt = contact.CreatedAt,
                Company = contact.Company,
                Location = contact.Location,
                Name = contact.Name,
                Title = contact.Title,
                Email = contact.Email,
                Phone = contact.Phone,
                LinkedIn = contact.LinkedIn,
                Notes = contact.Notes
            };
        }
    }
}
