using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pennywise.Model;
namespace Pennywise.Services.Interfaces;

// Interface for managing tags in the Pennywise application
public interface ITagService
{
    /*
     * Interface methods for tag operations:
     * - Retrieving all tags
     * - Adding custom tags
     * - Deleting custom tags
     */
    
    // Gets a list of all available tags asynchronously
    Task<List<string>> GetAllTagsAsync();
    
    // Adds a new custom tag to the system
    Task AddCustomTagAsync(string tag);
    
    // Removes an existing custom tag from the system
    Task DeleteCustomTagAsync(string tag);
} 