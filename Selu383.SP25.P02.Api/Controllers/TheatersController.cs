using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Selu383.SP25.P02.Api.Features.Users;

namespace Selu383.SP25.P02.Api.Controllers
{
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly DbSet<Theater> theaters;
        private readonly DataContext dataContext;
        private readonly UserManager<User> userManager;

        public TheatersController(DataContext dataContext, UserManager<User> userManager)
        {
            this.dataContext = dataContext;
            this.theaters = dataContext.Set<Theater>();
            this.userManager = userManager;
        }

        [HttpGet]
        public IQueryable<TheaterDto> GetAllTheaters()
        {
            return GetTheaterDtos(theaters);
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult<TheaterDto> GetTheaterById(int id)
        {
            var result = GetTheaterDtos(theaters.Where(x => x.Id == id)).FirstOrDefault();
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<TheaterDto>> CreateTheater(TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var user = await userManager.GetUserAsync(User);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            // Only admins can set ManagerId
            if (dto.ManagerId.HasValue && !isAdmin)
            {
                return Forbid();
            }

            var theater = new Theater
            {
                Name = dto.Name,
                Address = dto.Address,
                SeatCount = dto.SeatCount,
                ManagerId = dto.ManagerId
            };

            theaters.Add(theater);
            dataContext.SaveChanges();
            dto.Id = theater.Id;

            return CreatedAtAction(nameof(GetTheaterById), new { id = dto.Id }, dto);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult<TheaterDto>> UpdateTheater(int id, TheaterDto dto)
        {
            if (IsInvalid(dto))
            {
                return BadRequest();
            }

            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            // Check if user is admin or the theater manager
            if (!isAdmin && theater.ManagerId != user.Id)
            {
                return Forbid();
            }

            // Only admins can change ManagerId
            if (dto.ManagerId != theater.ManagerId && !isAdmin)
            {
                return Forbid();
            }

            theater.Name = dto.Name;
            theater.Address = dto.Address;
            theater.SeatCount = dto.SeatCount;
            theater.ManagerId = dto.ManagerId;

            dataContext.SaveChanges();
            dto.Id = theater.Id;

            return Ok(dto);
        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<ActionResult> DeleteTheater(int id)
        {
            var theater = theaters.FirstOrDefault(x => x.Id == id);
            if (theater == null)
            {
                return NotFound();
            }

            var user = await userManager.GetUserAsync(User);
            var isAdmin = await userManager.IsInRoleAsync(user, "Admin");

            // Only admins can delete theaters
            if (!isAdmin)
            {
                return Forbid();
            }

            theaters.Remove(theater);
            dataContext.SaveChanges();
            return Ok();
        }

        private static bool IsInvalid(TheaterDto dto)
        {
            return string.IsNullOrWhiteSpace(dto.Name) ||
                   dto.Name.Length > 120 ||
                   string.IsNullOrWhiteSpace(dto.Address) ||
                   dto.SeatCount <= 0;
        }

        private static IQueryable<TheaterDto> GetTheaterDtos(IQueryable<Theater> theaters)
        {
            return theaters
                .Select(x => new TheaterDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    SeatCount = x.SeatCount,
                    ManagerId = x.ManagerId
                });
        }
    }
}
