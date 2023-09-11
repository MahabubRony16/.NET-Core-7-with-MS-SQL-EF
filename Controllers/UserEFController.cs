using System;
using System.Numerics;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using AutoMapper;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserEFController : ControllerBase
{
    DataContextEF _entityFramework;
    IUserRepository _userRepository;
    IMapper _mapper;
    public UserEFController(IConfiguration config, IUserRepository userRepository)
    {
        _entityFramework = new DataContextEF(config);

        _userRepository = userRepository;

        _mapper = new Mapper(new MapperConfiguration(cfg =>{
            cfg.CreateMap<UserToAddDto, User>();
        }));
    }


    [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers()
    {
        IEnumerable<User> users = _userRepository.GetUsers();
        return users;

    }

    [HttpGet("GetSingleUser/{userId}")]
    // public IEnumerable<User> GetUsers()
    public User GetSingleUser(int userId)
    {
        return _userRepository.GetSingleUser(userId);
        
    }
    
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        User? userDb = _userRepository.GetSingleUser(user.UserId);

        if (userDb != null)
        {
            userDb.FirstName = user.FirstName;
            userDb.LastName = user.LastName;
            userDb.Email = user.Email;
            userDb.Gender = user.Gender;
            userDb.FirstName = user.FirstName;
            userDb.Active = user.Active;
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new System.Exception("Failed to Update User");
        }
        throw new System.Exception("Failed to Get User");
    }


    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        User userDb = _mapper.Map<User>(user);

        _userRepository.AddEntity<User>(userDb);
        if(_userRepository.SaveChanges())
        {
            return Ok();
        }
        throw new System.Exception("Failed to Add User");

    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        User? userDb = _userRepository.GetSingleUser(userId);

        if (userDb != null)
        {
            _userRepository.RemoveEntity<User>(userDb);
            if(_userRepository.SaveChanges())
            {
                return Ok();
            }
            throw new System.Exception("Failed to Delete User");
        }
        throw new System.Exception("Failed to Get User");
    }
}
