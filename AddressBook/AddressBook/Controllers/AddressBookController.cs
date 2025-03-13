using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Context;
using ModelLayer.DTO;
using ModelLayer.Model;
using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AddressBook.Controllers;

[ApiController]
[Route("api/addressbook")]
public class AddressBookController : ControllerBase
{
    

    /// <summary>
    /// Object of BusinessLayer Service
    /// </summary>
    private readonly IAddressBookBL _addressBookBL;

    /// <summary>
    /// call the constructor of controller
    /// </summary>
    /// <param name="context">DbContext from program.cs</param>
    public AddressBookController(IAddressBookBL addressBookBL)
    {
       
        _addressBookBL = addressBookBL;
    }
    /// <summary>
    /// Get the userId from Token
    /// </summary>
    /// <returns>return user id from token if present</returns>
    private int? GetLoggedInUserId()
    {
        var userIdClaim = User.FindFirst("UserId")?.Value;
        return userIdClaim != null ? int.Parse(userIdClaim) : null;
    }
    /// <summary>
    /// function to get the Role
    /// </summary>
    /// <returns>Role of user</returns>
    private string? GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }


    /// <summary>
    /// Get all the addressbook contacts (Admin only)
    /// </summary>
    /// <returns>Response of Success or failure</returns>
    [HttpGet]
    [Authorize(Roles="Admin")]
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
        return NotFound(response);

    }
    /// <summary>
    /// Get the address book contact by particular id(Admin can access all ids but user can access only its ids)
    /// </summary>
    /// <param name="id">id from user</param>
    /// <returns>Success or failure response</returns>
    [Authorize]
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var role = GetUserRole();
        var userId = GetLoggedInUserId();
        var data = _addressBookBL.GetContactById(id);
        if (data == null)
        {
            response.Success = false;
            response.Message = $"Contact with id  {id} not found.";
            return NotFound(response);
        }
        if(role=="User" && data.UserId != userId)
        {
            response.Message = "Not Allowed";
            return Forbid();

        }
        response.Success = true;
        response.Message = $"AddressBook Entry with {id} Read Successfully.";
        response.Data = data;
        return NotFound(response);

    }

    /// <summary>
    /// Add the Contact in the Address Book
    /// </summary>
    /// <param name="contact">AddressBookEntry from user in special format</param>
    /// <returns>Success or failure response</returns>
    [HttpPost]
    [Authorize(Roles ="User")]
    public IActionResult CreateContact(AddressBookDTO contact)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var userId = GetLoggedInUserId();
        if (userId==null)
        {
            return Unauthorized();
        }
        contact.UserId = userId.Value;
        var data = _addressBookBL.AddContact(contact);
        if (!data)
        {
            response.Success = false;
            response.Message = "Unable to add Contact.";
            return BadRequest(response);
        }
        response.Success = true;
        response.Message = "Contact Added Successfully.";
        response.Data = contact;
        return Ok(response);


    }
    /// <summary>
    /// Update the Address book entry at particular id
    /// </summary>
    /// <param name="id">id of contact to update</param>
    /// <param name="updatedcontact">updated info of contact</param>
    /// <returns>Succes or failure response</returns>
    [Authorize]
    [HttpPut("{id}")]
    public IActionResult Update(int id,AddressBookDTO updatedcontact)
    {
        var response = new ResponseBody<AddressBookDTO>();
        var role = GetUserRole();
        var userId = GetLoggedInUserId();
        var existingContact = _addressBookBL.GetContactById(id);
        if (existingContact == null)
        {
            response.Message = "Contact not found";
            return NotFound(response);
        }
        if(role=="User" && existingContact.UserId != userId)
        {
            return Forbid();
        }
        var result = _addressBookBL.Update(id, updatedcontact);
        if (!result)
        {
            response.Message = "Unable to update contact.";
            return BadRequest(response);
        }
        response.Success = true;
        response.Message = "Contact updated Successfully.";
        response.Data = updatedcontact;
        return Ok(response);
       

    }

    /// <summary>
    /// Delete the particular id Contact info if present
    /// </summary>
    /// <param name="id">id of Contact entered by user</param>
    /// <returns>Success or failure response</returns>
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(int id)
    {
        var response = new ResponseBody<string>();
        var role = GetUserRole();
        var userId = GetLoggedInUserId();
        var existingContact = _addressBookBL.GetContactById(id);
        if (existingContact == null)
        {
            response.Message = "Contact Not Found";
            return NotFound(response);
        }
        if(role=="User" && existingContact.UserId!= userId)
        {
            return Forbid();
        }
        var data = _addressBookBL.DeleteContact(id);
        if (data)
        {
            response.Success = true;
            response.Message = $"Contact with {id} deleted Successfully.";
            return Ok(response);
        }
        response.Success = false;
        response.Message=$"Unable to delete contact with id {id}." ;
        return BadRequest(response);
    
             
    }
}


