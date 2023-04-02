global using System.Collections.Concurrent;
global using IP2C_Web_API.Interfaces;
global using IP2C_Web_API.Models;
global using IP2C_Web_API.Services;
global using IP2C_Web_API.Helpers;
global using System;
global using System.Collections.Generic;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.AspNetCore.Mvc;
global using System.Linq;
global using Dapper;
global using System.Text.RegularExpressions;
global using RestSharp;
global using Microsoft.AspNetCore.Authorization;
global using static Microsoft.Toolkit.StringExtensions;
global using static Globals;

public class Globals
{
    public static ConcurrentDictionary<string, IPDetailsDTO> _cachedIPs = new();
}