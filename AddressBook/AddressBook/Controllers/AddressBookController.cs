using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using ModelLayer.DTO;
using ModelLayer.Model;
using BusinessLayer.Interface;

namespace AddressBook.Controllers;

[ApiController]
[Route("api/addressbook")]
public class AddressBookController : ControllerBase
{
    /// <summary>
    ///  object of Context to access the database
    /// </summary>
    private readonly AddressBookContext _context;

    /// <summary>
    /// Object of BusinessLayer Service
    /// </summary>
    private readonly IAddressBookBL _addressBookBL;

    /// <summary>
    /// call the constructor of controller
    /// </summary>
    /// <param name="context">DbContext from program.cs</param>
    public AddressBookController(AddressBookContext context,IAddressBookBL addressBookBL)
    {
        _context = context;
        _addressBookBL = addressBookBL;
    }
    /// <summary>
    /// Get all the addressbook contacts from database
    /// </summary>
    /// <returns>Response of Success or failure</returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new ResponseBody<List<AddressBookDTO>>();
        var data = _addressBookBL.GetAllContacts();
        if(data != null)
        {
            response.Success = true;
            response.Message = "All AddressBook Entries Read Successfully.";
            response.Data = data;
            return Ok(response);
        }
        response.Success = false;
        response.Message = "Cannot Read Entries";
        response.Data = null;
        return NotFound(response);

    }
    /// <summary>
    /// Get the address book contact by particular id
    /// </summary>
    /// <param name="id">id from user</param>
    /// <returns>Success or failure response</returns>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var data = _addressBookBL.GetContactById(id);
        if (data != null)
        {
            response.Success = true;
            response.Message = $"AddressBook Entry with {id} Read Successfully.";
            response.Data = data;
            return Ok(response);
        }
        response.Success = false;
        response.Message = $"Cannot Read {id} in database.";
        response.Data = null;
        return NotFound(response);

    }

    /// <summary>
    /// Add the Contact in the Address Book
    /// </summary>
    /// <param name="contact">AddressBookEntry from user in special format</param>
    /// <returns>Success or failure response</returns>
    [HttpPost]
    public IActionResult CreateContact(AddressBookDTO contact)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var data = _addressBookBL.AddContact(contact);
        if (data)
        {
            response.Success = true;
            response.Message = "Successfully added Contact";
            response.Data = contact;
            return Ok(response);
        }
        response.Success = false;
        response.Message ="Unable to add Contact" ;
        response.Data = null;
        return NotFound(response);

    }
    /// <summary>
    /// Update the Address book entry at particular id
    /// </summary>
    /// <param name="id">id of contact to update</param>
    /// <param name="updatedcontact">updated info of contact</param>
    /// <returns>Succes or failure response</returns>
    [HttpPut("{id}")]
    public IActionResult Update(int id,AddressBookDTO updatedcontact)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var result = _addressBookBL.Update(id, updatedcontact);
        if (result)
        {
            response.Success = true;
            response.Message = "Contact updated Successfully.";
            response.Data = updatedcontact;
            return Ok(response);
        }
        response.Success = false;
        response.Message = "Cannot update contact.";
        response.Data = null;
        return NotFound(response);

    }

    /// <summary>
    /// Delete the particular id Contact info if present
    /// </summary>
    /// <param name="id">id of Contact entered by user</param>
    /// <returns>Success or failure response</returns>
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var response = new ResponseBody<string>();
        var data = _addressBookBL.DeleteContact(id);
        if (data)
        {
            response.Success = true;
            response.Message = $"Contact with {id} deleted Successfully.";
            return Ok(response);
        }
        response.Success = false;
        response.Message=$"Unable to delete contact with id {id}." ;
        return NotFound(response);
    
             
    }
}


