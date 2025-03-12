using System;
using ModelLayer.DTO;

namespace BusinessLayer.Interface
{
	public interface IAddressBookBL
	{
        List<AddressBookDTO> GetAllContacts();
        AddressBookDTO GetContactById(int id);
        bool AddContact(AddressBookDTO contact);
        bool Update(int id, AddressBookDTO contact);
        bool DeleteContact(int id);
    }
}

