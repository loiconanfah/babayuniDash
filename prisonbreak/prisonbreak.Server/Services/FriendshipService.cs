using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Service pour gérer les amitiés entre utilisateurs.
    /// </summary>
    public class FriendshipService : IFriendshipService
    {
        private readonly HashiDbContext _context;

        public FriendshipService(HashiDbContext context)
        {
            _context = context;
        }

        public async Task<FriendRequestDto> SendFriendRequestAsync(int requesterId, int receiverId)
        {
            if (requesterId == receiverId)
                throw new ArgumentException("Un utilisateur ne peut pas s'envoyer une demande d'amitié à lui-même.");

            // Vérifier si une demande existe déjà
            var existingRequest = await _context.FriendRequests
                .FirstOrDefaultAsync(fr => (fr.RequesterId == requesterId && fr.ReceiverId == receiverId) ||
                                          (fr.RequesterId == receiverId && fr.ReceiverId == requesterId));

            if (existingRequest != null)
            {
                if (existingRequest.Status == FriendRequestStatus.Pending)
                {
                    // Si c'est la même demande (même requester et receiver), on informe l'utilisateur
                    if (existingRequest.RequesterId == requesterId && existingRequest.ReceiverId == receiverId)
                        throw new InvalidOperationException("Tu as déjà envoyé une demande d'amitié à cet utilisateur.");
                    
                    // Si c'est l'inverse (l'autre utilisateur a déjà envoyé une demande), on accepte automatiquement
                    if (existingRequest.RequesterId == receiverId && existingRequest.ReceiverId == requesterId)
                    {
                        // Accepter automatiquement la demande existante
                        var accepted = await AcceptFriendRequestAsync(existingRequest.Id, requesterId);
                        if (accepted)
                        {
                            // Retourner l'amitié créée
                            var friendship = await _context.Friendships
                                .Include(f => f.User)
                                .Include(f => f.Friend)
                                .FirstOrDefaultAsync(f => f.UserId == requesterId && f.FriendId == receiverId);
                            
                            if (friendship != null)
                            {
                                return new FriendRequestDto
                                {
                                    Id = existingRequest.Id,
                                    RequesterId = requesterId,
                                    RequesterName = friendship.User.Name,
                                    RequesterEmail = friendship.User.Email,
                                    ReceiverId = receiverId,
                                    ReceiverName = friendship.Friend.Name,
                                    ReceiverEmail = friendship.Friend.Email,
                                    CreatedAt = friendship.CreatedAt,
                                    Status = "Accepted"
                                };
                            }
                        }
                    }
                }
                if (existingRequest.Status == FriendRequestStatus.Accepted)
                    throw new InvalidOperationException("Vous êtes déjà amis avec cet utilisateur.");
            }

            // Vérifier si déjà amis
            var existingFriendship = await _context.Friendships
                .FirstOrDefaultAsync(f => (f.UserId == requesterId && f.FriendId == receiverId) ||
                                         (f.UserId == receiverId && f.FriendId == requesterId));

            if (existingFriendship != null && existingFriendship.Status == FriendshipStatus.Active)
                throw new InvalidOperationException("Ces utilisateurs sont déjà amis.");

            var requester = await _context.Users.FindAsync(requesterId);
            var receiver = await _context.Users.FindAsync(receiverId);

            if (requester == null || receiver == null)
                throw new ArgumentException("Utilisateur introuvable.");

            var request = new FriendRequest
            {
                RequesterId = requesterId,
                ReceiverId = receiverId,
                Status = FriendRequestStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.FriendRequests.Add(request);
            await _context.SaveChangesAsync();

            return new FriendRequestDto
            {
                Id = request.Id,
                RequesterId = requesterId,
                RequesterName = requester.Name,
                RequesterEmail = requester.Email,
                ReceiverId = receiverId,
                ReceiverName = receiver.Name,
                ReceiverEmail = receiver.Email,
                CreatedAt = request.CreatedAt,
                Status = request.Status.ToString()
            };
        }

        public async Task<bool> AcceptFriendRequestAsync(int requestId, int userId)
        {
            var request = await _context.FriendRequests
                .Include(fr => fr.Requester)
                .Include(fr => fr.Receiver)
                .FirstOrDefaultAsync(fr => fr.Id == requestId);

            if (request == null || request.ReceiverId != userId)
                return false;

            if (request.Status != FriendRequestStatus.Pending)
                return false;

            request.Status = FriendRequestStatus.Accepted;
            request.RespondedAt = DateTime.UtcNow;

            // Créer l'amitié dans les deux sens
            var friendship1 = new Friendship
            {
                UserId = request.RequesterId,
                FriendId = request.ReceiverId,
                Status = FriendshipStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            var friendship2 = new Friendship
            {
                UserId = request.ReceiverId,
                FriendId = request.RequesterId,
                Status = FriendshipStatus.Active,
                CreatedAt = DateTime.UtcNow
            };

            _context.Friendships.Add(friendship1);
            _context.Friendships.Add(friendship2);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectFriendRequestAsync(int requestId, int userId)
        {
            var request = await _context.FriendRequests.FindAsync(requestId);
            if (request == null || request.ReceiverId != userId)
                return false;

            if (request.Status != FriendRequestStatus.Pending)
                return false;

            request.Status = FriendRequestStatus.Rejected;
            request.RespondedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<FriendDto>> GetFriendsAsync(int userId)
        {
            // Optimisation : projection directe vers DTO pour éviter de charger toutes les données
            var now = DateTime.UtcNow;
            
            // Récupérer les amitiés avec projection optimisée (pas de Include, juste les champs nécessaires)
            var friendships = await _context.Friendships
                .Where(f => f.UserId == userId && f.Status == FriendshipStatus.Active)
                .Select(f => new
                {
                    FriendId = f.FriendId,
                    FriendName = f.Friend.Name,
                    FriendEmail = f.Friend.Email,
                    FriendLastLoginAt = f.Friend.LastLoginAt,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            if (!friendships.Any())
                return new List<FriendDto>();

            var friendIds = friendships.Select(f => f.FriendId).ToList();
            
            // Requête optimisée pour les sessions actives (uniquement les UserIds)
            var activeSessions = await _context.Sessions
                .Where(s => friendIds.Contains(s.UserId) && s.IsActive && s.ExpiresAt > now)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();

            // Récupérer les items équipés pour tous les amis en une seule requête
            var equippedItems = await _context.UserItems
                .Where(ui => friendIds.Contains(ui.UserId) && ui.IsEquipped)
                .Include(ui => ui.Item)
                .ToListAsync();

            // Grouper les items équipés par utilisateur
            var equippedItemsByUser = equippedItems
                .GroupBy(ui => ui.UserId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Mapper directement vers DTO avec les items équipés
            return friendships.Select(f => 
            {
                var friendEquippedItems = equippedItemsByUser.ContainsKey(f.FriendId)
                    ? equippedItemsByUser[f.FriendId]
                    : new List<Models.UserItem>();

                var avatar = friendEquippedItems.FirstOrDefault(ui => ui.Item.ItemType == "Avatar")?.Item;
                var theme = friendEquippedItems.FirstOrDefault(ui => ui.Item.ItemType == "Theme")?.Item;
                var decoration = friendEquippedItems.FirstOrDefault(ui => ui.Item.ItemType == "Decoration")?.Item;

                return new FriendDto
                {
                    Id = f.FriendId,
                    Name = f.FriendName,
                    Email = f.FriendEmail,
                    LastLoginAt = f.FriendLastLoginAt,
                    IsOnline = activeSessions.Contains(f.FriendId),
                    FriendsSince = f.CreatedAt,
                    EquippedItems = new DTOs.EquippedItemsDto
                    {
                        Avatar = avatar != null ? new DTOs.ItemDto
                        {
                            Id = avatar.Id,
                            Name = avatar.Name,
                            Description = avatar.Description,
                            Price = avatar.Price,
                            ItemType = avatar.ItemType,
                            Rarity = avatar.Rarity,
                            ImageUrl = avatar.ImageUrl,
                            Icon = avatar.Icon,
                            IsAvailable = avatar.IsAvailable
                        } : null,
                        Theme = theme != null ? new DTOs.ItemDto
                        {
                            Id = theme.Id,
                            Name = theme.Name,
                            Description = theme.Description,
                            Price = theme.Price,
                            ItemType = theme.ItemType,
                            Rarity = theme.Rarity,
                            ImageUrl = theme.ImageUrl,
                            Icon = theme.Icon,
                            IsAvailable = theme.IsAvailable
                        } : null,
                        Decoration = decoration != null ? new DTOs.ItemDto
                        {
                            Id = decoration.Id,
                            Name = decoration.Name,
                            Description = decoration.Description,
                            Price = decoration.Price,
                            ItemType = decoration.ItemType,
                            Rarity = decoration.Rarity,
                            ImageUrl = decoration.ImageUrl,
                            Icon = decoration.Icon,
                            IsAvailable = decoration.IsAvailable
                        } : null
                    }
                };
            }).ToList();
        }

        public async Task<List<FriendRequestDto>> GetPendingRequestsAsync(int userId)
        {
            var requests = await _context.FriendRequests
                .Include(fr => fr.Requester)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.ReceiverId == userId && fr.Status == FriendRequestStatus.Pending)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();

            return requests.Select(fr => new FriendRequestDto
            {
                Id = fr.Id,
                RequesterId = fr.RequesterId,
                RequesterName = fr.Requester.Name,
                RequesterEmail = fr.Requester.Email,
                ReceiverId = fr.ReceiverId,
                ReceiverName = fr.Receiver.Name,
                ReceiverEmail = fr.Receiver.Email,
                CreatedAt = fr.CreatedAt,
                Status = fr.Status.ToString()
            }).ToList();
        }

        public async Task<List<FriendRequestDto>> GetSentRequestsAsync(int userId)
        {
            var requests = await _context.FriendRequests
                .Include(fr => fr.Requester)
                .Include(fr => fr.Receiver)
                .Where(fr => fr.RequesterId == userId && fr.Status == FriendRequestStatus.Pending)
                .OrderByDescending(fr => fr.CreatedAt)
                .ToListAsync();

            return requests.Select(fr => new FriendRequestDto
            {
                Id = fr.Id,
                RequesterId = fr.RequesterId,
                RequesterName = fr.Requester.Name,
                RequesterEmail = fr.Requester.Email,
                ReceiverId = fr.ReceiverId,
                ReceiverName = fr.Receiver.Name,
                ReceiverEmail = fr.Receiver.Email,
                CreatedAt = fr.CreatedAt,
                Status = fr.Status.ToString()
            }).ToList();
        }

        public async Task<bool> RemoveFriendAsync(int userId, int friendId)
        {
            var friendships = await _context.Friendships
                .Where(f => (f.UserId == userId && f.FriendId == friendId) ||
                           (f.UserId == friendId && f.FriendId == userId))
                .ToListAsync();

            if (!friendships.Any())
                return false;

            _context.Friendships.RemoveRange(friendships);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AreFriendsAsync(int userId1, int userId2)
        {
            return await _context.Friendships
                .AnyAsync(f => f.UserId == userId1 && f.FriendId == userId2 && f.Status == FriendshipStatus.Active);
        }
    }
}
