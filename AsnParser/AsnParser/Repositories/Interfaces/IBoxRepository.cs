using AsnParser.Models;

namespace AsnParser.Repositories.Interfaces;

public interface IBoxRepository
{
    Task SaveBoxAsync(Box box);
}