using AutoMapper;
using Azure;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class VillaNumberAPIController : ControllerBase
	{
		protected APIResponse _response;
		private readonly IVillaRepository _dbVilla;
		private readonly IVillaNumberRepository _dbVillaNumber;
		private readonly IMapper _mapper;
		public VillaNumberAPIController(IVillaNumberRepository dbvillaRepository, IMapper mapper, IVillaRepository dbVilla)
		{
			_dbVillaNumber = dbvillaRepository;
			_mapper = mapper;
			_response = new();
			_dbVilla = dbVilla;
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<ActionResult<APIResponse>> GetVillasNumber()
		{
			try
			{
				IEnumerable<VillaNumber> villaNumberList = await _dbVillaNumber.GetAllAsync();
				_response.StatusCode = HttpStatusCode.OK;
				_response.Result = _mapper.Map<List<VillaNumberDTO>>(villaNumberList);
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string>() { ex.ToString() };
				return _response;
			}
		}

		[HttpGet("{id:int}", Name = "GetVillaNumber")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> GetVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}
				VillaNumber villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
				if (villaNumber == null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					return NotFound(_response);
				}
				else
				{
					_response.StatusCode = HttpStatusCode.OK;
					_response.IsSuccess = true;
					_response.Result = villaNumber;
					return Ok(villaNumber);
				}
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string>() { ex.ToString() };
				return _response;

			}
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDTO createDTO)	
		{
			try
			{
				if (await _dbVilla.GetAsync(u=>u.Id== createDTO.VillaID)==null)
				{
					_response.StatusCode= HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					_response.ErrorMessage = new List<string>() { "Villa ID does not exist !!" };
					return BadRequest(_response);
				}
				if (createDTO == null)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}
				if (await _dbVillaNumber.GetAsync(u => u.VillaNo == createDTO.VillaNo) != null)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					_response.ErrorMessage = new List<string>() { "Villa Number Already exist !!" };
					return BadRequest(_response);
				}
				VillaNumber villaNumber = _mapper.Map<VillaNumber>(createDTO);
				await _dbVillaNumber.CreateAsync(villaNumber);
				_response.StatusCode = HttpStatusCode.Created;
				_response.IsSuccess = true;
				_response.Result = createDTO;
				return CreatedAtRoute("GetVillaNumber",new { id = villaNumber.VillaNo},_response) ;
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string>() { ex.ToString() };
				return _response;
			}
		}	  


		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
		{
			try
			{
				if (id == 0)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					return BadRequest(_response);
				}
				var villaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);
				if (villaNumber==null)
				{
					_response.StatusCode = HttpStatusCode.NotFound;
					_response.IsSuccess = false;
					return NotFound(_response);
				}
				await _dbVillaNumber.RemoveAsync(villaNumber);
				_response.StatusCode=HttpStatusCode.OK;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage = new List<string>() { ex.ToString() };
				return _response;
			}
		}

		[HttpPut]
		public async Task <ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDTO updateDTO)
		{
			try
			{
				if (await _dbVilla.GetAsync(u => u.Id == updateDTO.VillaID) == null)
				{
					_response.StatusCode = HttpStatusCode.BadRequest;
					_response.IsSuccess = false;
					_response.ErrorMessage = new List<string>() { "Villa ID does not exist !!" };
					return BadRequest(_response);
				}
				if (updateDTO == null || id != updateDTO.VillaNo)
				{
					_response.IsSuccess = false;
					_response.StatusCode = HttpStatusCode.BadRequest;
					return BadRequest(_response);
				}
				VillaNumber villaNumber = _mapper.Map<VillaNumber>(updateDTO);
				await _dbVillaNumber.UpdateAsync(villaNumber);
				_response.Result = _mapper.Map<VillaNumberDTO>(villaNumber);
				_response.StatusCode = HttpStatusCode.NoContent;
				_response.IsSuccess = true;
				return Ok(_response);
			}
			catch (Exception ex)
			{
				_response.IsSuccess = false;
				_response.ErrorMessage
					= new List<string>() { ex.ToString() };
				return _response;
			}
		}
	}
}
