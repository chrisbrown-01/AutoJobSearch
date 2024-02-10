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
        // TODO: add all contactassociatedjobIds enumerable as an argument here?
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

                // TODO: debug
                contactModel.JobListingIds = contactsAssociatedJobIds.Where(x => x.ContactId == contactModel.Id).Select(x => x.JobListingId).ToList();

                contactModels.Add(contactModel);
            }

            return contactModels;
        }
    }
}
