using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Api.Repository;
using System;
using System.Threading.Tasks;

namespace Simple.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IDataRepository _dataRepository;
        public const string HealthyMessage = "Simple Data API";
        public DataController(IDataRepository dataRepository, ILogger<DataController> logger)
        {
            _dataRepository = dataRepository;
            _logger = logger;
        }

        [HttpGet]
        public string Index() => HealthyMessage;

        // GET api/data/key/123
        [HttpGet("key/{key}")]
        public async Task<ActionResult<string>> GetDataAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return BadRequest();
                }
                _logger.LogDebug($"Find data with the key{key}");
                var data =await _dataRepository.GetDataAsync(key);
                if (data==null)
                {
                    _logger.LogDebug($"Data not found for the key{key}");
                    return NotFound();
                }
                _logger.LogDebug($"Data found: {key}: {data}");
                return Ok(data.Value);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception {e.Message} found in getting data for the key{key}. Details: {e.StackTrace} - {e.InnerException} ");
                return StatusCode(500);
            }
        }

        // POST api/data/key/123/value/HelloWorld
        [HttpPost("key/{key}/value/{value}")]
        public async Task<ActionResult> PostDataAsync(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return BadRequest();
                _logger.LogDebug($"Start to save data into repository for {key}:{value}");
                var existing = await _dataRepository.GetDataAsync(key);
                if (existing != null)
                {
                    return Conflict(existing);
                }
                await _dataRepository.CreateDataAsync(key, value);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogDebug($"{e} exception found in writing [{key}:{value}]. Details: {e}");
                return StatusCode(500); 
            }
        }

        // GET api/data/key/123/value/HelloWorld
        [HttpPut("key/{key}/value/{value}")]
        public async Task<ActionResult> UpdateDataAsync(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return BadRequest();
                _logger.LogDebug($"Start to save data into repository for {key}:{value}");
                var existing = await _dataRepository.GetDataAsync(key);
                if (existing != null)
                {
                    return Conflict(existing);
                }
                await _dataRepository.CreateDataAsync(key, value);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogDebug($"{e} exception found in writing [{key}:{value}]. Details: {e}");
                return StatusCode(500);
            }
        }


    }
}
