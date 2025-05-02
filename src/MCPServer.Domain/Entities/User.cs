using System;

namespace MCPServer.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool IsActive { get; private set; }

        public User(string username, string email, string passwordHash)
        {
            Id = Guid.NewGuid();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }
    }
} 