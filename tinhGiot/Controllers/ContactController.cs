using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BaseClasses;

namespace tinhGiot.Controllers
{
    public class ContactController : ApiController
    {
        dbTinhGiotDataContext db = new dbTinhGiotDataContext();
        List<Contacts> contacts = new List<Contacts>();
        public IEnumerable<Contacts> GetAllContact()
        {
            GetContact();
            return contacts;
        }

        private void GetContact()
        {
            var contact = db.Contacts.ToList();

            foreach (var item in contact)
            {
                contacts.Add(new Contacts
                {
                    ID = Convert.ToInt32(item.ID),
                    Name = item.Name,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Note = item.Note,
                    ContactTime = Convert.ToDateTime(item.ContactTime)
                });
            }
        }
        public Contacts GetContactbyId(int id)
        {
            if (contacts.Count() > 0)
            {
                return contacts.Find(p => p.ID == id);
            }
            else
            {
                GetContact();
                var abc = contacts.Find(p => p.ID == id);
                return abc;
            }
        }
        public IEnumerable<Contacts> DeleteContact(int id)
        {
            GetDeleteContact(id);
            return contacts;
        }
        private void GetDeleteContact(int id)
        {
            Contact us = db.Contacts.SingleOrDefault(n => n.ID == id);

            if (us != null)
            {
                db.Contacts.DeleteOnSubmit(us);
                db.SubmitChanges();
            }
        }
    }
}
