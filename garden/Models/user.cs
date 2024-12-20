﻿using System.Text.Json.Serialization;
public class User
{
    public int UserId { get; set; } 
    public string Name { get; set; } = ""; 
    public string Email { get; set; } = ""; 
    public string Password { get; set; } = ""; 
    public string Role { get; set; } = "";

    public List<Order> Orders { get; set; } = new();
}