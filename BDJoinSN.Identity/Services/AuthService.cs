using BDJoinSN.Application.Constans;
using BDJoinSN.Application.Contracts.Identity;
using BDJoinSN.Application.Contracts.Persistance;
using BDJoinSN.Application.Exceptions;
using BDJoinSN.Application.Models.Identity;
using BDJoinSN.Application.Models.Pagination;
using BDJoinSN.Domain;
using BDJoinSN.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace BDJoinSN.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IProfileCreationService _profileCreationService;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IProfileCreationService profileCreationService, IOptions<JwtSettings> jwtSettings, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _profileCreationService = profileCreationService;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<AuthResponse> Login(AuthRequest request)
        {
            ApplicationUser? findUser = null;
            bool isEmail = IsValidEmail(request.Email); 

            if(isEmail){
                
                findUser = await _userManager.FindByEmailAsync(request.Email);

                if (findUser == null){
                    _logger.LogWarning($"Intento de login con email no registrado: {request.Email}");
                    throw new NotFoundException("Email", request.Email);
                }

            }else{
                var usernameToSearch = string.IsNullOrWhiteSpace(request.Username) ? request.Email : request.Username;

                findUser = await _userManager.FindByNameAsync(usernameToSearch);
                if (findUser == null)
                {
                    _logger.LogWarning($"Intento de login con username no registrado: {usernameToSearch}");
                    throw new NotFoundException("Username", usernameToSearch);
                }
            }

            var resultado = await _signInManager.PasswordSignInAsync(findUser.UserName, request.Password, false, lockoutOnFailure: false);
            if (!resultado.Succeeded){
                _logger.LogWarning($"Intento de login fallido para: {findUser.UserName}");
                throw new BadRequestException("Las credenciales son incorrectas. Verifica tu email y contraseña.");
            }

            var token = await GenerateToken(findUser);
            var authResponse = new AuthResponse { 
                Id = findUser.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Email = findUser.Email,
                Username = findUser.UserName
            };

            return authResponse; 
        }

        public async Task<RegistrationResponse> Register(RegistrationRequest request)
        {
            if (string.IsNullOrEmpty(request.RepeatPassword)){
                _logger.LogWarning($"Intento de registro sin repetir contraseña");
                throw new BadRequestException("Debe de repetir la contraseña");
            }
            if (request.Password != request.RepeatPassword)
            {
                _logger.LogWarning($"Intento de registro con contraseñas diferentes: {request.Email}");
                throw new BadRequestException("Las contraseñas no coinciden.");
            }

            var userEmail = await _userManager.FindByEmailAsync(request.Email);
            if(userEmail != null){
                _logger.LogWarning("Intento de registro con email existente");
                throw new BadRequestException($"El email {request.Email} ya esta en uso.");
            }
            var userUsrName = await _userManager.FindByNameAsync(request.Username);

            if(userUsrName != null){
                _logger.LogWarning("Intento de registro con username existente");
                throw new BadRequestException($"El username {request.Username} ya esta en uso.");
            }

            var user = new ApplicationUser
            {
                Email = request.Email,
                Name = request.Name,
                LastName = request.Lastname,
                UserName = request.Username,
                DisplayName = $"{request.Name} {request.Lastname}",
                EmailConfirmed = true
            };


            var result = await _userManager.CreateAsync(user, request.Password);

            if(!result.Succeeded){
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError($"Error al crear usuario : {errors}");
                throw new Exception($"Error al crear el usuario: {errors}");
            }
            

            await _userManager.AddToRoleAsync(user, "AuthUser");

            await _profileCreationService.CreateProfileAsync(
               user.Id,
               request.Name,
               request.Lastname,
               $"{request.Name} {request.Lastname}", 
               request.Username
           );

            return new RegistrationResponse
            {
                Email = user.Email,
                UserId = user.Id,
                Username = user.UserName
            };

        }

        private async Task<JwtSecurityToken> GenerateToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var roleClaims = new List<Claim>();

            foreach(var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(CustomClaimTypes.Uid, user.Id),
            }.Union(userClaims).Union(roleClaims);

            var symmetricSecurity = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurity, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
                );


            return jwtSecurityToken;
        }


        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        
    }
}
