using Microsoft.Extensions.Caching.Memory;
using MoviesApi.DTOs;
using Newtonsoft.Json;

namespace MoviesApi.Services
{
    /// <summary>
    /// Service for interacting with the OMDB API to fetch movie details.
    /// Implements caching to optimize repeated requests.
    /// </summary>
    public class OmdbService : IOmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OmdbService> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Initializes a new instance of the <see cref="OmdbService"/> class.
        /// </summary>
        /// <param name="httpClient">HttpClient instance for making API requests.</param>
        /// <param name="cache">Memory cache for storing retrieved movies.</param>
        /// <param name="configuration">Configuration settings for API keys and URLs.</param>
        /// <param name="logger">Logger instance for logging events.</param>
        public OmdbService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration, ILogger<OmdbService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves movie details by title from the OMDB API, utilizing caching for optimization.
        /// </summary>
        /// <param name="title">The title of the movie to search.</param>
        /// <returns>
        /// A <see cref="MovieDTO"/> object containing movie details if found, otherwise null.
        /// </returns>
        public async Task<MovieDTO?> GetMovieByTitleAsync(string title)
        {
            // Check if the movie is already cached
            if (_cache.TryGetValue(title, out MovieDTO? cachedMovie) && cachedMovie != null)
            {
                _logger.LogInformation("CACHE HIT: Returning {Title} from cache.", title);
                return cachedMovie;
            }

            _logger.LogInformation("CACHE MISS: Fetching {Title} from OMDB API.", title);

            // Retrieve API key and base URL from configuration
            string? apiKey = _configuration["OmdbApi:ApiKey"];
            string? baseUrl = _configuration["OmdbApi:BaseUrl"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(baseUrl))
            {
                _logger.LogError("ERROR: OMDB API key or BaseUrl is missing.");
                return null;
            }

            // Construct the API request URL
            string requestUrl = $"{baseUrl}?t={Uri.EscapeDataString(title)}&apikey={apiKey}";

            _logger.LogInformation("Sending request to OMDB: {Url}", requestUrl);

            try
            {
                // Perform API request
                var response = await _httpClient.GetAsync(requestUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch data from OMDB. HTTP Status Code: {StatusCode}", response.StatusCode);
                    return null;
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("OMDB API Response: {Response}", responseBody);

                // Deserialize JSON response to MovieDTO
                var movie = JsonConvert.DeserializeObject<MovieDTO>(responseBody);

                if (movie != null && !string.IsNullOrEmpty(movie.Title))
                {
                    _logger.LogInformation("STORING IN CACHE: {Title}", movie.Title);
                    _cache.Set(title, movie, _cacheDuration);
                    return movie;
                }
                else
                {
                    _logger.LogWarning("MOVIE NOT FOUND: {Title}", title);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error fetching movie: {Message}", ex.Message);
            }

            return null;
        }
    }
}
