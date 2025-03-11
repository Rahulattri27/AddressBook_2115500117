using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using ModelLayer.DTO;
using ModelLayer.Model;

namespace AddressBook.Controllers;

[ApiController]
[Route("api/addressbook")]
public class AddressBookController : ControllerBase
{
    /// <summary>
    /// Initialize the context to access the database
    /// </summary>
    private readonly AddressBookContext _context;

    /// <summary>
    /// call the constructor of controller
    /// </summary>
    /// <param name="context">DbContext from program.cs</param>
    public AddressBookController(AddressBookContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Request to get all the addressbookentries from database
    /// </summary>
    /// <returns>Response of Success or failure</returns>
    [HttpGet]
    public IActionResult GetAll()
    {
        var response = new ResponseBody<List<AddressBookEntry>>();
        var data = _context.AddressBook.ToList<AddressBookEntry>();
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
    /// Request to get the address book entry by particular id
    /// </summary>
    /// <param name="id">id from user</param>
    /// <returns>Success or failure response</returns>
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var response = new ResponseBody<AddressBookEntry>();
        var data = _context.AddressBook.Find(id);
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
    /// Request to add the Contact in the Address Book
    /// </summary>
    /// <param name="contact">AddressBookEntry from user in special format</param>
    /// <returns>Success or failure response</returns>
    [HttpPost]
    public IActionResult AddContact(AddressBookEntry contact)
    {
        var response = new ResponseBody<AddressBookEntry>();
        var data = _context.AddressBook.Add(contact);
        _context.SaveChanges();
        if (data != null)
        {
            response.Success = true;
            response.Message = "Contact added Successfully.";
            response.Data = contact;
            return Ok(response);
        }
        response.Success = false;
        response.Message = "Cannot add contact.";
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
    public IActionResult UpdateContact(int id,AddressBookEntry updatedcontact)
    {
        var response = new ResponseBody<AddressBookEntry>();
        var contact = _context.AddressBook.Find(id);
        if (contact != null)
        {
            contact.Name = updatedcontact.Name;
            contact.Email = updatedcontact.Email;
            contact.Phone_No = updatedcontact.Phone_No;
            _context.SaveChanges();

            response.Success = true;
            response.Message = "Contact updated Successfully.";
            response.Data = contact;
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
    public IActionResult DeleteContact(int id)
    {
        var response = new ResponseBody<string>();
        var data = _context.AddressBook.Find(id);
        if (data == null)
        {
            response.Message = $"{id} not present in database";
            return NotFound(response);
        }
        _context.AddressBook.Remove(data);
        _context.SaveChanges();
        response.Success = true;
        response.Message = $"successfully deleted {id}.";
        return Ok(response);
    }
}


