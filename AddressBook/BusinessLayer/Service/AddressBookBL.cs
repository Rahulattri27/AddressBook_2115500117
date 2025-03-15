using System;
using AutoMapper;
using ModelLayer.DTO;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using BusinessLayer.Interface;
using StackExchange.Redis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace BusinessLayer.Service
{
	public class AddressBookBL:IAddressBookBL
	{
		private readonly IAddressBookRL _addressBookRL;
		private readonly IMapper _mapper;
		private readonly IDatabase _cache;
		private readonly TimeSpan _cacheDuration;
		//Constructor of class 
		public AddressBookBL(IAddressBookRL addressBookRL,IMapper mapper, IConnectionMultiplexer redis,IConfiguration configuration)
		{
			_mapper = mapper;
			_addressBookRL = addressBookRL;
			_cache = redis.GetDatabase();
			_cacheDuration = TimeSpan.FromSeconds(int.Parse(configuration["Redis:CacheDuration"] ?? "300"));
		}
		
		//method to get the  contacts from Respository layer if not available in cache
		public async Task<List<AddressBookDTO>> GetAllContacts()
		{
			//check the cache if its not null
			const string cacheKey = "contact_list";
			var cachedData = await _cache.StringGetAsync(cacheKey);
			if (!cachedData.IsNullOrEmpty)
			{
				return JsonSerializer.Deserialize<List<AddressBookDTO>>(cachedData);
			}

			var contacts = _addressBookRL.GetAll();
			//create the updated cache  if not available
			await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(_mapper.Map<List<AddressBookDTO>>(contacts)), _cacheDuration);
			return _mapper.Map<List<AddressBookDTO>>(contacts);
		}
        //method to get the particular contact from Respository layer if not available in cache
        public async Task<AddressBookDTO?> GetContactById(int id)
		{
			//check the contact in cache
			string cacheKey = $"contact_{id}";
			var cachedData = await _cache.StringGetAsync(cacheKey);
			if (!cachedData.IsNullOrEmpty)
			{
				return JsonSerializer.Deserialize<AddressBookDTO>(cachedData);
			}
			var contact = _addressBookRL.GetById(id);
			if (contact == null)
			{
				return null;
			}
			//create the contact in cache
			await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contact), _cacheDuration);
            return _mapper.Map<AddressBookDTO>(contact);
        }


        //method to add the contact in Respository layer
        public async Task<bool> AddContact(AddressBookDTO contact)
		{
			var entry = _mapper.Map<AddressBookEntry>(contact);
			var response= _addressBookRL.AddEntry(entry);
			if (response)
			{
				//create the contact in cache and update the cache list
				string cacheKey = $"contact_{entry.Id}";
				await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contact), _cacheDuration);
				var contacts = _addressBookRL.GetAll();
				await _cache.StringSetAsync("contact_list", JsonSerializer.Serialize(_mapper.Map<List<AddressBookDTO>>(contacts)), _cacheDuration);
			}
			return response;
			
		}


        //method to update  the contacts in Respository layer
        public async Task<bool> Update(int id,AddressBookDTO contact)
		{
			string cacheKey = $"contact_{id}";
			var cachedData = await _cache.StringGetAsync(cacheKey);
			if (!cachedData.IsNullOrEmpty)
			{
				var cachedcontact = JsonSerializer.Deserialize<AddressBookDTO>(cacheKey);
				if (cachedcontact != null)
				{
					//update the data in cache
					cachedcontact = contact;
					await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(cachedcontact), _cacheDuration);
				}

			}
			var existingContact = _addressBookRL.GetById(id);
			if (existingContact == null)
			{
				return false;
			}
			//update the database
			var updatedcontact = _mapper.Map<AddressBookEntry>(contact);
			_addressBookRL.UpdateEntry(id, updatedcontact);
			await _cache.KeyDeleteAsync("contact_list");
			return true;
			
		}


        //method to delete the contacts from Respository layer
        public async Task<bool> DeleteContact(int id)
		{
			bool result = _addressBookRL.DeleteEntry(id);
			if (result)
			{
				//this will create the latest list on calling the method in cache
				await _cache.KeyDeleteAsync($"contact_{id}");
				await _cache.KeyDeleteAsync("contact_list");
			}
			return result;
			
		}
	}
}

