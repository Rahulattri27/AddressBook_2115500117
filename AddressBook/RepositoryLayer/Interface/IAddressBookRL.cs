﻿using System;
using ModelLayer.Model;
namespace RepositoryLayer.Interface
{
	public interface IAddressBookRL
	{
		AddressBookEntry GetById(int id);
        public List<AddressBookEntry> GetAll();
        bool AddEntry(AddressBookEntry entry);
        bool UpdateEntry(int id, AddressBookEntry updatedentry);
        bool DeleteEntry(int id);

        }
}

