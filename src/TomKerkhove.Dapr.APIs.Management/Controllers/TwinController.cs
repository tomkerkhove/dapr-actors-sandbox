﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GuardNet;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using TomKerkhove.Dapr.Actors.Device.Interface.Contracts;
using TomKerkhove.Dapr.APIs.Management.Repositories;

namespace TomKerkhove.Dapr.APIs.Management.Controllers
{
    [ApiController]
    [Route("api/v1/devices")]
    public class TwinController : ControllerBase
    {
        private readonly DeviceRepository _deviceRepository;

        public TwinController(DeviceRepository deviceRepository)
        {
            Guard.NotNull(deviceRepository,nameof(deviceRepository));

            _deviceRepository = deviceRepository;
        }

        /// <summary>
        ///     Report Twin Properties
        /// </summary>
        /// <remarks>Provides capability to report one or more twin reported properties for the device.</remarks>
        /// <param name="deviceId">Unique id for a given device</param>
        /// <param name="reportedProperties">Collection of reported properties for the device twin</param>
        /// <response code="200">Properties were reported for the device</response>
        /// <response code="400">Message sent to device</response>
        /// <response code="503">We are undergoing some issues</response>
        [HttpPost("{deviceId}/twin/properties/reported", Name = "Twin_ReportTwinProperties")]
        [ProducesResponseType(typeof(DeviceInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.BadRequest, (int)HttpStatusCode.InternalServerError }, "RequestId", "string", "The header that has a request ID that uniquely identifies this operation call")]
        [SwaggerResponseHeader(new[] { (int)HttpStatusCode.OK, (int)HttpStatusCode.BadRequest, (int)HttpStatusCode.InternalServerError }, "X-Transaction-Id", "string", "The header that has the transaction ID is used to correlate multiple operation calls.")]
        public async Task<IActionResult> ReportTwinProperties([FromRoute] string deviceId, [Required, FromBody] Dictionary<string, string> reportedProperties)
        {
            if (reportedProperties?.Any() == false)
            {
                return ValidationProblem("No reported properties were specified.");
            }

            await _deviceRepository.ReportPropertiesAsync(deviceId, reportedProperties);
            return Ok();
        }
    }
}