using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Api.Repository;
using System;

namespace Simple.Api.Controllers
{
    [Route("api/[controller]")]
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

        // POST api/data
        [HttpGet("key/{key}")]
        public ActionResult<string> GetData(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                {
                    return BadRequest();
                }
                _logger.LogDebug($"Find data with the key{key}");
                var data = _dataRepository.GetData(key);
                if (string.IsNullOrEmpty(data))
                {
                    _logger.LogDebug($"Data not found for the key{key}");
                    return NotFound();
                }
                _logger.LogDebug($"Data found: {key}: {data}");
                return Ok(data);
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception {e.Message} found in getting data for the key{key}. Details: {e.StackTrace} - {e.InnerException} ");
                return StatusCode(500);
            }
        }

        // GET api/data/key/123/value/HelloWorld
        [HttpPost("key/{key}/value/{value}")]
        public ActionResult PostData(string key, string value)
        {
            try
            {
                if (string.IsNullOrEmpty(key))
                    return BadRequest();
                _logger.LogDebug($"Start to save data into repository for {key}:{value}");
                _dataRepository.SaveData(key, value);
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
