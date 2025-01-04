using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Pennywise.Model;
using Pennywise.Services.Interfaces;

public class TagService : ITagService
{
    private readonly string _tagsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "tags.csv");

    private readonly List<string> _defaultTags = new List<string>
    {
        "Yearly", "Monthly", "Food", "Drinks", "Clothes", "Gadgets",
        "Miscellaneous", "Fuel", "Rent", "EMI", "Party"
    };

    public async Task<List<string>> GetAllTagsAsync()
    {
        var customTags = await LoadCustomTagsAsync();
        return _defaultTags.Concat(customTags).Distinct().ToList();
    }

    public async Task AddCustomTagAsync(string tag)
    {
        var customTags = await LoadCustomTagsAsync();
        if (!customTags.Contains(tag) && !_defaultTags.Contains(tag))
        {
            customTags.Add(tag);
            await SaveCustomTagsAsync(customTags);
        }
    }

    private async Task<List<string>> LoadCustomTagsAsync()
    {
        if (!File.Exists(_tagsFilePath))
        {
            return new List<string>();
        }

        var tags = await File.ReadAllLinesAsync(_tagsFilePath);
        return tags.ToList();
    }

    private async Task SaveCustomTagsAsync(List<string> tags)
    {
        await File.WriteAllLinesAsync(_tagsFilePath, tags);
    }

    public async Task DeleteCustomTagAsync(string tag)
    {
        if (_defaultTags.Contains(tag))
        {
            throw new InvalidOperationException("Cannot delete default tags");
        }

        var customTags = await LoadCustomTagsAsync();
        if (customTags.Contains(tag))
        {
            customTags.Remove(tag);
            await SaveCustomTagsAsync(customTags);
        }
    }
} 