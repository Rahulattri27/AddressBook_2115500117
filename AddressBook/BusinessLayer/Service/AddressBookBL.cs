using System;
using AutoMapper;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
namespace BusinessLayer.Service
{
	public class AddressBookBL:IAddressBookBL
	{
		private readonly IAddressBookRL _addressBookRL;
		private readonly IMapper _mapper;
		//Constructor of class 
		public AddressBookBL(IAddressBookRL addressBookRL,IMapper mapper)
		{
			_mapper = mapper;
			_addressBookRL = addressBookRL;
		}
		//method to get the  contacts from Respository layer
		public List<AddressBookDTO> GetAllContacts()
		{
			var contacts = _addressBookRL.GetAll();
			return _mapper.Map<List<AddressBookDTO>>(contacts);
		}
        //method to get the particular contact from Respository layer
        public AddressBookDTO? GetContactById(int id)
		{
			var contact = _addressBookRL.GetById(id);
			if (contact == null)
			{
				return null;
			}
            return _mapper.Map<AddressBookDTO>(contact);
        }
        //method to add the contact in Respository layer
        public bool AddContact(AddressBookDTO contact)
		{
			var entry = _mapper.Map<AddressBookEntry>(contact);
			return _addressBookRL.AddEntry(entry);
			
		}
        //method to update  the contacts in Respository layer
        public bool Update(int id,AddressBookDTO contact)
		{
			var updatedcontact = _mapper.Map<AddressBookEntry>(contact);
			return _addressBookRL.UpdateEntry(id, updatedcontact);
			
		}
        //method to delete the contacts from Respository layer
        public bool DeleteContact(int id)
		{
			bool result = _addressBookRL.DeleteEntry(id);
			return result;
			
		}
	}
}

