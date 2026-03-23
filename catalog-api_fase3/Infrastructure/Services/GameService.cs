
using Core.Entity;
using Core.Input;
using Core.Repository;
using Core.Services;

namespace Infrastructure.Services
{
    public class GameService : BaseService<Game>, IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository) : base(gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public IList<GameDto> ObterTodosDto()
        {
            return _gameRepository
                .ObterTodos()
                .Select(MapToDto)
                .ToList();
        }

        public GameDto? ObterPorIdDto(int id)
        {
            var game = _gameRepository.ObterPorId(id);

            if (game == null)
                return null;

            return MapToDto(game);
        }

        private static GameDto MapToDto(Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                CreatedAt = game.CreatedAt,
                UpdatedAt = game.UpdatedAt,
                Title = game.Title,
                Description = game.Description,
                Price = game.Price,
                Genre = game.Genre,
                Developer = game.Developer,
                ReleaseDate = game.ReleaseDate
            };
        }

        public Task<List<GameRecommendationDto>> GetRecommendationsAsync(int userId, int top = 5)
        {
            return _gameRepository.GetRecommendationsAsync(userId, top);
        }
    }
}