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
        internal static List<ContactModel> ConvertContactsToContactModels(IEnumerable<Contact> contacts)
        {
            var contactModels = new List<ContactModel>();

            foreach (var contact in contacts)
            {
                var contactModel = new ContactModel
                {
                    Id = contact.Id,
                    JobListingId = contact.JobListingId,
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

                contactModels.Add(contactModel);
            }

            return contactModels;
        }
    }
}
